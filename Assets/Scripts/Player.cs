using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : LivingThing
{
    [Header ("-------------")]
    [Header ("Player Fields")]
    public float moveSpeed;
    public bool canMove;
    public Transform flankParent;
    public List<Flank> playerFlanks = new List<Flank>();
    public KeyCode key;
    [Range(0f, 100f)]
    public float dodgeChance;
    [Range(0f, 100f)]
    public float critChance;
    [Range(0f, 100f)]
    public float knockChance;
    public UnityEvent OnPlayerKillEnemy;
    public UnityEvent OnPlayerDamageEnemy;

    Vector2 mv;
    Vector2 mp;

    public override void Start()
    {
        base.Start();

        if (flankParent != null)
            foreach (Transform child in flankParent)
                playerFlanks.Add(child.GetComponent<Flank>());
    }

    public override void Update()
    {
        base.Update();

        GetPlayerInput();
    }

    void FixedUpdate()
    {
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

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            TriggerDeath();
        }
    }
}
