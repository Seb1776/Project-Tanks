using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public Player player;
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    public Projectile projectile;
    public Projectile enemyProjectile;
    public Transform projectileSpawn;
    public Transform enemyProjectileSpawn;

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

            for (int i = 0; i < am.preventsOrAffectPlayerDamageAbilities.Count; i++)
            {
                player.OnEntityTakeDamage.AddListener(am.preventsOrAffectPlayerDamageAbilities[i].ApplyEffect);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Instantiate(projectile.gameObject, projectileSpawn.position, Quaternion.identity);
        
        if (Input.GetKeyDown(KeyCode.Return))
            Instantiate(enemyProjectile.gameObject, enemyProjectileSpawn.position, Quaternion.identity);
    }
}
