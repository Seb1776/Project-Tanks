using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicExplosion : Explosion
{
    public Environmental enviromentalEffect;

    float currentTime;

    public override void Start()
    {
        base.Start();
    }

    public override void HandleExplosion()
    {
        base.HandleExplosion();
        Instantiate(enviromentalEffect.gameObject, transform.position, Quaternion.identity);
    }
}
