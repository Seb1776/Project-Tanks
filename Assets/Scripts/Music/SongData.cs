using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Soundtrack Data", menuName = "Soundtrack Data")]
public class SongData : ScriptableObject
{
    public string songID;
    public string songName;
    public AudioClip controlStart;
    public AudioClip control;
    public AudioClip anticipationStart;
    public AudioClip anticipation;
    public AudioClip assaultStart;
    public AudioClip assault;
    public SongEffects[] songEffects;
}

[System.Serializable]
public class SongEffects
{
    public enum Effect {Pulse, PoliceAlarm}
    public Effect effect;
    public Vector2 startEndDuration;
    public float timeBtwPulses;
    public Color[] possibleColors;
    public enum ColorPulseOrder {Ordered, Random}
    public ColorPulseOrder colorPulseOrder;
}
