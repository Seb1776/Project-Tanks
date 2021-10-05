using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   
    public Player player;
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    public Projectile projectile;
    public Projectile enemyProjectile;
    public Transform projectileSpawn;
    public Transform enemyProjectileSpawn;
    public float flashBangDuration;

    [Header("UI")]
    public Image flashbangPanel;

    public bool flashbang;

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
        FlashbangGrenade();

        if (Input.GetKeyDown(KeyCode.Y))
            TriggerFlashbang(0.8f);
    }

    public void TriggerFlashbang(float aValue)
    {
        if (!flashbang)
        {
            flashbang = true;
            flashbangPanel.gameObject.SetActive(true);

            Color tmp = flashbangPanel.color;
            tmp.a = aValue;
            flashbangPanel.color = tmp;
        }
    }

    void FlashbangGrenade()
    {
        if (flashbang && flashbangPanel.gameObject.activeSelf)
        {
            Color tmp = flashbangPanel.color;

            if (tmp.a <= 0f)
            {
                flashbangPanel.gameObject.SetActive(false);
                flashbang = false;
            }

            else
            {
                tmp.a -= Time.deltaTime * (flashBangDuration / 100f);
            }

            flashbangPanel.color = tmp;
        }
    }
}
