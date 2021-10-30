using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
    public Image[] weaponImage;
    public Slider clipSlider;
    public Text clipAmount;
    public Slider magSlider;
    public Text magAmount;

    GameManager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void SetWeaponInfo(WeaponStat _stat)
    {
        weaponImage[0].sprite = _stat.weaponImage;
        weaponImage[1].sprite = _stat.weaponImage;
        clipSlider.maxValue = _stat._clipSize;
        clipSlider.value = _stat._clipSize;
        clipAmount.text = _stat._clipSize.ToString();
        magSlider.maxValue = _stat._magSize;
        magSlider.value = _stat._magSize;
        magAmount.text = _stat._magSize.ToString();
    }

    public void ChangeClipUI(int _value)
    {
        clipSlider.value = _value;
    }

    public void ChangeMagUI(int _value)
    {
        clipSlider.value = _value;
    }
}
