using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public bool showDebugLimits;
    public float explosionRadius;
    public int explosionDamage;
    public float lifeTime;
    public GameObject explosionEffect;
    public LayerMask affectedByExplosion;
    public AudioClip explosionSound;

    AudioSource source;

    public virtual void Start()
    {
        source = GetComponent<AudioSource>();

        if (explosionSound != null && source != null)
            source.clip = explosionSound;

        HandleExplosion();
        Destroy(this.gameObject, lifeTime);
    }

    public virtual void HandleExplosion()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (explosionSound != null)
            source.PlayOneShot(explosionSound);

        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, explosionRadius, affectedByExplosion);

        foreach (Collider2D c in near)
        {
            if (c.transform.GetComponent<LivingThing>() != null)
            {
                c.transform.GetComponent<LivingThing>().MakeDamage(explosionDamage);
            }
        }
    }

    void OnDrawGizmosSelected() 
    {
        if (showDebugLimits)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
