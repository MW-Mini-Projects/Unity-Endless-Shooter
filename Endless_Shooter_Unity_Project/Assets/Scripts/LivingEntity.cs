using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health;
    protected bool dead;

    //Our death even that removes the need for the 
    //living entity to keep track of the spawner
    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        //Will work on this later with hit var
        TakeDamage(damage);
    }

    //Simpler method of take hit method
    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        dead = true;
        //Check if someone is subscribed to the death event
        //if so invoke/activate the event
        OnDeath?.Invoke();
        GameObject.Destroy(gameObject);
    }
}
