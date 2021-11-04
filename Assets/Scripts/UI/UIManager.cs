using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("General")]
    public GameObject canvasPanel;
    [Header("Map")]
    public UIMapRoom[] uIMapRooms;
    [Header("Inventory")]
    public List<Flank> _playerFlanks = new List<Flank>();
    public List<Outline> uiFlankOutlines = new List<Outline>();
    public List<GameObject> shopContainers = new List<GameObject>();
    public Image weaponImage;
    public Text wpnText, magText, clipText, dmgText, accText, pckText;
    public GameObject buyFlankPanel;
    public GameObject weaponUpgradesButtons;
    public WeaponStat defaultFlankWeapon;
    public Text flankPriceText;
    [Header("Store")]
    public GameObject buyPreviewObj;
    public Image wpnImagePrev;
    public Transform buyedWeaponsParent;
    public Button[] allPossibleProjectiles;
    public int[] projectilePrices;
    public string[] projectileType;
    public WeaponInInventory inventoryFrame;
    public List<WeaponInInventory> createdInventoryFrames = new List<WeaponInInventory>();

    public Text wpnTextPrev, magTextPrev, clipTextPrev, dmgTextPrev, accTextPrev, pckTextPrev, priceTextPrev
    , wpnTypeTextPrev, fireModeTextPrev, bpsTextPrev, reloadTextPrev;

    public List<WeaponShopContainer> 
    shopPistols = new List<WeaponShopContainer>(), 
    shopARs = new List<WeaponShopContainer>(), 
    shopSMGs = new List<WeaponShopContainer>(), 
    shopShotguns = new List<WeaponShopContainer>(), 
    shopLMGs = new List<WeaponShopContainer>(), 
    shopSnipers = new List<WeaponShopContainer>(), 
    shopSpecials = new List<WeaponShopContainer>();

    GameManager manager;
    MusicSystem ms;
    Flank currentlySelectedFlank;
    WeaponStat weaponToBuy;
    Player player;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ms = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && ms.currentPhase == MusicSystem.Phase.Control)
        {
            if (manager.currentGameState == GameManager.GameState.Playing)
            {
                manager.currentGameState = GameManager.GameState.Tabbed;
                canvasPanel.SetActive(true);
            }
            
            else
            {
                manager.currentGameState = GameManager.GameState.Playing;
                canvasPanel.SetActive(false);
            }
        }

        if (manager.currentGameState == GameManager.GameState.Playing && canvasPanel.activeSelf)
            canvasPanel.SetActive(false);
    }

    public void ReCheckEverySection(bool reCheck)
    {
        for (int j = 0; j < shopPistols.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopPistols[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopPistols[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopPistols[j].GetComponent<Button>().interactable = true;
            }
        }

        for (int j = 0; j < shopARs.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopARs[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopARs[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopARs[j].GetComponent<Button>().interactable = true;
            }
        }

        for (int j = 0; j < shopSMGs.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopSMGs[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopSMGs[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopSMGs[j].GetComponent<Button>().interactable = true;
            }
        }

        for (int j = 0; j < shopShotguns.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopShotguns[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopShotguns[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopShotguns[j].GetComponent<Button>().interactable = true;
            }
        }

        for (int j = 0; j < shopLMGs.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopLMGs[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopLMGs[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopLMGs[j].GetComponent<Button>().interactable = true;
            }
        }

        for (int j = 0; j < shopSnipers.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopSnipers[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopSnipers[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopSnipers[j].GetComponent<Button>().interactable = true;
            }
        }

        for (int j = 0; j < shopSpecials.Count; j++)
        {
            for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
            {
                if (shopSpecials[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                    shopSpecials[j].GetComponent<Button>().interactable = false;
                
                else
                    if (reCheck)
                        shopSpecials[j].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void ChangeShopSection(string goTo)
    {
        switch (goTo)
        {
            case "pistol":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[0].SetActive(true);
                    
                for (int j = 0; j < shopPistols.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopPistols[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopPistols[j].GetComponent<Button>().interactable = false;
                        
                        shopPistols[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopPistols[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;

            case "ar":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[1].SetActive(true);

                for (int j = 0; j < shopARs.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopARs[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopARs[j].GetComponent<Button>().interactable = false;
                        
                        shopARs[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopARs[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;

            case "smg":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[2].SetActive(true);

                for (int j = 0; j < shopSMGs.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopSMGs[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopSMGs[j].GetComponent<Button>().interactable = false;
                        
                        shopSMGs[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopSMGs[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;

            case "shotgun":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[3].SetActive(true);

                for (int j = 0; j < shopShotguns.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopShotguns[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopShotguns[j].GetComponent<Button>().interactable = false;
                        
                        shopShotguns[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopShotguns[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;

            case "lmg":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[4].SetActive(true);

                for (int j = 0; j < shopLMGs.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopLMGs[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopLMGs[j].GetComponent<Button>().interactable = false;
                        
                        shopLMGs[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopLMGs[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;

            case "sniper":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[5].SetActive(true);

                for (int j = 0; j < shopSnipers.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopSnipers[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopSnipers[j].GetComponent<Button>().interactable = false;
                        
                        shopSnipers[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopSnipers[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;

            case "special":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[6].SetActive(true);

                for (int j = 0; j < shopSpecials.Count; j++)
                {
                    for (int k = 0; k < currentlySelectedFlank.flankWeaponInventory.Count; k++)
                    {
                        if (shopSpecials[j].respectiveWeapon == currentlySelectedFlank.flankWeaponInventory[k].dataWeapon)
                            shopSpecials[j].GetComponent<Button>().interactable = false;
                        
                        shopSpecials[j].transform.GetChild(2).GetComponent<Text>().text = "$ " + shopSpecials[j].respectiveWeapon.weaponPrice.ToString();
                    }
                }
            break;
        }   
    }

    public void SetPossibleProjectiles()
    {
        List<string> _capable = new List<string>();

        for (int i = 0; i < currentlySelectedFlank.weaponStat.capableOfShooting.Length; i++)
        {
            if (currentlySelectedFlank.weaponStat.capableOfShooting[i].GetComponent<ElectricProjectile>() != null)
                _capable.Add("Electric");
            
            else if (currentlySelectedFlank.weaponStat.capableOfShooting[i].GetComponent<FireProjectile>() != null)
                _capable.Add("Fire");
            
            else if (currentlySelectedFlank.weaponStat.capableOfShooting[i].GetComponent<ExplosiveProjectile>() != null)
                _capable.Add("Explosive");
            
            else if (currentlySelectedFlank.weaponStat.capableOfShooting[i].GetComponent<PiercingProjectile>() != null)
                _capable.Add("Piercing");
            
            else if (currentlySelectedFlank.weaponStat.capableOfShooting[i].GetComponent<ToxicExplosion>() != null)
                _capable.Add("Toxic");
            
            else if (currentlySelectedFlank.weaponStat.capableOfShooting[i].GetComponent<Projectile>() != null)
                _capable.Add("Default");
        }

        if (_capable.Contains("Default"))
            allPossibleProjectiles[0].gameObject.SetActive(true);
        
        if (_capable.Contains("Electric"))
            allPossibleProjectiles[1].gameObject.SetActive(true);
        
        if (_capable.Contains("Fire"))
            allPossibleProjectiles[2].gameObject.SetActive(true);
        
        if (_capable.Contains("Explosive"))
            allPossibleProjectiles[3].gameObject.SetActive(true);
        
        if (_capable.Contains("Piercing"))
            allPossibleProjectiles[4].gameObject.SetActive(true);
        
        if (_capable.Contains("Toxic"))
            allPossibleProjectiles[5].gameObject.SetActive(true);
    }

    public void BuyProjectile(int projIndex)
    {
        if (manager.money >= projectilePrices[projIndex])
            allPossibleProjectiles[projIndex].interactable = true;
    }

    public void BuyWeapon(GameObject buyPanel)
    {
        if (manager.money >= weaponToBuy.weaponPrice)
        {
            manager.money -= weaponToBuy.weaponPrice;
            currentlySelectedFlank.AddWeaponToInventory(weaponToBuy);
            buyPanel.SetActive(false);
            weaponToBuy = null;
            ReCheckEverySection(false);
            SetInventoryFromFlank();
        }
    }

    public void SetWeaponFromInventory(WeaponInInventory inv)
    {
        int weaponToSetIndex = 0;

        for (int i = 0; i < currentlySelectedFlank.flankWeaponInventory.Count; i++)
        {
            if (currentlySelectedFlank.flankWeaponInventory[i].dataWeapon == inv.weaponData)
            {
                weaponToSetIndex = i;
                break;
            }
        }

        for (int i = 0; i < createdInventoryFrames.Count; i++)
            createdInventoryFrames[i].GetComponent<Button>().interactable = true;
        
        inv.GetComponent<Button>().interactable = false;

        currentlySelectedFlank.ReplaceInventoryWeaponData(weaponToSetIndex);
    }

    public void UnlockNewFlank()
    {
        if (manager.money >= currentlySelectedFlank.unlockPrice)
        {
            manager.money -= currentlySelectedFlank.unlockPrice;
            currentlySelectedFlank.boughtWeapon = true;
            currentlySelectedFlank.weaponStat = defaultFlankWeapon;
            currentlySelectedFlank.gameObject.SetActive(true);
            currentlySelectedFlank.SetWeaponStats();
            currentlySelectedFlank.SetStarterVariables();
            SetWeaponInfo(currentlySelectedFlank.flankPosition);
        }
    }
    
    public void SetInventoryFromFlank()
    {
        if (createdInventoryFrames.Count > 0)
        {
            for (int i = 0; i < createdInventoryFrames.Count; i++)
                Destroy(createdInventoryFrames[i].gameObject);

            createdInventoryFrames.Clear();
        }

        for (int i = 0; i < currentlySelectedFlank.flankWeaponInventory.Count; i++)
        {
            WeaponInInventory tmpInv = Instantiate(inventoryFrame, buyedWeaponsParent.position, buyedWeaponsParent.rotation, buyedWeaponsParent);
            tmpInv.SetWeaponData(currentlySelectedFlank.flankWeaponInventory[i].dataWeapon);
            tmpInv.GetComponent<Button>().onClick.AddListener(delegate{SetWeaponFromInventory(tmpInv);});
            tmpInv.uiManager = this;
            createdInventoryFrames.Add(tmpInv);

            if (tmpInv.weaponData == currentlySelectedFlank.weaponStat)
                tmpInv.GetComponent<Button>().interactable = false;
        }
    }

    public void ShowPreviewBuy(WeaponShopContainer container)
    {
        weaponToBuy = container.respectiveWeapon;
        wpnImagePrev.sprite = container.respectiveWeapon.weaponImage;
        wpnTextPrev.text = container.respectiveWeapon._weaponSettingName.ToUpper();
        magTextPrev.text = container.respectiveWeapon._magSize.ToString();
        clipTextPrev.text = container.respectiveWeapon._clipSize.ToString();
        dmgTextPrev.text = container.respectiveWeapon._damage.ToString();
        accTextPrev.text = (20 - (int)container.respectiveWeapon.accuracy.x).ToString();
        pckTextPrev.text = container.respectiveWeapon._ammoPickupChance.x.ToString() + " - " + container.respectiveWeapon._ammoPickupChance.y.ToString();
        priceTextPrev.text = "$ " + container.respectiveWeapon.weaponPrice;

        switch (container.respectiveWeapon.weaponType)
        {
            case WeaponStat.WeaponType.AssaultRifle:
                wpnTypeTextPrev.text = "ASSAULT RIFLE";
            break;

            case WeaponStat.WeaponType.LightMachineGun:
                wpnTypeTextPrev.text = "LIGHT MACHINE GUN";
            break;

            case WeaponStat.WeaponType.SubmachineGun:
                wpnTypeTextPrev.text = "SUB MACHINE GUN";
            break;

            case WeaponStat.WeaponType.SniperRifle:
                wpnTypeTextPrev.text = "SNIPER RIFLE";
            break;

            case WeaponStat.WeaponType.Shotgun:
                wpnTypeTextPrev.text = "SHOTGUN";
            break;

            case WeaponStat.WeaponType.Pistol:
                wpnTypeTextPrev.text = "PISTOL";
            break;
        }

        fireModeTextPrev.text = container.respectiveWeapon.defaultFireMode.ToString().ToUpper();
        bpsTextPrev.text = ((int)60 * container.respectiveWeapon._timeBtwShots).ToString() + " ms.";
        reloadTextPrev.text = container.respectiveWeapon._reloadTime.ToString() + " s.";

        buyPreviewObj.SetActive(true);
    }
    
    public void SetWeaponInfo(string _pos)
    {
        WeaponStat tmpStat = null;

        switch (_pos)
        {
            case "C":
                tmpStat = _playerFlanks[0].weaponStat;
                currentlySelectedFlank = _playerFlanks[0];

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[0].enabled = true;
            break;

            case "R45":
                if (_playerFlanks[1].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[1].weaponStat;
                }

                currentlySelectedFlank = _playerFlanks[1];

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[1].enabled = true;
            break;

            case "L45":
                if (_playerFlanks[2].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[2].weaponStat;
                }

                currentlySelectedFlank = _playerFlanks[2];

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[2].enabled = true;
            break;

            case "R180":
                if (_playerFlanks[3].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[3].weaponStat;
                }

                currentlySelectedFlank = _playerFlanks[3];

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[3].enabled = true;
            break;

            case "L180":
                if (_playerFlanks[4].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[4].weaponStat;
                }

                currentlySelectedFlank = _playerFlanks[4];

                foreach (Outline o in uiFlankOutlines)
                        o.enabled = false;

                uiFlankOutlines[4].enabled = true;
            break;
        }

        buyFlankPanel.SetActive(tmpStat == null);
        weaponUpgradesButtons.SetActive(tmpStat != null);

        if (tmpStat != null)
        {
            ChangeShopSection("ar");
            weaponImage.sprite = tmpStat.weaponImage;
            wpnText.text = tmpStat._weaponSettingName;
            magText.text = tmpStat._magSize.ToString();
            clipText.text = tmpStat._clipSize.ToString();
            dmgText.text = tmpStat._damage.ToString();
            accText.text = (20 - (int)tmpStat.accuracy.x).ToString();
            pckText.text = tmpStat._ammoPickupChance.x.ToString() + " - " + tmpStat._ammoPickupChance.y.ToString();
        }

        if (buyFlankPanel.activeSelf)
        {
            flankPriceText.text = "$ " + currentlySelectedFlank.unlockPrice.ToString();
        }
    }
}

[System.Serializable]
public class UIMapRoom
{
    public string roomID;
    public GameObject roomObject;
    public UIMapRoomSpawns[] uIMapRoomSpawns;
}

[System.Serializable]
public class UIMapRoomSpawns
{
    public string spawnPointsFor;
    public Transform[] spawnPoint;
}
