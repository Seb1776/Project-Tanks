using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    public enum FireMode {SingleShot, SemiAuto, ShotgunBurst, FullAuto, AutoShotgunBurst}
    public FireMode currentFireMode;
    public Projectile flankProjectile;
    public Transform firePoint;
    public int clipSize;
    public int magSize;
    public int currentClip;
    public int currentMag;
    public float timeBtwShots;
    public float reloadTime;
    public float originalReloadTime;
    public float recoil;
    public bool reloading;
    public bool canShoot;
    public bool multipleBullets;
    public Vector2 shotgunSpread;
    public int shotgunSpreadBullets;
    public int semiAutoBullets;

    float currentTimeBtwShots;
    float currentReloadTime;

    void Start()
    {
        SetStarterVariables();
    }

    void Update()
    {
        HandleReload();
        HandleShooting();
        HandleMultipleBullets();
    }

    public void Shoot()
    {   
        if (!reloading || (currentClip > 0 && currentMag > 0))
        {
            switch (currentFireMode)
            {
                case FireMode.SingleShot:
                    Instantiate(flankProjectile.gameObject, firePoint.position, Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, 90f)));
                    currentClip--;
                break;

                case FireMode.ShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < shotgunSpreadBullets; i++)
                        {
                            float randRotation = Random.Range(shotgunSpread.x, shotgunSpread.y) + 70f;
                            Quaternion bulletSpread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotation));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                            currentClip--;

                            canShoot = false;
                        }
                    }
                break;

                case FireMode.SemiAuto:
                    multipleBullets = true;
                break;

                case FireMode.FullAuto:
                    if (canShoot)
                    {
                        Instantiate(flankProjectile.gameObject, firePoint.position, Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, 90f)));
                        currentClip--;
                        canShoot = false;
                    }
                break;

                case FireMode.AutoShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < shotgunSpreadBullets; i++)
                        {
                            float randRotation = Random.Range(shotgunSpread.x, shotgunSpread.y) + 70f;
                            Quaternion bulletSpread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotation));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                            currentClip--;

                            canShoot = false;
                        }
                    }
                break;
            }

            if (currentClip <= 0)
            {
                currentClip = 0;
                reloading = true;
            }
        }
    }

    void HandleMultipleBullets()
    {
        
    }

    public void AddAmmo(int amount)
    {
        currentMag += amount;

        if (currentMag > magSize)
            currentMag = magSize;
    }

    void SetStarterVariables()
    {
        originalReloadTime = reloadTime;
        currentClip = clipSize;
        currentMag = magSize;

        if (shotgunSpread.x < 0f)
            shotgunSpread.x = 0f;
        
        if (shotgunSpread.x > 360f)
            shotgunSpread.x = 360f;
        
        if (shotgunSpread.y < 0f)
            shotgunSpread.y = 0f;
        
        if (shotgunSpread.y > 360f)
            shotgunSpread.y = 360f;
    }

    void HandleShooting()
    {
        if (!canShoot)
        {
            if (currentTimeBtwShots >= timeBtwShots)
            {
                currentTimeBtwShots = 0f;
                canShoot = true;
            }

            else
                currentTimeBtwShots += Time.deltaTime;
        }
    }

    void HandleReload()
    {
        if (reloading && magSize > 0)
        {
            if (currentReloadTime >= reloadTime)
            {
                if (currentMag < currentClip)
                {
                    currentClip = clipSize;
                    currentMag = 0;
                }

                else if (currentMag > currentClip)
                {
                    currentClip = clipSize;
                    currentMag -= clipSize;
                }

                reloading = false;
                currentReloadTime = 0;
            }

            else
                currentReloadTime += Time.deltaTime;
        }
    }
}
