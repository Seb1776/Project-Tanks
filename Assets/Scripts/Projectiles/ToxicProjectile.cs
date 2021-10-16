using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicProjectile : Projectile
{
    [Header("Toxic Projectile")]
    [Header ("---------------")]
    public Environmental enviromentalEffect;
    [Range(0f, 1f)]
    public float chanceOfSmokeCloud;

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
    }
}
