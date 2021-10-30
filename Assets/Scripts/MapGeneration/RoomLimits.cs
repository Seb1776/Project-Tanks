using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Limits", menuName = "Room Limits")]
public class RoomLimits : ScriptableObject
{
    public List<EnemyLimits> limitsForThisRooms = new List<EnemyLimits>();
}

[System.Serializable]
public class EnemyLimits
{
    public enum EnemyType{Normals, Shield, Sniper, Tazer, Medic, Pyro, Delta, Smoker, Kamikz, BulldozerA, BulldozerB, BulldozerC}
    public EnemyType enemyIdentifier;
    public int limit;
}
