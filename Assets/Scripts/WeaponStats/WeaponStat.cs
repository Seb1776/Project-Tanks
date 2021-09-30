using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapons Stat", menuName = "Weapon Stat")]
public class WeaponStat : ScriptableObject
{
    public enum WeaponType {SingleShot, SemiAuto, ShotgunBurst, FullAuto, AutoShotgunBurst}
    public WeaponType weaponType;
    public enum AvailableProjectiles {Normal, Electric, Fire, Explosive, Toxic, Piercing}
    public List<AvailableProjectiles> capableOfShooting = new List<AvailableProjectiles>();
    public string _weaponSettingName;
    public int _clipSize;
    public int _magSize;
    public int _damage;
    public float _timeBtwShots;
    public float _reloadTime;
    public Vector2 _normalSpread;
    public Vector2 _shotgunSpread;
    public int _shotgunSpreadBullets;
    public int _semiAutoBullets;
    public Vector2 _ammoPickupChance;
    [Tooltip ("X is distance, Y is the multiplier")]
    public List<Vector2> _optimalDamageRangeMultiplier = new List<Vector2>();
}
