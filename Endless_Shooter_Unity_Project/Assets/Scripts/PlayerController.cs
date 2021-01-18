using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class PlayerController: MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        //Avoid rotation on y-axis, use own transform y and only use lookPoint x & z
        Vector3 playerLookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        //Rotate the player towards the lookPoint
        transform.LookAt(playerLookPoint);
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
