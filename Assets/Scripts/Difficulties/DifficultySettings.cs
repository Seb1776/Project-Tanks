using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Difficulty Level")]
public class DifficultySettings : ScriptableObject
{
    public string difficultyID;
    public string difficultyName;
}
