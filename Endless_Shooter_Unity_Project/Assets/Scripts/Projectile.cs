using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public Color trailColour;

    float speed = 10;
    float damage = 1;

    float lifetime = 3;
    float skinWidth = .1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        //Check right away if we are already colliding with an enemy
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            //Call OnHitObject that takes a Collider instead of a Raycasthit
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().startColor = trailColour;
    }

    public void SetProjectileSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    //Each frame check if we collide with what we specified in our collisionMask
    void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        //Get & check if hit object implements the interface IDamageable
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        //print(hit.collider.gameObject.name);
        GameObject.Destroy(gameObject);
    }
}
