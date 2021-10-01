using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : Explosion
{
    public override void Start()
    {
        base.Start();
    }

    public override void HandleExplosion()
    {
        base.HandleExplosion();

        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, explosionRadius, affectedByExplosion);

        foreach (Collider2D c in near)
        {
            if (c.transform.GetComponent<Enemy>() != null)
            {
                c.transform.GetComponent<Enemy>().TriggerFire();
            }
        }
    }
}
