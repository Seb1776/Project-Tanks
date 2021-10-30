using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapons Stat", menuName = "Weapon Stat")]
public class WeaponStat : ScriptableObject
{
    public enum WeaponType {Pistol, AssaultRifle, Shotgun, SniperRifle, SubmachineGun, LightMachineGun, Special}
    public WeaponType weaponType;
    public enum FireMode {SingleShot, SemiAuto, FullAuto}
    public FireMode[] availableFireModes;
    public FireMode defaultFireMode;
    public Projectile[] capableOfShooting;
    public Projectile defaultProjectile;
    public string _weaponSettingName;
    public Sprite weaponImage;
    public int weaponPrice;
    public int _clipSize;
    public int _magSize;
    public int _damage;
    public float _timeBtwShots;
    public float _reloadTime;
    [Tooltip ("Accuracy goes from 0 to 20, 0 being perfect.")]
    public Vector2 accuracy;
    public int _burstBulletsToFire;
    public Vector2 _ammoPickupChance;
    public AudioClip _shootSFX;
    [Tooltip ("X is distance, Y is the multiplier")]
    public List<Vector2> _optimalDamageRangeMultiplier = new List<Vector2>();
}
