using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public Player player;
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    public Projectile projectile;
    public Transform projectileSpawn;

    AbilityManager am;

    void Start()
    {
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player != null)
        {
            for (int i = 0; i < am.activateOnEnemyDeathAbilities.Count; i++)
            {
                player.OnPlayerKillEnemy.AddListener(am.activateOnEnemyDeathAbilities[i].ApplyEffect);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Instantiate(projectile.gameObject, projectileSpawn.position, Quaternion.identity);
    }
}
