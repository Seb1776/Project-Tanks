using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    public enum FireMode {SingleShot, SemiAuto, ShotgunBurst, FullAuto, AutoShotgunBurst}
    public FireMode currentFireMode;
    public Projectile flankProjectile;
    public Transform firePoint;
    public Vector2 normalSpread;
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
    public float timeBtwSAShots;

    int currentAutoClipped;
    float currentTimeBtwSAShots;
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
        if (!reloading && (currentMag != 0 || currentClip != 0))
        {
            switch (currentFireMode)
            {
                case FireMode.SingleShot:
                    float randRotationss = Random.Range(normalSpread.x, normalSpread.y) + 90f;
                    Quaternion bulletSpreadss = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationss));
                    Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadss);
                    currentClip--;
                break;

                case FireMode.ShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < shotgunSpreadBullets; i++)
                        {
                            float randRotationsb = Random.Range(shotgunSpread.x, shotgunSpread.y) + 70f;
                            Quaternion bulletSpreadsb = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationsb));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadsb);
                            tmpProj.GetComponent<Projectile>().damage /= shotgunSpreadBullets;
                            currentClip--;

                            if (currentClip <= 0)
                                break;
                        }

                        canShoot = false;
                    }
                break;

                case FireMode.SemiAuto:
                    if (!multipleBullets && canShoot)
                        multipleBullets = true;
                break;

                case FireMode.FullAuto:
                    if (canShoot)
                    {
                        float randRotation = Random.Range(normalSpread.x, normalSpread.y) + 90f;
                        Quaternion bulletSpread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotation));
                        Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                        currentClip--;
                        canShoot = false;
                    }
                break;

                case FireMode.AutoShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < shotgunSpreadBullets; i++)
                        {
                            float randRotationass = Random.Range(shotgunSpread.x, shotgunSpread.y) + 70f;
                            Quaternion bulletSpreadass = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationass));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadass);
                            currentClip--;

                            if (currentClip <= 0)
                                break;
                        }

                        canShoot = false;
                    }
                break;
            }

            if (currentClip <= 0)
            {
                if (multipleBullets)
                    multipleBullets = false;

                currentClip = 0;
                reloading = true;
            }
        }
    }

    void HandleMultipleBullets()
    {
        if (multipleBullets)
        {
            for (int i = 0; i < semiAutoBullets; i++)
            {
                if (currentTimeBtwSAShots <= 0f)
                {
                    float randRotation = Random.Range(normalSpread.x, normalSpread.y) + 90f;
                    Quaternion bulletSpread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotation));
                    Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                    currentClip--;
                    currentAutoClipped++;

                    if (currentClip <= 0)
                    {
                        currentAutoClipped = 0;
                        multipleBullets = false;
                        reloading = true;
                        break;
                    }

                    else if (currentAutoClipped >= semiAutoBullets)
                    {
                        multipleBullets = false;
                        canShoot = false;
                        currentAutoClipped = 0;
                        break;
                    }

                    currentTimeBtwSAShots = timeBtwSAShots;
                }

                else
                    currentTimeBtwSAShots -= Time.deltaTime;
            }
        }
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
                if (clipSize > currentMag)
                {
                    currentClip = currentMag;
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
