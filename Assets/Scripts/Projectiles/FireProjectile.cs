using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : Projectile
{
    [Header ("Fire Projectile")]
    [Header ("-------------------")]
    public float chanceToFire;

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

        if (Random.value <= chanceToFire)
            other.GetComponent<Enemy>().TriggerFire();
    }
}
