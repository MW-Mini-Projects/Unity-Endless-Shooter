using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemaingingToSpawn;
    int enemiesAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingCheck = 2;
    float campThreshholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    int newInfinityMap = 30;
    int infinityCount = 0;
    //Event that helps other scripts to keep track of when a new wave starts
    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        //Set variables needed to check if the player is camping
        nextCampCheckTime = timeBetweenCampingCheck + Time.time;
        campPositionOld = playerT.position;
        //Subscribe a method to the players death
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();    
    }

    void Update()
    {
        //Avoid trying to spawn enemeis when the player is dead
        if(!isDisabled)
        {
            //Check every campCheck if the player is camping
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingCheck;
                //If the player has not moved far enough, set camping to true
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThreshholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemaingingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemaingingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        //Only for developer use
        if(devMode)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        //If the player is camping set the spawnTile to the players position
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = Color.white;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            //Mathf.PingPong will go between 0 & specified number, a number of times per second
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        //Delegate the event OnDeath to reference the method OnEnemyDeath()
        //so that the function is called when the event happens
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.enemyMoveSpeed, currentWave.attkDmg, currentWave.enemyHealth, currentWave.skinColour);
    }

    //Disable the spawner when the player is dead
    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    //Method called when the even OnDeath is published
    //from LivingEntity
    void OnEnemyDeath()
    {
        enemiesAlive--;
        if(currentWave.infinite)
        {
            infinityCount++;
            if(infinityCount == newInfinityMap)
            {
                infinityCount = 0;
                NextWave();
            }
        }
        if(enemiesAlive == 0)
        {
            NextWave();
        }
    }

    //When a new wave starts reset the players position to the middle
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    //Instanciates the next wave
    void NextWave()
    {
        //A bit of a hack to generate new maps for the infinite level
        if(currentWave != null)
        {
            if (currentWave.infinite)
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                //Set the flag for the event, if someone is subscribed to it
                if (OnNewWave != null)
                {
                    OnNewWave(currentWaveNumber);
                }
                ResetPlayerPosition();
                return;
            }
        }

        if(currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }

        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemaingingToSpawn = currentWave.enemyCount;
            enemiesAlive = enemiesRemaingingToSpawn;

            //Set the flag for the event, if someone is subscribed to it
            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    public int waveNumberIndex()
    {
        return currentWaveNumber;
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float enemyMoveSpeed;
        public float attkDmg;
        public float enemyHealth;
        public Color skinColour;
    }
}
