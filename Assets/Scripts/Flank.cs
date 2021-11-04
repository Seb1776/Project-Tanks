using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flank : MonoBehaviour
{
    public bool boughtWeapon;
    public WeaponStat weaponStat;
    public List<WeaponInventoryContainer> flankWeaponInventory = new List<WeaponInventoryContainer>();
    public enum FireMode {SingleShot, SemiAuto, ShotgunBurst, FullAuto, AutoShotgunBurst}
    public FireMode currentFireMode;
    public Projectile flankProjectile;
    public Transform firePoint;
    public string flankPosition;
    public int unlockPrice;
    public string weaponName;
    public Vector2 accuracy;
    public Vector2 focusedAccuracy;
    public Vector2 originalAccuracy;
    public List<Vector2> optimalDamageRangeMultiplier = new List<Vector2>();
    public int clipSize;
    public int magSize;
    public int damage;
    public int currentClip;
    public int currentMag;
    public float timeBtwShots;
    public float reloadTime;
    public float originalReloadTime;
    public bool reloading;
    public bool canShoot;
    public bool consumeMag;
    public bool useWeaponDefaultProjectile;
    public UnityEvent OnFiredBullet;
    public UnityEvent OnReloadWeapon;
    public bool multipleBullets;
    public int burstBulletsToFire;
    public float timeBtwSAShots;
    public Vector2 ammoPickupChance;
    public LivingThing entity;
    public AudioClip shootSFX;

    int currentAutoClipped;
    float currentTimeBtwSAShots;
    float currentTimeBtwShots;
    public float currentReloadTime;
    Animator animator;
    AudioSource source;
    GameManager manager;
    UIManager uiManager;

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

    public void SetStarterVariables()
    {
        originalReloadTime = reloadTime;
        currentClip = clipSize;
        currentMag = magSize;
        originalAccuracy = accuracy;
        focusedAccuracy.x = accuracy.x / 2f;
        focusedAccuracy.y = accuracy.y / 2f;

        int recentlyAdded = 0;
        WeaponInventoryContainer _set = new WeaponInventoryContainer();
        flankWeaponInventory.Add(_set);
        recentlyAdded = flankWeaponInventory.IndexOf(_set);
        flankWeaponInventory[recentlyAdded].SetContainerDataFromFlank(this);

        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        if (entity.GetComponent<Player>() != null)
        {
            int flankIndex = entity.GetComponent<Player>().playerFlanks.IndexOf(this);
            manager.weaponHUD[flankIndex].gameObject.SetActive(true);
            manager.weaponHUD[flankIndex].weaponImage[0].sprite = weaponStat.weaponImage;
            manager.weaponHUD[flankIndex].weaponImage[1].sprite = weaponStat.weaponImage;
            manager.weaponHUD[flankIndex].clipSlider.maxValue = weaponStat._clipSize;
            manager.weaponHUD[flankIndex].magSlider.maxValue = weaponStat._magSize;
            manager.weaponHUD[flankIndex].clipAmount.text = currentClip.ToString();
            manager.weaponHUD[flankIndex].magAmount.text = currentMag.ToString();
        }
    }

    public void AddWeaponToInventory(WeaponStat statToSet)
    {
        int recentlyAdded = 0;
        WeaponInventoryContainer _set = new WeaponInventoryContainer();
        flankWeaponInventory.Add(_set);
        recentlyAdded = flankWeaponInventory.IndexOf(_set);
        flankWeaponInventory[recentlyAdded].SetContainerDataFromData(statToSet);
    }

    public void SetWeaponStats()
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

        if (useWeaponDefaultProjectile)
            flankProjectile = weaponStat.defaultProjectile;

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

    public void ReplaceInventoryWeaponData(int weaponToSetIndex)
    {
        int index = 0;

        for (int i = 0; i < flankWeaponInventory.Count; i++)
        {
            if (weaponStat == flankWeaponInventory[i].dataWeapon)
            {
                index = i;
                break;
            }
        }

        if (flankWeaponInventory[index] != null)
        {
            switch (currentFireMode)
            {
                case Flank.FireMode.FullAuto:
                    flankWeaponInventory[index].currentFireMode = WeaponInventoryContainer.FireMode.FullAuto;
                break;

                case Flank.FireMode.SemiAuto:
                    flankWeaponInventory[index].currentFireMode = WeaponInventoryContainer.FireMode.SemiAuto;
                break;

                case Flank.FireMode.SingleShot:
                    flankWeaponInventory[index].currentFireMode = WeaponInventoryContainer.FireMode.SingleShot;
                break;

                case Flank.FireMode.AutoShotgunBurst:
                    flankWeaponInventory[index].currentFireMode = WeaponInventoryContainer.FireMode.AutoShotgunBurst;
                break;

                case Flank.FireMode.ShotgunBurst:
                    flankWeaponInventory[index].currentFireMode = WeaponInventoryContainer.FireMode.ShotgunBurst;
                break;
            }

            flankWeaponInventory[index].flankProjectile = flankProjectile;
            flankWeaponInventory[index].weaponName = weaponName;
            flankWeaponInventory[index].accuracy = accuracy;
            flankWeaponInventory[index].focusedAccuracy = focusedAccuracy;
            flankWeaponInventory[index].originalAccuracy = originalAccuracy;
            flankWeaponInventory[index].optimalDamageRangeMultiplier = optimalDamageRangeMultiplier;
            flankWeaponInventory[index].damage = damage;
            flankWeaponInventory[index].currentClip = currentClip;
            flankWeaponInventory[index].currentMag = currentMag;
            flankWeaponInventory[index].timeBtwShots = timeBtwShots;
            flankWeaponInventory[index].burstBulletsToFire = burstBulletsToFire;
            flankWeaponInventory[index].timeBtwSAShots = timeBtwSAShots;
        }

        if (flankWeaponInventory[weaponToSetIndex] != null)
        {
            switch (flankWeaponInventory[weaponToSetIndex].currentFireMode)
            {
                case WeaponInventoryContainer.FireMode.FullAuto:
                    currentFireMode = FireMode.FullAuto;
                break;

                case WeaponInventoryContainer.FireMode.SemiAuto:
                    currentFireMode = FireMode.SemiAuto;
                break;

                case WeaponInventoryContainer.FireMode.SingleShot:
                    currentFireMode = FireMode.SingleShot;
                break;

                case WeaponInventoryContainer.FireMode.AutoShotgunBurst:
                    currentFireMode = FireMode.AutoShotgunBurst;
                break;

                case WeaponInventoryContainer.FireMode.ShotgunBurst:
                    currentFireMode = FireMode.ShotgunBurst;
                break;
            }

            weaponStat = flankWeaponInventory[weaponToSetIndex].dataWeapon;
            flankProjectile = flankWeaponInventory[weaponToSetIndex].flankProjectile;
            weaponName = flankWeaponInventory[weaponToSetIndex].weaponName;
            accuracy = flankWeaponInventory[weaponToSetIndex].accuracy;
            focusedAccuracy = flankWeaponInventory[weaponToSetIndex].focusedAccuracy;
            originalAccuracy = accuracy;
            optimalDamageRangeMultiplier = flankWeaponInventory[weaponToSetIndex].optimalDamageRangeMultiplier;
            damage = flankWeaponInventory[weaponToSetIndex].damage;
            currentClip = flankWeaponInventory[weaponToSetIndex].currentClip;
            currentMag = flankWeaponInventory[weaponToSetIndex].currentMag;
            clipSize = flankWeaponInventory[weaponToSetIndex].dataWeapon._clipSize;
            magSize = flankWeaponInventory[weaponToSetIndex].dataWeapon._magSize;
            currentTimeBtwSAShots = currentTimeBtwShots = 0f;
            timeBtwSAShots = flankWeaponInventory[weaponToSetIndex].timeBtwSAShots;
            timeBtwShots = flankWeaponInventory[weaponToSetIndex].timeBtwShots;
            burstBulletsToFire = flankWeaponInventory[weaponToSetIndex].burstBulletsToFire;
            currentAutoClipped = 0;
            int flankIndex = entity.GetComponent<Player>().playerFlanks.IndexOf(this);
            manager.weaponHUD[flankIndex].weaponImage[0].sprite = flankWeaponInventory[weaponToSetIndex].dataWeapon.weaponImage;
            manager.weaponHUD[flankIndex].weaponImage[1].sprite = flankWeaponInventory[weaponToSetIndex].dataWeapon.weaponImage;
            manager.weaponHUD[flankIndex].clipSlider.maxValue = flankWeaponInventory[weaponToSetIndex].dataWeapon._clipSize;
            manager.weaponHUD[flankIndex].magSlider.maxValue = flankWeaponInventory[weaponToSetIndex].dataWeapon._magSize;
            manager.weaponHUD[flankIndex].clipAmount.text = flankWeaponInventory[weaponToSetIndex].currentClip.ToString();
            manager.weaponHUD[flankIndex].magAmount.text = flankWeaponInventory[weaponToSetIndex].currentMag.ToString();
            uiManager.SetWeaponInfo(flankPosition);
        }
    }
}

