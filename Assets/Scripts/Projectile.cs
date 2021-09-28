using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType {Normal, Fire, Explosive, Toxic, Piercing}
    public ProjectileType currentProjectile;
    public float moveSpeed;
    public float lifeTime;
    public float destroyDecreaseFactor;
    public int damage;
    public bool destroy;

    AbilityManager am;
    Collider2D box;
    Rigidbody2D rb;

    void Start()
    {
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        box = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(ProjectileLifeTime());
    }

    void Update()
    {
        if (destroy)
            HandleDestroy();
    }

    void HandleDestroy()
    {
        StopAllCoroutines();
        box.enabled = false;

        Vector2 tmpLocalScale = this.transform.localScale;

        if (tmpLocalScale.x > 0 && tmpLocalScale.y > 0)
        {
            tmpLocalScale.x -= Time.deltaTime * destroyDecreaseFactor;
            tmpLocalScale.y -= Time.deltaTime * destroyDecreaseFactor;
        }

        else
            Destroy(this.gameObject);

        transform.localScale = tmpLocalScale;
    }

    IEnumerator ProjectileLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }

    void FixedUpdate()
    {   
        if (!destroy)
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        AbilityManager am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();

        if (this.transform.tag == "PlayerBullet" && other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().dealtDamage += damage;
            other.GetComponent<Enemy>().recievedBullets++;
            am.lastKilledEnemy = other.GetComponent<Enemy>();
            am.lastProjectileHitEnemy = this;
            other.GetComponent<Enemy>().MakeDamage(damage);
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
    }
}
