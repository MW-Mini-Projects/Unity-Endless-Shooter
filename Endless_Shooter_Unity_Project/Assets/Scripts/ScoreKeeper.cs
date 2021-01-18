using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score;
    float lastEnemyKillTime;
    public static int streakCount;
    float streakExpiryTime = 1f;

    public Spawner spawner;

    void Start()
    {
        score = 0;
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void Update()
    {
        if (Time.time > lastEnemyKillTime + streakExpiryTime)
        {
            streakCount = 0;
        }
    }

    void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakCount++;
        }
        lastEnemyKillTime = Time.time;

        score += 5 + spawner.waveNumberIndex() * 5 + streakCount * 3;
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
