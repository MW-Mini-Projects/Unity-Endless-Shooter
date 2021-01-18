using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]

public class Player : LivingEntity
{

    public float moveSpeed = 5;

    public Crosshairs crosshairs;

    Camera viewCamera;
    //Will be doing the calculations for the player
    PlayerController controller;
    GunController gunController;
    int waveNumber;
    int latestGun;

    // Start is called before the first frame update
    protected override void Start()
    {
        AudioManager.instance.setPlayerTransform();
        base.Start();
    }

    //Ensure this code is ran before the Spawner code
    void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        //Subscribe to the spawners new wave event
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int _waveNumber)
    {
        health = startingHealth;
        if(waveNumber == _waveNumber)
        {
            int newRand = Random.Range(0, 5);
            while(newRand == latestGun)
            {
                newRand = Random.Range(0, 5);
            }
            latestGun = newRand;
            gunController.EquipGun(latestGun);
            return;
        }
        waveNumber = _waveNumber;
        latestGun = waveNumber - 1;
        gunController.EquipGun(waveNumber - 1);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement input
        //Get horizontal/vertical direction, normalize it and set the magnitude to the movespeed
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //Look input ----
        //Cast a ray from the camera to the mousepointer 
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        //Check if the ray from camera to mouse hits the plane, if so get magnitude/distance
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            //Send found point on plane to playerController
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            //Send the aim vector point to the gun aim
            //so we can rotate the gun towards the point
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }
        //---------------

        //Weapon input
        if(Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown (KeyCode.R))
        {
            gunController.Reload();
        }
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}
