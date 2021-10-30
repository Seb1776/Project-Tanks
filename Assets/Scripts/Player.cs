using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cinemachine;

public class Player : LivingThing
{
    [Header ("-------------")]
    [Header ("Player Fields")]
    public float moveSpeed;
    public float runningMoveSpeed;
    public float originalRunningMoveSpeed;
    public float runSpeedTransitionSpeed;
    public float focusedMoveSpeed;
    public float originalMoveSpeed;
    public float gracePeriod;
    public bool canMove;
    public bool focusing;
    public Transform flankParent;
    public List<Flank> playerFlanks = new List<Flank>();
    public Grenade playerGrenade;
    public Transform grenadeShootPoint;
    public int grenadeCount;
    public float grenadeCooldown;
    public CinemachineVirtualCamera playerCam;
    public float zoomSpeed;
    public float zoomSize;
    [Range(2f, 4f)]
    public float speedDivisorReductorOnFocus;
    [Range(0f, 80f)]
    public float dodgeChance;
    [Range(0f, 100f)]
    public float critChance;
    [Range(0f, 100f)]
    public float knockChance;
    [Range(0f, 75f)]
    public float absorptionPercentage;
    public UnityEvent OnPlayerKillEnemy;
    public UnityEvent OnPlayerDamageEnemy;

    Vector2 mv;
    [HideInInspector]
    public Vector2 mp;
    float originalZoomSize;
    float currentGracePeriod;
    bool onGrace;
    float currentGrenadeTime;
    int currentGrenadeCount;
    bool inGrenadeCooldown;
    GameManager manager;
    UIManager uiManager;

    public override void Start()
    {
        base.Start();

        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        originalZoomSize = playerCam.m_Lens.OrthographicSize;
        originalMoveSpeed = moveSpeed;
        focusedMoveSpeed = moveSpeed / speedDivisorReductorOnFocus;
        runningMoveSpeed = moveSpeed * 2f;
        originalRunningMoveSpeed = runningMoveSpeed;
        currentGrenadeCount = grenadeCount;

        if (flankParent != null)
        {
            foreach (Transform child in flankParent)
                playerFlanks.Add(child.GetComponent<Flank>());
            
            foreach (Flank f in playerFlanks)
            {
                f.entity = this;
                f.consumeMag = true;
            }

            for (int i = 0; i < playerFlanks.Count; i++)
            {   
                if (playerFlanks[i].boughtWeapon)
                {
                    manager.weaponHUD[i].gameObject.SetActive(true);
                    manager.weaponHUD[i].GetComponent<WeaponHUD>().SetWeaponInfo(playerFlanks[i].weaponStat);
                }

                uiManager._playerFlanks.Add(playerFlanks[i]);

                if (!playerFlanks[i].boughtWeapon)
                    playerFlanks[i].gameObject.SetActive(false);
            }
        }
    }

    public override void Update()
    {   
        if (manager.currentGameState != GameManager.GameState.Tabbed)
        {
            base.Update();

            GetPlayerInput();
            HandleGrenades();
            HandleGrace();
        }
    }

    void FixedUpdate()
    {   
        if (manager.currentGameState != GameManager.GameState.Tabbed)
            PlayerMovement();
    }

    void GetPlayerInput()
    {
        mv.x = Input.GetAxisRaw("Horizontal");
        mv.y = Input.GetAxisRaw("Vertical");

        mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (playerFlanks.Count > 0)
        {
            foreach (Flank f in playerFlanks)
            {
                if (f.currentFireMode == Flank.FireMode.SingleShot ||
                    f.currentFireMode == Flank.FireMode.SemiAuto ||
                    f.currentFireMode == Flank.FireMode.ShotgunBurst)
                {   
                    if (Input.GetMouseButtonDown(0))
                        f.Shoot();
                }

                else
                    if (Input.GetMouseButton(0))
                        f.Shoot();
            }
        }
        
        if (Input.GetKey(KeyCode.LeftShift) && !focusing)
        {   
            if (moveSpeed < runningMoveSpeed)
                moveSpeed = Mathf.Lerp(moveSpeed, runningMoveSpeed, runSpeedTransitionSpeed * Time.deltaTime);
        }

        else
        {
            if (moveSpeed > originalMoveSpeed)
                moveSpeed = Mathf.Lerp(moveSpeed, originalMoveSpeed, runSpeedTransitionSpeed * Time.deltaTime);

            if (Input.GetMouseButton(1))
            {
                if (playerCam.m_Lens.OrthographicSize >= zoomSize)
                {
                    playerCam.m_Lens.OrthographicSize = Mathf.Lerp(playerCam.m_Lens.OrthographicSize, zoomSize, zoomSpeed * Time.deltaTime);
                    moveSpeed = Mathf.Lerp(moveSpeed, focusedMoveSpeed, zoomSpeed * Time.deltaTime);

                    foreach (Flank f in playerFlanks)
                    {
                        f.accuracy.x = Mathf.Lerp(f.accuracy.x, f.focusedAccuracy.x, zoomSpeed * Time.deltaTime);
                        f.accuracy.y = Mathf.Lerp(f.accuracy.y, f.focusedAccuracy.y, zoomSpeed * Time.deltaTime);
                    }
                }

                focusing = true;
            }

            else
            {
                if (playerCam.m_Lens.OrthographicSize <= originalZoomSize)
                {
                    playerCam.m_Lens.OrthographicSize = Mathf.Lerp(playerCam.m_Lens.OrthographicSize, originalZoomSize, zoomSpeed * Time.deltaTime);
                    moveSpeed = Mathf.Lerp(moveSpeed, originalMoveSpeed, zoomSpeed * Time.deltaTime);

                    foreach (Flank f in playerFlanks)
                    {
                        f.accuracy.x = Mathf.Lerp(f.accuracy.x, f.originalAccuracy.x, zoomSpeed * Time.deltaTime);
                        f.accuracy.y = Mathf.Lerp(f.accuracy.y, f.originalAccuracy.y, zoomSpeed * Time.deltaTime);
                    }
                }

                focusing = false;
            }
        }

        if (currentGrenadeCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inGrenadeCooldown)
                {
                    Instantiate(playerGrenade.gameObject, grenadeShootPoint.position, Quaternion.Euler(grenadeShootPoint.rotation.eulerAngles + new Vector3(0f, 0f, 90f)));
                    currentGrenadeCount--;
                    inGrenadeCooldown = true;
                }
            }
        }
    }

    void HandleGrace()
    {
        if (onGrace)
        {
            if (currentGracePeriod >= gracePeriod)
            {
                currentGracePeriod = 0f;
                onGrace = false;
            }

            else
                currentGracePeriod += Time.deltaTime;
        }
    }

    void HandleGrenades()
    {
        if (inGrenadeCooldown)
        {
            if (currentGrenadeTime >= grenadeCooldown)
            {
                inGrenadeCooldown = false;
                currentGrenadeTime = 0f;
            }

            else
                currentGrenadeTime += Time.deltaTime;
        }
    }

    void PlayerMovement()
    {
        rb.MovePosition(rb.position + mv.normalized * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mp - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    public override void MakeDamage(int damage)
    {
        base.MakeDamage(damage);

        if (!onGrace)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                TriggerDeath();
            }

            onGrace = true;
        }
    }

    public override void TriggerDeath()
    {
        base.TriggerDeath();

        box.enabled = false;
    }
}
