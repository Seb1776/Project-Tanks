using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnGroup : MonoBehaviour
{
    public List<Enemy> enemy = new List<Enemy>();
    public List<RequirementForSpawn> spawnGroupInfo = new List<RequirementForSpawn>();
    public int minimumRequiredDifficultyToSpawn;

    void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    public int GetRequiredIndexByName(string enemyName)
    {
        for (int i = 0; i < spawnGroupInfo.Count; i++)
        {
            if (enemyName == spawnGroupInfo[i].enemyTypeInGroup.ToString())
                return i;
        }

        return -1;
    }
}

[System.Serializable]
public class RequirementForSpawn
{
    public enum EnemyType{Normals, Shield, Sniper, Tazer, Medic, Pyro, Delta, Smoker, Kamikz, BulldozerA, BulldozerB, BulldozerC}
    public EnemyType enemyTypeInGroup;
    public int expectedMinimumAmount;
}
