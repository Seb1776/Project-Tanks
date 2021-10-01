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
        /*ParticleSystem effect = transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        effect.main.loop = false;*/

        if (tmpLocalScale.x > 0f && tmpLocalScale.y > 0f)
        {
            tmpLocalScale.x = Mathf.Lerp(tmpLocalScale.x, 0f, 2f * Time.deltaTime);
            tmpLocalScale.y = Mathf.Lerp(tmpLocalScale.y, 0f, 2f * Time.deltaTime);
        }

        else if (tmpLocalScale.x <= 0f && tmpLocalScale.y <= 0f)
            Destroy(this.gameObject);

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
