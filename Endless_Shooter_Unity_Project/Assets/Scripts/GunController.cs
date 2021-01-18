using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] allGuns;
    Gun equippedGun;
    int weaponIndex;

    public event System.Action OnNewWeapon;

    void Start()
    {
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        //Need to cast the instantiated object as a Gun 
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        //Make the gun a child of the parent, so that it moves with the player
        equippedGun.transform.parent = weaponHold;
        if(OnNewWeapon != null)
        {
            OnNewWeapon();
        }
    }

    public void EquipGun(int _weaponIndex)
    {
        weaponIndex = _weaponIndex;
        EquipGun(allGuns[weaponIndex]);
    }

    public int getWeaponIndex()
    {
        return weaponIndex;
    }

    public void OnTriggerHold()
    {
        if(equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get {
            return weaponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }
}
