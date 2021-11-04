using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public Player player;
    public enum GameState {Playing, Tabbed}
    public GameState currentGameState;
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    public float flashBangDuration;
    public int money;
    public DifficultySettings currentDifficulty;
    public DifficultySettings[] allDifficulty;
    public int currentDifficultyIndex;
    public Room playerIsInRoom;
    public int requiredAssaultsToEndFloor;
    public int currentCompletedAssaults;

    [Header("UI")]
    public Image flashbangPanel;
    public Animator assaultBannerAnim;
    public Animator startPanelAnim;
    public WeaponHUD[] weaponHUD;
    public Slider healthSlider;
    public Slider dodgeSlider;
    public Slider absorptionSlider;
    public Text roomMechanic;
    public Text difficultyText;
    public Image[] starsDifficulty;
    public Text startFloorLevel;
    public Text startDifficultyText;
    public Text startFloorName;
    public Text moneyText;
    public GameObject gameOverScreen;

    public bool flashbang;

    bool gameLoaded;
    bool gameOver;

    AbilityManager am;
    MusicSystem ms;
    MapGenerator mg;
    SpawnSystem ss;

    void Start()
    {
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ms = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();
        mg = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>();
        ss = GameObject.FindGameObjectWithTag("SpawnSystem").GetComponent<SpawnSystem>();

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

        for (int i = 0; i < currentDifficulty.UIStars.Length; i++)
        {
            switch (currentDifficulty.UIStars[i].starBlip)
            {
                case Star.StarMode.Dark:
                    starsDifficulty[i].GetComponent<Animator>().SetTrigger("dark");
                break;

                case Star.StarMode.Light:
                    starsDifficulty[i].GetComponent<Animator>().SetTrigger("bright");
                break;
            }
        }

        startFloorLevel.text = "FLOOR " + currentDifficulty.floorLevel.ToString();
        startFloorName.text = "/// " + currentDifficulty.floorName + " ///";
        startDifficultyText.text = currentDifficulty.difficultyName.ToUpper();

        difficultyText.text = currentDifficulty.difficultyName.ToUpper();
        dodgeSlider.maxValue = 100f;
        absorptionSlider.maxValue = 100f;
    }

    void Update()
    {   
        if (gameLoaded)
        {
            FlashbangGrenade();
            UI();

            if (Input.GetKeyDown(KeyCode.Y))
                TriggerFlashbang(0.8f);
            
            if (gameOver)
                GameOver();
        }
    }

    public void TriggerGameOver()
    {
        ss.canSpawn = false;
        gameOverScreen.SetActive(true);

        foreach (Enemy e in spawnedEnemies)
            Destroy(e.gameObject);

        Destroy(player.gameObject);

        gameOver = true;
    }

    void GameOver()
    {
        if (ms.source.pitch > 0f)
            ms.source.pitch -= Time.deltaTime / 6f;
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
        
        moneyText.text = "$ " + money.ToString("#,#");
    }

    public void EndFloor()
    {
        currentDifficultyIndex++;
        currentDifficulty = allDifficulty[currentDifficultyIndex];
        startFloorLevel.text = "FLOOR " + currentDifficulty.floorLevel.ToString();
        startFloorName.text = "/// " + currentDifficulty.floorName + " ///";
        startDifficultyText.text = currentDifficulty.difficultyName.ToUpper();

        for (int i = 0; i < currentDifficulty.UIStars.Length; i++)
        {
            switch (currentDifficulty.UIStars[i].starBlip)
            {
                case Star.StarMode.Dark:
                    starsDifficulty[i].GetComponent<Animator>().SetTrigger("dark");
                break;

                case Star.StarMode.Light:
                    starsDifficulty[i].GetComponent<Animator>().SetTrigger("bright");
                break;
            }
        }

        ms.source.Stop();
        startPanelAnim.gameObject.SetActive(true);
        startPanelAnim.SetTrigger("reload");
        StartCoroutine(LoadNewMap());
    }

    IEnumerator DeactivateStartPanel()
    {
        yield return new WaitForSeconds(3.5f);
        startPanelAnim.gameObject.SetActive(false);
    }

    IEnumerator LoadNewMap()
    {
        yield return new WaitForSeconds(.5f);
        mg.ReGenerateMap();

        ms.doneLoadingMusic = false;
        gameLoaded = false;

        LoadMusic();
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

    public void LoadMusic()
    {
        StartCoroutine(WaitForGameLoad());
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

    IEnumerator WaitForGameLoad()
    {
        yield return new WaitForSeconds(1f);

        ms.GetMusic();

        if (ms.doneLoadingMusic)
        {
            ms.ChangePhase(MusicSystem.Phase.Control, true);
            gameLoaded = true;
        }

        while (!gameLoaded)
        {
            yield return null;
        }

        StartCoroutine(DeactivateStartPanel());
        startPanelAnim.SetTrigger("start");
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
            yield return null;
    }
}
