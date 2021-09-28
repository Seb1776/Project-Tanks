using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingThing
{
    Player player;
    AbilityManager am;

    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void MakeDamage(int damage)
    {
        base.MakeDamage(damage);

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            am.damageDealtToLastEnemy = dealtDamage;
            am.bulletsRecievedByLastEnemy = recievedBullets;
            player.OnPlayerKillEnemy.Invoke();
            TriggerDeath();
        }
    }

    public override void TriggerDeath()
    {
        base.TriggerDeath();

        box.enabled = false;
    }
}
