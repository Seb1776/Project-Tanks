using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flank : MonoBehaviour
{
    public bool boughtWeapon;
    public WeaponStat weaponStat;
    public List<WeaponStat> flankWeaponInventory = new List<WeaponStat>();
    [HideInInspector] public enum FireMode {SingleShot, SemiAuto, ShotgunBurst, FullAuto, AutoShotgunBurst}
    [HideInInspector] public FireMode currentFireMode;
    public Projectile flankProjectile;
    public Transform firePoint;
    [HideInInspector] public string weaponName;
    [HideInInspector] public Vector2 accuracy;
    [HideInInspector] public Vector2 focusedAccuracy;
    [HideInInspector] public Vector2 originalAccuracy;
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
    public UnityEvent OnFiredBullet;
    public UnityEvent OnReloadWeapon;
    [HideInInspector] public bool multipleBullets;
    [HideInInspector] public int burstBulletsToFire;
    [HideInInspector] public float timeBtwSAShots;
    [HideInInspector] public Vector2 ammoPickupChance;
    [HideInInspector] public LivingThing entity;
    [HideInInspector] public AudioClip shootSFX;

    int currentAutoClipped;
    float currentTimeBtwSAShots;
    float currentTimeBtwShots;
    public float currentReloadTime;
    Animator animator;
    AudioSource source;
    GameManager manager;

    void Awake()
    {   
        if (boughtWeapon)
            SetWeaponStats();
    }

    void Start()
    {   
        if (boughtWeapon)
            SetStarterVariables();
    }

    void Update()
    {   
        if (boughtWeapon)
        {
            HandleReload();
            HandleShooting();
            HandleMultipleBullets();
        }
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
                        float randRotationss = Random.Range(accuracy.x, accuracy.y) + 90f;
                        Quaternion bulletSpreadss = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationss));
                        GameObject tmpProja = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadss);
                        tmpProja.GetComponent<Projectile>().firedFrom = firePoint.position;
                        tmpProja.GetComponent<Projectile>().parentFlank = this;
                        SetProjectileSender(tmpProja);
                        animator.SetTrigger("shoot");
                        currentClip--;
                        OnFiredBullet.Invoke();

                        canShoot = false;
                    }
                break;

                case FireMode.ShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < burstBulletsToFire; i++)
                        {
                            float randRotationsb = Random.Range(accuracy.x, accuracy.y) + 90f;
                            Quaternion bulletSpreadsb = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationsb));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadsb);
                            tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                            tmpProj.GetComponent<Projectile>().parentFlank = this;
                            SetProjectileSender(tmpProj);
                            currentClip--;
                            OnFiredBullet.Invoke();

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
                        float randRotation = Random.Range(accuracy.x, accuracy.y) + 90f;
                        Quaternion bulletSpread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotation));
                        GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                        tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                        tmpProj.GetComponent<Projectile>().parentFlank = this;
                        SetProjectileSender(tmpProj);
                        animator.SetTrigger("shoot");
                        currentClip--;
                        OnFiredBullet.Invoke();
                        canShoot = false;
                    }
                break;

                case FireMode.AutoShotgunBurst:
                    if (canShoot)
                    {
                        for (int i = 0; i < burstBulletsToFire; i++)
                        {
                            float randRotationass = Random.Range(accuracy.x, accuracy.y) + 90f;
                            Quaternion bulletSpreadass = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotationass));
                            GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpreadass);
                            tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                            tmpProj.GetComponent<Projectile>().parentFlank = this;
                            SetProjectileSender(tmpProj);
                            currentClip--;
                            OnFiredBullet.Invoke();

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
            for (int i = 0; i < burstBulletsToFire; i++)
            {
                if (currentTimeBtwSAShots <= 0f)
                {
                    float randRotation = Random.Range(accuracy.x, accuracy.y) + 90f;
                    Quaternion bulletSpread = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0f, 0f, randRotation));
                    GameObject tmpProj = Instantiate(flankProjectile.gameObject, firePoint.position, bulletSpread);
                    tmpProj.GetComponent<Projectile>().firedFrom = firePoint.position;
                    tmpProj.GetComponent<Projectile>().parentFlank = this;
                    SetProjectileSender(tmpProj);
                    animator.SetTrigger("shoot");
                    currentClip--;
                    currentAutoClipped++;
                    OnFiredBullet.Invoke();

                    if (currentClip <= 0)
                    {
                        currentAutoClipped = 0;
                        multipleBullets = false;
                        reloading = true;
                        break;
                    }

                    else if (currentAutoClipped >= burstBulletsToFire)
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
        originalAccuracy = accuracy;
        focusedAccuracy.x = accuracy.x / 2f;
        focusedAccuracy.y = accuracy.y / 2f;
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void SetWeaponStats()
    {
        string _defaultFireMode = weaponStat.defaultFireMode.ToString();

        switch (weaponStat.weaponType)
        {
            case WeaponStat.WeaponType.Pistol:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.AssaultRifle: case WeaponStat.WeaponType.SubmachineGun:
                if (weaponStat.defaultFireMode == WeaponStat.FireMode.SemiAuto)
                    currentFireMode = FireMode.SemiAuto;
                
                else if (weaponStat.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.Shotgun:
                if (weaponStat.defaultFireMode == WeaponStat.FireMode.SingleShot)
                    currentFireMode = FireMode.ShotgunBurst;
                
                else if (weaponStat.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.AutoShotgunBurst;
            break;

            case WeaponStat.WeaponType.SniperRifle:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.LightMachineGun:
                currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.Special:
                if (weaponStat.defaultFireMode == WeaponStat.FireMode.SingleShot)
                    currentFireMode = FireMode.SingleShot;
                
                else if (weaponStat.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.FullAuto;
            break;
        }

        clipSize = weaponStat._clipSize;
        magSize = weaponStat._magSize;
        timeBtwShots = weaponStat._timeBtwShots;
        reloadTime = weaponStat._reloadTime;
        accuracy = weaponStat.accuracy;
        burstBulletsToFire = weaponStat._burstBulletsToFire;
        ammoPickupChance = weaponStat._ammoPickupChance;
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
                OnReloadWeapon.Invoke();
                currentReloadTime = 0;
            }

            else
                currentReloadTime += Time.deltaTime;
        }
    }
}
