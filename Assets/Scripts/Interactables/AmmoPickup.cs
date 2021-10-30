using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    bool atLeastOne;

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < other.GetComponent<Player>().playerFlanks.Count; i++)
            {
                if (other.GetComponent<Player>().playerFlanks[i].currentMag < other.GetComponent<Player>().playerFlanks[i].magSize)
                {
                    other.GetComponent<Player>().playerFlanks[i].AddAmmo(
                        (int)Random.Range(other.GetComponent<Player>().playerFlanks[i].ammoPickupChance.x, other.GetComponent<Player>().playerFlanks[i].ammoPickupChance.y));
                    
                    atLeastOne = true;
                }
            }

            if (atLeastOne) Destroy(this.gameObject);
        }
    }
}
