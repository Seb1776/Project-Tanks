using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public bool canSpawn;
    public Vector2 randomTimeBtwSpawns;
    [Range(0.1f, 2f)]
    public float spawnTimeMultiplier;
    public List<EnemyLimitSetter> mapLimits = new List<EnemyLimitSetter>();
    public List<CurrentEnemyLimitSetter> currentMapLimits = new List<CurrentEnemyLimitSetter>();
    public List<EnemySpawnGroup> allEnemyStrats = new List<EnemySpawnGroup>();
    public List<Transform> spawnpoints = new List<Transform>();
    public Room currentRoom;

    float currentMaxTimeBtwSpawns;
    float currentTimBtwSpawns;
    MusicSystem music;
    [SerializeField] List<EnemySpawnGroup> availableEnemyGroups = new List<EnemySpawnGroup>();
    [SerializeField] List<EnemySpawnGroup> unavailableEnemyGroups = new List<EnemySpawnGroup>();

    void Start()
    {
        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();

        CheckForSpawns();
        SetRandomSpawnTime();
    }

    void Update()
    {   
        if (canSpawn)
        {
            SpawnBehaviour();
        }
    }

    void SpawnBehaviour()
    {
        if (currentTimBtwSpawns >= currentMaxTimeBtwSpawns)
        {
            int randSpawnGroup = Random.Range(0, availableEnemyGroups.Count);
            int randSpawnPoint = Random.Range(0, spawnpoints.Count);
            List<string> enemiesToSpawnNames = new List<string>();
            List<int> enemiesToSpawnAmount = new List<int>();

            for (int i = 0; i < availableEnemyGroups[randSpawnGroup].spawnGroupInfo.Count; i++)
            {
                enemiesToSpawnNames.Add(availableEnemyGroups[randSpawnGroup].spawnGroupInfo[i].enemyTypeInGroup.ToString());
                enemiesToSpawnAmount.Add(availableEnemyGroups[randSpawnGroup].spawnGroupInfo[i].expectedMinimumAmount);
            }
            
            if (enemiesToSpawnNames.Count > 0 && enemiesToSpawnAmount.Count > 0)
            {
                UpdateMapLimits(enemiesToSpawnNames, enemiesToSpawnAmount, true);
                Instantiate(availableEnemyGroups[randSpawnGroup].gameObject, spawnpoints[randSpawnPoint].position, Quaternion.identity);
            }

            else
                Debug.LogError("Couldn't spawn, empty group.");
            
            currentTimBtwSpawns = 0f;
            SetRandomSpawnTime();
        }

        else
        {
            currentTimBtwSpawns += Time.deltaTime * spawnTimeMultiplier;
        }

        if (music.currentMusicPhase == MusicSystem.MusicPhase.Control)
            spawnTimeMultiplier = 0.25f;
        
        else if (music.currentMusicPhase == MusicSystem.MusicPhase.Anticipation)
            spawnTimeMultiplier = 0.5f;
        
        else spawnTimeMultiplier = 1f;
    }

    void CheckForSpawns()
    {
        for (int i = 0; i < allEnemyStrats.Count; i++)
        {
            if (CheckSpawnGroup(allEnemyStrats[i]))
            {
                if (!availableEnemyGroups.Contains(allEnemyStrats[i]))
                {
                    availableEnemyGroups.Add(allEnemyStrats[i]);
                    Debug.Log(allEnemyStrats[i].gameObject.name + " can spawn.");
                }
            }
            
            else
            {   
                if (!unavailableEnemyGroups.Contains(allEnemyStrats[i]))
                {
                    unavailableEnemyGroups.Add(allEnemyStrats[i]);
                    Debug.Log(allEnemyStrats[i].gameObject.name + " cant spawn.");
                }
            }
        }
    }

    void SetRandomSpawnTime()
    {
        if (randomTimeBtwSpawns.x > randomTimeBtwSpawns.y)
        {
            float tmpX = randomTimeBtwSpawns.x;
            randomTimeBtwSpawns.x = randomTimeBtwSpawns.y;
            randomTimeBtwSpawns.y = tmpX;
        }

        currentMaxTimeBtwSpawns = Random.Range(randomTimeBtwSpawns.x, randomTimeBtwSpawns.y);
    }

    bool CheckSpawnGroup(EnemySpawnGroup _spawnInfo)
    {
        bool[] enemyCheckers = new bool[_spawnInfo.spawnGroupInfo.Count];

        string currentEnemyCheck = "";
        int currentLimitCheck = 0;
        int currentRequiredCheck = 0;
        int expectedCurrentLimitIfSpawn = 0;
        int boolCountCheck = 0;

        for (int i = 0; i < _spawnInfo.spawnGroupInfo.Count; i++)
        {
            currentEnemyCheck = _spawnInfo.spawnGroupInfo[i].enemyTypeInGroup.ToString();
            currentLimitCheck = GetLimitIndexByName(currentEnemyCheck);
            currentRequiredCheck = _spawnInfo.GetRequiredIndexByName(currentEnemyCheck);
            expectedCurrentLimitIfSpawn = _spawnInfo.spawnGroupInfo[currentRequiredCheck].expectedMinimumAmount + currentMapLimits[currentLimitCheck].currentLimit;

            if (expectedCurrentLimitIfSpawn < mapLimits[currentLimitCheck].limit)
                enemyCheckers[i] = true;

            else
                enemyCheckers[i] = false;
        }

        for (int i = 0; i < enemyCheckers.Length; i++)
        {
            if (enemyCheckers[i])
                boolCountCheck++;
            
            if (boolCountCheck >= enemyCheckers.Length)
                return true;
        }

        return false;
    }

    public void UpdateMapLimits(List<string> enemiesToAffect, List<int> amountToSpawn, bool mode)
    {
        for (int i = 0; i < enemiesToAffect.Count; i++)
        {
            int enemyIndex = GetLimitIndexByName(enemiesToAffect[i]);
            
            if (mode)
                currentMapLimits[enemyIndex].currentLimit += amountToSpawn[i];
            else
            {
                currentMapLimits[enemyIndex].currentLimit -= amountToSpawn[i];

                if (currentMapLimits[enemyIndex].currentLimit < 0)
                    currentMapLimits[enemyIndex].currentLimit = 0;
            }
        }
    }

    public int GetLimitIndexByName(string enemyName)
    {   
        switch (enemyName)
        {
            case "Normals":
                return 0;
            
            case "Shield":
                return 1;

            case "Sniper":
                return 2;
            
            case "Tazer":
                return 3;
            
            case "Medic":
                return 4;
            
            case "Pyro":
                return 5;
            
            case "Delta":
                return 6;
            
            case "Smoker":
                return 7;
            
            case "Kamikz":
                return 8;
            
            case "BulldozerA":
                return 9;
            
            case "BulldozerB":
                return 10;
            
            case "BulldozerC":
                return 11;
        }

        return -1;
    }
}

[System.Serializable]
public class EnemyLimitSetter
{
    public enum EnemyType{Normals, Shield, Sniper, Tazer, Medic, Pyro, Delta, Smoker, Kamikz, BulldozerA, BulldozerB, BulldozerC}
    public EnemyType enemyIdentifier;
    public int limit;
}

[System.Serializable]
public class CurrentEnemyLimitSetter
{
    public enum EnemyType{Normals, Shield, Sniper, Tazer, Medic, Pyro, Delta, Smoker, Kamikz, BulldozerA, BulldozerB, BulldozerC}
    public EnemyType enemyIdentifier;
    public int currentLimit;
}
