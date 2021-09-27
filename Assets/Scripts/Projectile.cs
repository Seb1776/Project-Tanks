using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType {Normal, Fire, Explosive, Toxic, Piercing}
    public ProjectileType currentProjectile;
    public float moveSpeed;
    public int damage;

    AbilityManager am;
    Collider2D box;
    Rigidbody2D rb;

    void Start()
    {
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        box = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
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
            other.GetComponent<Enemy>().MakeDamage(damage);
            player.OnPlayerDamageEnemy.Invoke();
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
        }
    }
}
