using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivingThing : MonoBehaviour
{
    public int baseHealth;
    public int startingHealth;
    public int currentHealth;
    public int dealtDamage;
    public int recievedBullets;
    public UnityEvent OnEntityTakeDamage;
    public UnityEvent OnEntityDeath;

    [Header ("Don't Modify")]
    public Collider2D box;
    public Rigidbody2D rb;

    public virtual void Start()
    {
        box = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (baseHealth > 0)
            startingHealth = currentHealth = baseHealth;
        else
            Debug.LogError(gameObject.name + " has 0 as base health");
    }

    public virtual void Update()
    {
        
    }

    public virtual void MakeDamage(int damage)
    {}

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > startingHealth)
            currentHealth = startingHealth;
    }

    public virtual void TriggerDeath()
    {
        Debug.Log(gameObject.name + " died");
    }
}
