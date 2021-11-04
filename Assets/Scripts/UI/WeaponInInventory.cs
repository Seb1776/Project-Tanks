using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInInventory : MonoBehaviour
{
    public Text weaponName;
    public Image weaponImage;
    public WeaponStat weaponData;
    public UIManager uiManager;

    public void SetWeaponData(WeaponStat _stat)
    {
        weaponName.text = _stat._weaponSettingName;
        weaponImage.sprite = _stat.weaponImage;
        weaponData = _stat;
    }
}
