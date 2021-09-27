using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    [Range(1, 3)]
    public int abilityLevels;
    [Range(0, 2)]
    public int currentAbilityLevel;
    public bool abilityOverridesDeath;

    public virtual void ApplyEffect(){}

    public virtual void Update(){}
}
