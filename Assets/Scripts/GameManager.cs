using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   
    public Player player;
    public enum GameState {Playing, Tabbed}
    public GameState currentGameState;
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    public Projectile projectile;
    public Projectile enemyProjectile;
    public Transform projectileSpawn;
    public Transform enemyProjectileSpawn;
    public float flashBangDuration;

    [Header("UI")]
    public Image flashbangPanel;
    public Animator assaultBannerAnim;
    public WeaponHUD[] weaponHUD;
    public Slider healthSlider;
    public Slider dodgeSlider;
    public Slider absorptionSlider;

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

        dodgeSlider.maxValue = 100f;
        absorptionSlider.maxValue = 100f;
    }

    void Update()
    {
        FlashbangGrenade();
        UI();

        if (Input.GetKeyDown(KeyCode.Y))
            TriggerFlashbang(0.8f);
    }

    void UI()
    {
        for (int i = 0; i < weaponHUD.Length; i++)
        {
            if (weaponHUD[i].gameObject.activeSelf)
            {
                weaponHUD[i].clipSlider.value = Mathf.Lerp(weaponHUD[i].clipSlider.value, player.playerFlanks[i].currentClip, 2.5f * Time.deltaTime);
                weaponHUD[i].magSlider.value = Mathf.Lerp(weaponHUD[i].magSlider.value, player.playerFlanks[i].currentMag, 2.5f * Time.deltaTime);
                weaponHUD[i].clipAmount.text = player.playerFlanks[i].currentClip.ToString();
                weaponHUD[i].magAmount.text = player.playerFlanks[i].currentMag.ToString();

                if (player.playerFlanks[i].reloading)
                    weaponHUD[i].weaponImage[1].fillAmount = (player.playerFlanks[i].currentReloadTime / player.playerFlanks[i].reloadTime);

                else
                    weaponHUD[i].weaponImage[1].fillAmount = 1f;
            }
        }

        healthSlider.maxValue = player.startingHealth;

        if (player.currentHealth != healthSlider.value)
            healthSlider.value = Mathf.Lerp(healthSlider.value, player.currentHealth, 2.5f * Time.deltaTime);

        if (player.dodgeChance != dodgeSlider.value)
            dodgeSlider.value = Mathf.Lerp(dodgeSlider.value, player.dodgeChance, 2.5f * Time.deltaTime);

        if (player.absorptionPercentage != absorptionSlider.value)
            absorptionSlider.value = Mathf.Lerp(absorptionSlider.value, player.absorptionPercentage, 2.5f * Time.deltaTime);
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

    public void TriggerAssault(bool mode)
    {
        assaultBannerAnim.SetBool("show", mode);
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

    public void AddAmmo()
    {
        foreach (Flank f in player.playerFlanks)
            f.AddAmmo(f.magSize);
    }

    public void HealBag()
    {
        player.Heal(player.startingHealth);
    }
}
