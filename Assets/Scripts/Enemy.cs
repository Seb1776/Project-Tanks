using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : LivingThing
{
    [Header("------------")]
    [Header("Enemy Fields")]
    public bool showDebugLimits;
    public enum EnemyState {Wander, Chasing, Attacking, Stop}
    public EnemyState enemyState;
    public enum ShootRotation {SpinFire, FocusPlayer}
    public ShootRotation shootRotationMode;
    public List<Flank> enemyFlanks = new List<Flank>();
    public Transform flankParent;
    public float chaseDetectionDistance;
    public float attackDetectionDistance;
    public float stopDetectionDistance;
    public float enemyMoveSpace;
    public int fireDamagePerSecond;
    public float fireDamageDuration;
    public float electricEffectDuration;
    public bool canMove;
    public float moveSpeed;
    public bool canRotate;
    public bool canShoot;
    public float rotationSpeed;

    float currentElectricEffectDuration;
    float currentFireEffectDuration;
    float currentFireDPSDuration;
    float currentElectricEPSDuration;
    bool onElectricity;
    bool onFire;
    float nearCount;
    GameObject currentElectricEffect;
    GameObject currentFireEffect;
    Vector2 areaSum = Vector2.zero;
    Player player;
    AbilityManager am;

    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();

        if (flankParent != null)
        {
            foreach (Transform child in flankParent)
                enemyFlanks.Add(child.GetComponent<Flank>());
            
            foreach (Flank f in enemyFlanks)
            {
                f.entity = this;
                f.consumeMag = false;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        EnemyBehaviour();
        HandleElectricEffect();
        HandleFireEffect();
        GetStateDistanceFromPlayer();
    }

    void EnemyBehaviour()
    {
        if (canRotate)
        {   
            if (enemyState != EnemyState.Chasing || enemyState != EnemyState.Wander)
            {
                if (enemyState == EnemyState.Attacking || enemyState == EnemyState.Stop)
                {   
                    if (shootRotationMode == ShootRotation.SpinFire)
                        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
                    
                    else
                        LookAtTarget(player.transform.position);
                }

                else
                    LookAtTarget(player.transform.position);
            }

            else
                LookAtTarget(player.transform.position);
        }

        if (canMove)
        {
            if ((enemyState == EnemyState.Chasing || enemyState == EnemyState.Attacking) && (enemyState != EnemyState.Stop || enemyState != EnemyState.Wander))
                MoveEnemy(player.transform.position);
        }

        if (canShoot)
        {
            if (enemyState == EnemyState.Attacking || enemyState == EnemyState.Stop)
                foreach (Flank f in enemyFlanks)
                    f.Shoot();
        }
    }

    void MoveEnemy(Vector3 target)
    {
        Collider2D[] nearEnemies = Physics2D.OverlapCircleAll(transform.position, enemyMoveSpace);

        foreach (Collider2D c2d in nearEnemies)
        {
            if (c2d.GetComponent<Enemy>() != null && !c2d.CompareTag("Collisionable") && c2d.transform != transform)
            {
                Vector2 diff = transform.position - c2d.transform.position;
                diff = diff.normalized / Mathf.Abs(diff.magnitude);
                areaSum += diff;
                nearCount++;
            }
        }

        if (nearCount > 0)
        {
            areaSum /= nearCount;
            areaSum = areaSum.normalized * moveSpeed;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)areaSum, (moveSpeed / 2f) * Time.deltaTime);
        }

        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void GetStateDistanceFromPlayer()
    {
        float dstFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (dstFromPlayer > chaseDetectionDistance)
            enemyState = EnemyState.Wander;
        
        else if (dstFromPlayer <= chaseDetectionDistance)
        {
            enemyState = EnemyState.Chasing;

            if (dstFromPlayer <= attackDetectionDistance)
            {
                enemyState = EnemyState.Attacking;

                if (dstFromPlayer <= stopDetectionDistance)
                {
                    enemyState = EnemyState.Stop;
                }
            }
        }
    }

    void LookAtTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.Normalize();
        float zAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion lookRot = Quaternion.Euler(0f, 0f, zAngle - 90);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
    }

    public override void MakeDamage(int damage)
    {
        base.MakeDamage(damage);

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            am.damageDealtToLastEnemy = dealtDamage;
            am.bulletsRecievedByLastEnemy = recievedBullets;
            player.OnPlayerKillEnemy.Invoke();
            TriggerDeath();
        }
    }

    public void TriggerElectricity()
    {
        if (!onElectricity)
        {
            onElectricity = true;
            currentElectricEffect = Instantiate(electrifiedEffect, transform.position, Quaternion.identity);
        }

        else
        {
            currentElectricEffectDuration -= 2f;

            if (currentElectricEffectDuration < 0f)
                currentElectricEffectDuration = 0f;
        }
    }

    void HandleElectricEffect()
    {
        if (onElectricity)
        {
            if (currentElectricEffectDuration >= electricEffectDuration)
            {
                currentElectricEffectDuration = 0f;
                onElectricity = false;
                canMove = canRotate = canShoot = true;

                if (currentElectricEffect != null)
                {
                    Destroy(currentElectricEffect.gameObject);
                    currentElectricEffect = null;
                }
            }

            else
            {
                currentElectricEffectDuration += Time.deltaTime;
                canMove = canRotate = canShoot = false;
            }
            
            if (currentElectricEPSDuration >= 0.5f)
            {
                var thisRotation = transform.eulerAngles;
                thisRotation.z = Random.Range(0f, 360f);
                transform.eulerAngles = thisRotation;
                currentElectricEPSDuration = 0f;
            }

            else
                currentElectricEPSDuration += Time.deltaTime;
        }
    }

    public void TriggerFire()
    {
        if (!onFire)
            onFire = true;

        else
        {
            currentFireEffectDuration -= 2f;

            if (currentFireEffectDuration < 0f)
                currentFireEffectDuration = 0f;
        }
    }

    void HandleFireEffect()
    {
        if (onFire)
        {
            if (currentFireEffectDuration >= fireDamageDuration)
            {
                currentFireEffectDuration = 0f;
                onFire = false;
            }

            else
                currentFireEffectDuration += Time.deltaTime;
            
            if (currentFireDPSDuration >= 1f)
            {
                dealtDamage += fireDamagePerSecond;
                MakeDamage(fireDamagePerSecond);
                currentFireDPSDuration = 0f;
            }

            else
                currentElectricEPSDuration += Time.deltaTime;
        }
    }

    public override void TriggerDeath()
    {
        base.TriggerDeath();

        box.enabled = false;
        onFire = onElectricity = false;
    }

    void OnDrawGizmosSelected() 
    {   
        if (showDebugLimits)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDetectionDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDetectionDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stopDetectionDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyMoveSpace);
        }
    }
}
