using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environmental : MonoBehaviour
{
    public bool showDebugLimits;
    public float affectedRadius;
    public float timeBtwDamage;
    public float timeBeforeDeath;
    public int damageOverTime;
    public float lifeTime;
    public LayerMask affected;

    float currentTimeBeforeDeath;
    float currentLifeTime;
    float currentTimeBtwDamage;
    bool destroy;

    void Update()
    {
        if (!destroy)
            HandleEnviromental();
        else
            HandleDestroy();
    }

    void HandleEnviromental()
    {
        if (currentTimeBtwDamage >= timeBtwDamage)
        {
            Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, affectedRadius, affected);

            foreach (Collider2D c in near)
            {
                c.transform.GetComponent<LivingThing>().MakeDamage(damageOverTime);
            }

            currentTimeBtwDamage = 0f;
        }

        else
            currentTimeBtwDamage += Time.deltaTime;
        
        if (currentLifeTime >= lifeTime)
            destroy = true;
        
        else
            currentLifeTime += Time.deltaTime;
    }

    void HandleDestroy()
    {
        Vector2 tmpLocalScale = this.transform.localScale;

        if (transform.childCount > 0)
        {
            ParticleSystem.MainModule effect = transform.GetChild(0).transform.GetComponent<ParticleSystem>().main;
        
            if (effect.loop)
                effect.loop = false;
        }

        if (currentTimeBeforeDeath >= timeBeforeDeath)
            Destroy(this.gameObject);

        else
            currentTimeBeforeDeath += Time.deltaTime;

        transform.localScale = tmpLocalScale;
    }

    void OnDrawGizmosSelected() 
    {
        if (showDebugLimits)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, affectedRadius);
        }    
    }
}
