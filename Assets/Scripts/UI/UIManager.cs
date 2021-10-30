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

    public List<WeaponShopContainer> 
    shopPistols = new List<WeaponShopContainer>(), 
    shopARs = new List<WeaponShopContainer>(), 
    shopSMGs = new List<WeaponShopContainer>(), 
    shopShotguns = new List<WeaponShopContainer>(), 
    shopLMGs = new List<WeaponShopContainer>(), 
    shopSnipers = new List<WeaponShopContainer>(), 
    shopSpecials = new List<WeaponShopContainer>();

    public List<GameObject> shopContainers = new List<GameObject>();

    public Image weaponImage;
    public Text wpnText, magText, clipText, dmgText, accText, pckText;
    public GameObject buyFlankPanel;
    public GameObject weaponUpgradesButtons;

    GameManager manager;
    Flank currentlySelectedFlank;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
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
    }

    public void ChangeShopSection(string goTo)
    {
        switch (goTo)
        {
            case "pistol":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[0].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopPistols.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;

            case "ar":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[1].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopARs.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;

            case "smg":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[2].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopSMGs.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;

            case "shotgun":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[3].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopShotguns.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;

            case "lmg":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[4].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopLMGs.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;

            case "sniper":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[5].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopSnipers.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;

            case "special":
                foreach (GameObject g in shopContainers)
                    g.SetActive(false);
                
                shopContainers[6].SetActive(true);

                for (int i = 0; i < _playerFlanks.Count; i++)
                {
                    if (_playerFlanks[i].flankWeaponInventory.Count > 0)
                    {
                        for (int j = 0; j < shopSpecials.Count; j++)
                        {
                            if (_playerFlanks[i].flankWeaponInventory.Contains(shopPistols[j].respectiveWeapon))
                                shopPistols[j].GetComponent<Button>().interactable = false;
                        }
                    }

                    else
                        break;
                }
            break;
        }   
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
                    currentlySelectedFlank = _playerFlanks[1];
                }

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[1].enabled = true;
            break;

            case "L45":
                if (_playerFlanks[2].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[2].weaponStat;
                    currentlySelectedFlank = _playerFlanks[2];
                }

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[2].enabled = true;
            break;

            case "R180":
                if (_playerFlanks[3].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[3].weaponStat;
                    currentlySelectedFlank = _playerFlanks[3];
                }

                foreach (Outline o in uiFlankOutlines)
                    o.enabled = false;

                uiFlankOutlines[3].enabled = true;
            break;

            case "L180":
                if (_playerFlanks[4].gameObject.activeSelf)
                {
                    tmpStat = _playerFlanks[4].weaponStat;
                    currentlySelectedFlank = _playerFlanks[4];
                }

                foreach (Outline o in uiFlankOutlines)
                        o.enabled = false;

                uiFlankOutlines[4].enabled = true;
            break;
        }

        buyFlankPanel.SetActive(tmpStat == null);
        weaponUpgradesButtons.SetActive(tmpStat != null);

        if(tmpStat != null)
        {
            weaponImage.sprite = tmpStat.weaponImage;
            wpnText.text = tmpStat._weaponSettingName;
            magText.text = tmpStat._magSize.ToString();
            clipText.text = tmpStat._clipSize.ToString();
            dmgText.text = tmpStat._damage.ToString();
            accText.text = (12 - (int)tmpStat.accuracy.x).ToString();
            pckText.text = tmpStat._ammoPickupChance.x.ToString() + " - " + tmpStat._ammoPickupChance.y.ToString();
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
