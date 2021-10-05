using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbangExplosion : Explosion
{
    public override void Start()
    {
        base.Start();
    }

    public override void HandleExplosion()
    {
        base.HandleExplosion();

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GameManager manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        float dstFromPlayer = Vector2.Distance(transform.position, player.gameObject.transform.position);

        if (dstFromPlayer <= explosionRadius)
        {
            float realDst = Mathf.Abs(10f - dstFromPlayer);
            manager.TriggerFlashbang(realDst / 10f);
        }
    }
}
