using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity 
{
    //Different states the enemy can be in
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    //Splatter effect when the enemy dies
    public ParticleSystem deathEffect;

    public static event System.Action OnDeathStatic;

    NavMeshAgent pathfinder;
    //Most likely only going to be the player
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Color originalColour;

    float attackDistanceTreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    //SetCharacteristics will be called before the enemy is spawned
    //We solve this by using a awake method (called when we instantiate the enemy)
    void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        //Player might be dead by the time we spawn the enemy
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            //Get the enemies and targets radius, to avoid walking into the player
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        //Call the base start (from Living Entity)
        //and then do the extra stuff "override"
        base.Start();

        if(hasTarget)
        {
            //By the default the enemy chases the player
            currentState = State.Chasing;

            //We have in the awake method the players living entity transform
            //subscribe to its OnDeath event
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
    }

    //When creating the enemy we can set certain characteristics
    public void SetCharacteristics(float moveSpeed, float attkDmg, float health, Color skinColour)
    {
        pathfinder.speed = moveSpeed;
        damage = attkDmg;
        startingHealth = health;

        //Set the particle systems color to the enemies color
        var main = deathEffect.main;
        main.startColor = new Color(skinColour.r, skinColour.g, skinColour.b, 1);

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColour;
        originalColour = skinMaterial.color;
    }

    public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    //When the player dies go to an idle state
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                //Compare the sqrt distance from the target, compared to the attack distance + the radius of the enemy&player
                if (sqrDstToTarget < Mathf.Pow(attackDistanceTreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    //Start a coroutine for our attack so
                    //we don't block everything else
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        //Ensure that the enemy doesn't move while attacking
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 startPosition = transform.position;
        //Make it so that the enemy doesn't go into the player when attacking
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.yellow;
        bool hasAppliedDamage = false;

        //Attack animation
        while (percent <= 1)
        {

            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            //Start from 0 go to 1
            percent += Time.deltaTime * attackSpeed;
            //4 * (-x^2 + x) is our interpolation curve
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            //Linearly interpolate between two points
            transform.position = Vector3.Lerp(startPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColour;
        //Enable the enemy to walk again
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    //Lower the load only update the enemies path each 1/4 of a second
    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (hasTarget)
        {
            //Check that we are not in a attacking/idle state
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                //Remove from target position the radius from enemy & player as well as half the attack distance
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceTreshold/2);
                //Ensure the enemy is not already dead
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
