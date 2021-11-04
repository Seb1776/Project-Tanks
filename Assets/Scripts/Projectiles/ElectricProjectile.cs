using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricProjectile : Projectile
{
    [Header ("Electric Projectile")]
    [Header ("-------------------")]
    [Range(0f, 100f)]
    public float chanceToElectrify;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        float realChance = chanceToElectrify / 100f;

        if (this.transform.tag == "PlayerBullet" && other.transform.CompareTag("Enemy"))
            if (Random.value <= realChance)
                other.GetComponent<Enemy>().TriggerElectricity();
        
        if (this.transform.tag == "EnemyBullet" && other.transform.CompareTag("Player"))
        {
            if (Random.value <= realChance)
                other.GetComponent<Player>().TriggerElectricity();
        }
    }
}
