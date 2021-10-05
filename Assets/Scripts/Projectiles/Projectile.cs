using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType {Normal, Fire, Explosive, Electric, Toxic, Piercing}
    public ProjectileType currentProjectile;
    public float moveSpeed;
    public Vector2 possibleMoveSpeedDrag;
    public GameObject destroyEffect;
    public float lifeTime;
    public float delayBeforeDeath;
    public float destroyDecreaseFactor;
    public int damage;
    public bool destroy;
    public Flank parentFlank;
    public Vector2 firedFrom;
    public float traveledDistance;

    bool contact;
    AbilityManager am;
    Collider2D box;
    Rigidbody2D rb;

    public virtual void Start()
    {
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        box = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        float randDrag = Random.Range(possibleMoveSpeedDrag.x, possibleMoveSpeedDrag.y);

        if (randDrag < 0)
            moveSpeed -= randDrag;
        else if (randDrag > 0)
            moveSpeed += randDrag;

        StartCoroutine(ProjectileLifeTime());
    }

    public virtual void Update()
    {
        if (destroy)
            HandleDestroy();
    }

    void HandleDestroy()
    {
        contact = true;
        box.enabled = false;

        Vector2 tmpLocalScale = this.transform.localScale;
        Vector2 effectLocalScale = transform.GetChild(0).transform.localScale;

        if (tmpLocalScale.x > 0 && tmpLocalScale.y > 0)
        {
            tmpLocalScale.x -= Time.deltaTime * destroyDecreaseFactor;
            tmpLocalScale.y -= Time.deltaTime * destroyDecreaseFactor;
            effectLocalScale.x -= Time.deltaTime * destroyDecreaseFactor;
            effectLocalScale.y -= Time.deltaTime * destroyDecreaseFactor;
        }

        else
        {
            if (destroyEffect != null)
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            
            if (GetComponent<ExplosiveProjectile>() != null)
                if (GetComponent<ExplosiveProjectile>().explosion != null)
                    Instantiate(GetComponent<ExplosiveProjectile>().explosion.gameObject, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
        
        transform.GetChild(0).transform.localScale = effectLocalScale;
        transform.localScale = tmpLocalScale;
    }

    float GetDamageMultiplier()
    {
        for (int i = 0; i < parentFlank.optimalDamageRangeMultiplier.Count; i++)
        {
            if (traveledDistance <= parentFlank.optimalDamageRangeMultiplier[i].x)
            {
                return parentFlank.optimalDamageRangeMultiplier[i].y;
            }
        }

        return 1;
    }

    IEnumerator ProjectileLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!contact)
            destroy = true;
    }

    void FixedUpdate()
    {   
        if (!destroy)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
            traveledDistance = Vector2.Distance(transform.position, firedFrom);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        AbilityManager am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();

        if (this.transform.tag == "PlayerBullet" && other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().dealtDamage += (int)(damage * GetDamageMultiplier());
            other.GetComponent<Enemy>().recievedBullets++;
            am.lastKilledEnemy = other.GetComponent<Enemy>();
            am.lastProjectileHitEnemy = this;
            other.GetComponent<Enemy>().MakeDamage((int)(damage * GetDamageMultiplier()));
            player.OnPlayerDamageEnemy.Invoke();
            destroy = true;
        }

        else if (this.transform.tag == "EnemyBullet" && other.CompareTag("Player"))
        {
            if (am.preventsOrAffectPlayerDamageAbilities.Count > 0)
            {
                am.lastProjectileHitPlayer = this;
                player.OnEntityTakeDamage.Invoke();
            }

            else
            {
                other.GetComponent<Player>().MakeDamage(damage);
            }

            destroy = true;
        }

        else if (other.CompareTag("Collisionable"))
        {
            destroy = true;
        }
    }
}
