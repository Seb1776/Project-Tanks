using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int health;
    public Explosion explodeEffect;
    [Range(0f, 1f)]
    public float chanceOfAppearingPickup;
    public Interactable[] pickupType;

    int currentHealth;

    void Start()
    {
        currentHealth = health;
    }

    public void MakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            DestroyDestructible();
    }

    void DestroyDestructible()
    {
        if (explodeEffect != null)
            Instantiate(explodeEffect.gameObject, transform.position, Quaternion.identity);
        
        if (pickupType.Length > 0 && Random.value <= chanceOfAppearingPickup)
        {
            int randInteractable = Random.Range(0, pickupType.Length);
            Interactable selected = pickupType[randInteractable];
            Instantiate(selected.gameObject, transform.position, Quaternion.identity);
        }
        
        Destroy(this.gameObject);
    }
}
