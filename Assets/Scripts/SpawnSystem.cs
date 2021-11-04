using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    float currentMaxTimeBtwSpawns;
    float currentTimeBtwSpawns;
    float assaultDuration;
    float currentAssaultDuration;
    float expectedOverdue;
    MusicSystem music;
    bool overdue;
    [SerializeField] List<EnemySpawnGroup> usableEnemyGroups = new List<EnemySpawnGroup>();
    [SerializeField] List<EnemySpawnGroup> availableEnemyGroups = new List<EnemySpawnGroup>();
    [SerializeField] List<EnemySpawnGroup> unavailableEnemyGroups = new List<EnemySpawnGroup>();
    GameManager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();

        SetRandomAssaultDuration(manager.currentDifficulty.assaultDuration);
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
        if (currentAssaultDuration < assaultDuration)
        {
            /// ASSAULT IN PROGRESS ///
            if (currentTimeBtwSpawns >= currentMaxTimeBtwSpawns)
            {
                int randSpawnGroup = Random.Range(0, availableEnemyGroups.Count);
                int randSpawnPoint = Random.Range(0, spawnpoints.Count);
                List<string> enemiesName = new List<string>();
                List<int> enemiesAmount = new List<int>();

                for (int i = 0; i < availableEnemyGroups[randSpawnGroup].spawnGroupInfo.Count; i++)
                {
                    enemiesName.Add(availableEnemyGroups[randSpawnGroup].spawnGroupInfo[i].enemyTypeInGroup.ToString());
                    enemiesAmount.Add(availableEnemyGroups[randSpawnGroup].spawnGroupInfo[i].expectedMinimumAmount);
                }

                if (enemiesAmount.Count > 0 && enemiesName.Count > 0)
                {
                    UpdateMapLimits(enemiesName, enemiesAmount, true);
                    Instantiate(availableEnemyGroups[randSpawnGroup].gameObject, spawnpoints[randSpawnPoint].position, Quaternion.identity);
                }

                currentTimeBtwSpawns = 0f;
                SetRandomSpawnTime();
                CheckForSpawns();
            }

            else
                currentTimeBtwSpawns += Time.deltaTime * spawnTimeMultiplier;

            if (music.currentMusicPhase == MusicSystem.MusicPhase.Control)
                spawnTimeMultiplier = 0.25f;
            
            else if (music.currentMusicPhase == MusicSystem.MusicPhase.Anticipation || currentAssaultDuration <= expectedOverdue)
                spawnTimeMultiplier = 0.5f;
            
            else spawnTimeMultiplier = 1f;

            currentAssaultDuration += Time.deltaTime;
        }

        else
        {   
            if (manager.spawnedEnemies.Count <= 0)
            {
                currentAssaultDuration = 0f;
                SetRandomAssaultDuration(manager.currentDifficulty.assaultDuration);
                manager.currentCompletedAssaults++;
                music.StopAssault();
            }
        }
    }

    public void CheckForSpawns()
    {
        for (int i = 0; i < usableEnemyGroups.Count; i++)
        {
            if (CheckSpawnGroup(usableEnemyGroups[i]))
            {
                if (unavailableEnemyGroups.Contains(usableEnemyGroups[i]))
                    unavailableEnemyGroups.Remove(usableEnemyGroups[i]);

                if (!availableEnemyGroups.Contains(usableEnemyGroups[i]))
                {
                    availableEnemyGroups.Add(usableEnemyGroups[i]);
                }
            }
            
            else
            {   
                if (availableEnemyGroups.Contains(usableEnemyGroups[i]))
                    availableEnemyGroups.Remove(usableEnemyGroups[i]);

                if (!unavailableEnemyGroups.Contains(usableEnemyGroups[i]))
                {
                    unavailableEnemyGroups.Add(usableEnemyGroups[i]);
                }
            }
        }
    }

    public void CheckForUsableGroups()
    {
        for (int i = 0; i < allEnemyStrats.Count; i++)
        {
            if (allEnemyStrats[i].minimumRequiredDifficultyToSpawn <= manager.currentDifficulty.difficultyValue)
                if (!usableEnemyGroups.Contains(allEnemyStrats[i]))   
                    usableEnemyGroups.Add(allEnemyStrats[i]);
        }

        CheckForSpawns();
    }

    void SetRandomSpawnTime()
    {
        if (manager.currentDifficulty.timeBtwSpawns.x > manager.currentDifficulty.timeBtwSpawns.y)
        {
            float tmpX = manager.currentDifficulty.timeBtwSpawns.x;
            manager.currentDifficulty.timeBtwSpawns.x = manager.currentDifficulty.timeBtwSpawns.y;
            manager.currentDifficulty.timeBtwSpawns.y = tmpX;
        }

        currentMaxTimeBtwSpawns = Random.Range(manager.currentDifficulty.timeBtwSpawns.x, manager.currentDifficulty.timeBtwSpawns.y);
    }

    public void SetRandomAssaultDuration(Vector2 possibleDurations)
    {
        assaultDuration = Random.Range(possibleDurations.x, possibleDurations.y);
        expectedOverdue = assaultDuration - 15f;
    }

    bool CheckSpawnGroup(EnemySpawnGroup _spawnInfo)
    {
        bool[] enemyChecks = new bool[_spawnInfo.spawnGroupInfo.Count];

        string currentEnemy = "";
        int currentLimitIdx = 0;
        int currentRequiredIdx = 0;
        int expectedCurrentLimitIfSpawn = 0;
        int boolCountCheck = 0;

        for (int i = 0; i < _spawnInfo.spawnGroupInfo.Count; i++)
        {
            currentEnemy = _spawnInfo.spawnGroupInfo[i].enemyTypeInGroup.ToString();
            currentLimitIdx = GetLimitIndexByName(currentEnemy);
            currentRequiredIdx = _spawnInfo.GetRequiredIndexByName(currentEnemy);
            expectedCurrentLimitIfSpawn = _spawnInfo.spawnGroupInfo[currentRequiredIdx].expectedMinimumAmount + currentMapLimits[currentLimitIdx].currentLimit;

            if (expectedCurrentLimitIfSpawn <= mapLimits[currentLimitIdx].limit)
                enemyChecks[i] = true;
            
            else
                enemyChecks[i] = false;

            //Debug.Log("Checking for " + currentEnemy + ", the limit is " + mapLimits[currentLimitIdx].limit + ", " + _spawnInfo.spawnGroupInfo[currentRequiredIdx].expectedMinimumAmount + " will be spawned, after spawning the new limit will be " + expectedCurrentLimitIfSpawn);
        }

        for (int i = 0; i < enemyChecks.Length; i++)
        {
            if (enemyChecks[i])
                boolCountCheck++;
            
            if (boolCountCheck >= enemyChecks.Length)
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

    public void SetMaxLimits(string enteredRoomID, List<Transform> _enemySpawns)
    {
        int roomIdx = -1;

        for (int i = 0; i < manager.currentDifficulty.enemyMapLimits.Length; i++)
        {
            if (manager.currentDifficulty.enemyMapLimits[i].respectiveRoom.roomID == enteredRoomID)
            {
                roomIdx = i;
                break;
            }
        }

        if (roomIdx != -1)
            for (int i = 0; i < mapLimits.Count; i++)
                mapLimits[i].limit = manager.currentDifficulty.enemyMapLimits[roomIdx].allEnemy[i].limit;

        spawnpoints = _enemySpawns;
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
