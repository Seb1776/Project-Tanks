using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingProjectile : Projectile
{
    [Header("Piercing Projectile")]
    [Header("-------------------")]
    public int pierceAmount;
    public int currentPierced;

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
