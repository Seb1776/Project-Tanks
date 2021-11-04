using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Difficulty Level")]
public class DifficultySettings : ScriptableObject
{
    public string difficultyID;
    public string difficultyName;
    public int difficultyValue;
    public float moneyMultiplier;
    public float damageMultiplier;
    public int floorLevel;
    public string floorName;
    public Vector2 timeBtwSpawns;
    public Vector2 assaultDuration;
    public Vector2 enemiesToKill;
    public int roomsToGenerate;
    public int assaultRooms;
    public int conditionalRooms;
    public int protectRooms;
    public int deathSentenceRooms;
    public Star[] UIStars;
    public SongData difficultySong;
    public EnemyMapLimits[] enemyMapLimits;
}

[System.Serializable]
public class EnemyMapLimits
{
    public Room respectiveRoom;
    public EnemyList[] allEnemy;
}

[System.Serializable]
public class EnemyList
{
    public enum EnemyType{Normals, Shield, Sniper, Tazer, Medic, Pyro, Delta, Smoker, Kamikz, BulldozerA, BulldozerB, BulldozerC}
    public EnemyType enemyIdentifier;
    public int limit;
    public int money;
}

[System.Serializable]
public class Star
{
    public enum StarMode {Blip, Light, Dark}
    public StarMode starBlip;
}