[System.Serializable]
public class WeaponInventoryContainer
{
    public WeaponStat dataWeapon;
    public enum FireMode {SingleShot, SemiAuto, FullAuto, ShotgunBurst, AutoShotgunBurst}
    public FireMode currentFireMode;
    public Projectile flankProjectile;
    public string weaponName;
    public Vector2 accuracy;
    public Vector2 focusedAccuracy;
    public Vector2 originalAccuracy;
    public List<Vector2> optimalDamageRangeMultiplier = new List<Vector2>();
    public int damage;
    public int currentClip;
    public int currentMag;
    public float timeBtwShots;
    public int burstBulletsToFire;
    public float timeBtwSAShots;

    public void SetContainerDataFromFlank(Flank _data)
    {
        switch (_data.weaponStat.weaponType)
        {
            case WeaponStat.WeaponType.Pistol:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.AssaultRifle: case WeaponStat.WeaponType.SubmachineGun:
                if (_data.weaponStat.defaultFireMode == WeaponStat.FireMode.SemiAuto)
                    currentFireMode = FireMode.SemiAuto;
                
                else if (_data.weaponStat.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.Shotgun:
                if (_data.weaponStat.defaultFireMode == WeaponStat.FireMode.SingleShot)
                    currentFireMode = FireMode.ShotgunBurst;
                
                else if (_data.weaponStat.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.AutoShotgunBurst;
            break;

            case WeaponStat.WeaponType.SniperRifle:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.LightMachineGun:
                currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.Special:
                if (_data.weaponStat.defaultFireMode == WeaponStat.FireMode.SingleShot)
                    currentFireMode = FireMode.SingleShot;
                
                else if (_data.weaponStat.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.FullAuto;
            break;
        }

        dataWeapon = _data.weaponStat;
        flankProjectile = _data.flankProjectile;
        weaponName = _data.weaponName;
        accuracy = _data.accuracy;
        focusedAccuracy = _data.focusedAccuracy;
        originalAccuracy = _data.originalAccuracy;
        optimalDamageRangeMultiplier = _data.optimalDamageRangeMultiplier;
        damage = _data.damage;
        currentClip = _data.currentClip;
        currentMag = _data.currentMag;
        timeBtwShots = _data.timeBtwShots;
        timeBtwSAShots = _data.timeBtwSAShots;
        burstBulletsToFire = _data.burstBulletsToFire;
    }

    public void SetContainerDataFromData(WeaponStat _data)
    {
        switch (_data.weaponType)
        {
            case WeaponStat.WeaponType.Pistol:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.AssaultRifle: case WeaponStat.WeaponType.SubmachineGun:
                if (_data.defaultFireMode == WeaponStat.FireMode.SemiAuto)
                    currentFireMode = FireMode.SemiAuto;
                
                else if (_data.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.Shotgun:
                if (_data.defaultFireMode == WeaponStat.FireMode.SingleShot)
                    currentFireMode = FireMode.ShotgunBurst;
                
                else if (_data.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.AutoShotgunBurst;
            break;

            case WeaponStat.WeaponType.SniperRifle:
                currentFireMode = FireMode.SingleShot;
            break;

            case WeaponStat.WeaponType.LightMachineGun:
                currentFireMode = FireMode.FullAuto;
            break;

            case WeaponStat.WeaponType.Special:
                if (_data.defaultFireMode == WeaponStat.FireMode.SingleShot)
                    currentFireMode = FireMode.SingleShot;
                
                else if (_data.defaultFireMode == WeaponStat.FireMode.FullAuto)
                    currentFireMode = FireMode.FullAuto;
            break;
        }

        dataWeapon = _data;
        flankProjectile = _data.defaultProjectile;
        weaponName = _data._weaponSettingName;
        accuracy = _data.accuracy;
        focusedAccuracy.x = accuracy.x / 2f;
        focusedAccuracy.y = accuracy.y / 2f;
        originalAccuracy = accuracy;
        optimalDamageRangeMultiplier = _data._optimalDamageRangeMultiplier; 
        damage = _data._damage;
        currentClip = _data._clipSize;
        currentMag = _data._magSize;
        timeBtwSAShots = _data._timeBtwShots;
        timeBtwShots = _data._timeBtwShots;
        burstBulletsToFire = _data._burstBulletsToFire;
    }
}
