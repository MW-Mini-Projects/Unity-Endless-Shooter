using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletCounter : MonoBehaviour
{
    public GameObject[] ammoTypes;
    public GameObject[] shotGun;
    public Image[] basicGun;
    public Image[] fastGun;

    public Text bulletCounter;
    public Spawner spawner;
    public GameObject gunHolder;
    public Player player;

    Gun currentGun;
    GunController gunController;

    // Start is called before the first frame update
    void Start()
    {
        if(player != null)
        {
            gunController = player.GetComponent<GunController>();
            gunController.OnNewWeapon += OnSwitchWeapon;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gunHolder != null && currentGun == null)
        {
            currentGun = gunHolder.transform.GetChild(0).GetComponent<Gun>();
        }
        if (currentGun != null)
        {
            if (currentGun.fireMode == Gun.FireMode.Single || currentGun.fireMode == Gun.FireMode.Auto)
            {
                if(gunController.getWeaponIndex() == 0)
                {
                    displayBasicAmmo();
                }
                else if(gunController.getWeaponIndex() == 3)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        shotGun[i].SetActive(true);
                    }
                    for(int i = currentGun.projectilesPerMag/3; i > currentGun.projectilesRemainingInMag/3; i--)
                    {
                        shotGun[i - 1].SetActive(false);
                    }
                } else
                {
                    for (int i = 0; i < currentGun.projectilesPerMag; i++)
                    {
                        if (i <= currentGun.projectilesRemainingInMag - 1)
                        {
                            fastGun[i].enabled = true;
                        }
                        else
                        {
                            fastGun[i].enabled = false;
                        }
                    }
                }

                bulletCounter.text = "x" + currentGun.projectilesRemainingInMag;
            }
            else if (currentGun.fireMode == Gun.FireMode.Burst)
            {
                if (gunController.getWeaponIndex() == 1)
                {
                    displayBasicAmmo();
                }
                bulletCounter.text = "x" + currentGun.projectilesRemainingInMag;
            }
        }
    }

    void displayBasicAmmo()
    {
        for(int i = 0; i < currentGun.projectilesPerMag; i++)
        {
            if(i <= currentGun.projectilesRemainingInMag - 1)
            {
                basicGun[i].enabled = true;
            } else
            {
                basicGun[i].enabled = false;
            }
        }
    }

    void OnSwitchWeapon ()
    {
        int setActive;
        if(gunController.getWeaponIndex() == 0 || gunController.getWeaponIndex() == 1)
        {
            setActive = 0;
            bulletCounter.rectTransform.anchoredPosition = new Vector2(4, 2);
        } else if (gunController.getWeaponIndex() == 2 || gunController.getWeaponIndex() == 4)
        {
            setActive = 1;
            bulletCounter.rectTransform.anchoredPosition = new Vector2(4, 2);
        } else
        {
            setActive = 2;
            bulletCounter.rectTransform.anchoredPosition = new Vector2(-55, 2);
        }

        for (int i = 0; i < 3; i++)
        {
            if(i == setActive)
            {
                ammoTypes[i].SetActive(true);
            } else
            {
                ammoTypes[i].SetActive(false);
            }
        }
    }
}
