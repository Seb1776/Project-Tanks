using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    public WeaponStat weaponStat;
    [HideInInspector] public enum FireMode {SingleShot, SemiAuto, ShotgunBurst, FullAuto, AutoShotgunBurst}
    [HideInInspector] public FireMode currentFireMode;
    public Projectile flankProjectile;
    public Transform firePoint;
    [HideInInspector] public string weaponName;
    [HideInInspector] public Vector2 normalSpread;
    [HideInInspector] public Vector2 focusedNormalSpread;
    [HideInInspector] public Vector2 originalNormalSpread;
    [HideInInspector] public List<Vector2> optimalDamageRangeMultiplier = new List<Vector2>();
    [HideInInspector] public int clipSize;
    [HideInInspector] public int magSize;
    [HideInInspector] public int damage;
    public int currentClip;
    public int currentMag;
    [HideInInspector] public float timeBtwShots;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public float originalReloadTime;
    [HideInInspector] public bool reloading;
    [HideInInspector] public bool canShoot;
    public bool consumeMag;
    [HideInInspector] public bool multipleBullets;
    [HideInInspector] public Vector2 shotgunSpread;
    [HideInInspector] public int shotgunSpreadBullets;
    [HideInInspector] public int semiAutoBullets;
    [HideInInspector] public float timeBtwSAShots;
    [HideInInspector] public Vector2 ammoPickupChance;
    [HideInInspector] public LivingThing entity;
    [HideInInspector] public AudioClip shootSFX;

    int currentAutoClipped;
    float currentTimeBtwSAShots;
    float currentTimeBtwShots;
    float currentReloadTime;
    Animator animator;
    AudioSource source;

    void Awake()
    {
        SetWeaponStats();
    }

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
                    if (canShoot)
                    {
                        float randRotationss = Random.Range(normalSpread.x, normalSpread.y) + 90f;
                        Quaternion bulletSpreadss = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationss));
                        GameObject tmpProja = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadss);
                        tmpProja.GetComponent<Projectile>().firedFrom = firePoint.position;
                        tmpProja.GetComponent<Projectile>().parentFlank = this;
                        SetProjectileSender(tmpProja);
                        animator.SetTrigger("shoot");
                        source.PlayOneShot(shootSFX);
                        currentClip--;

                        canShoot = false;
                    }
                break;

                case FireMode.ShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < shotgunSpreadBullets; i++)
                        {
                            float randRotationsb = Random.Range(shotgunSpread.x, shotgunSpread.y) + 90f;
                            Quaternion bulletSpreadsb = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationsb));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadsb);
                            tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                            tmpProj.GetComponent<Projectile>().parentFlank = this;
                            SetProjectileSender(tmpProj);
                            source.PlayOneShot(shootSFX);
                            currentClip--;

                            if (currentClip <= 0)
                                break;
                        }

                        animator.SetTrigger("shoot");
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
                        GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                        tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                        tmpProj.GetComponent<Projectile>().parentFlank = this;
                        SetProjectileSender(tmpProj);
                        source.PlayOneShot(shootSFX);
                        animator.SetTrigger("shoot");
                        currentClip--;
                        canShoot = false;
                    }
                break;

                case FireMode.AutoShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < shotgunSpreadBullets; i++)
                        {
                            float randRotationass = Random.Range(shotgunSpread.x, shotgunSpread.y) + 90f;
                            Quaternion bulletSpreadass = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationass));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadass);
                            tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                            tmpProj.GetComponent<Projectile>().parentFlank = this;
                            SetProjectileSender(tmpProj);
                            source.PlayOneShot(shootSFX);
                            currentClip--;

                            if (currentClip <= 0)
                                break;
                        }

                        animator.SetTrigger("shoot");
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

    void SetProjectileSender(GameObject p)
    {
        if (entity.GetComponent<Player>() != null)
            p.transform.tag = "PlayerBullet";
        
        else if (entity.GetComponent<Enemy>() != null)
            p.transform.tag = "EnemyBullet";
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
                    GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                    tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                    tmpProj.GetComponent<Projectile>().parentFlank = this;
                    SetProjectileSender(tmpProj);
                    source.PlayOneShot(shootSFX);
                    animator.SetTrigger("shoot");
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
        originalNormalSpread = normalSpread;
        focusedNormalSpread.x = normalSpread.x / 2f;
        focusedNormalSpread.y = normalSpread.y / 2f;
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void SetWeaponStats()
    {
        switch (weaponStat.weaponType)
        {
            case WeaponStat.WeaponType.SingleShot:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.SemiAuto:
                currentFireMode = FireMode.SemiAuto;
            break;

            case WeaponStat.WeaponType.ShotgunBurst:
                currentFireMode = FireMode.ShotgunBurst;
            break;

            case WeaponStat.WeaponType.FullAuto:
                currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.AutoShotgunBurst:
                currentFireMode = FireMode.AutoShotgunBurst;
            break;
        }

        clipSize = weaponStat._clipSize;
        magSize = weaponStat._magSize;
        timeBtwShots = weaponStat._timeBtwShots;
        reloadTime = weaponStat._reloadTime;
        normalSpread = weaponStat._normalSpread;
        shotgunSpreadBullets = weaponStat._shotgunSpreadBullets;
        semiAutoBullets = weaponStat._semiAutoBullets;
        ammoPickupChance = weaponStat._ammoPickupChance;
        shotgunSpread = weaponStat._shotgunSpread;
        optimalDamageRangeMultiplier = weaponStat._optimalDamageRangeMultiplier;
        weaponName = weaponStat._weaponSettingName;
        damage = weaponStat._damage;
        shootSFX = weaponStat._shootSFX;
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
                    
                    if (consumeMag)
                        currentMag = 0;
                }

                else if (currentMag > currentClip)
                {
                    currentClip = clipSize;

                    if (consumeMag)
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
