using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using Pathfinding;
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
    [Range(0f, 360f)]
    public float fieldOfView;
    public LayerMask whatIsTarget;
    public LayerMask whatIsObstacle;
    public List<int> flankDamage = new List<int>();
    public List<float> flankReload = new List<float>();
    public float chaseDetectionDistance;
    public float attackDetectionDistance;
    public float stopDetectionDistance;
    public float deathDeltaTime;
    public float enemyMoveSpace;
    public int fireDamagePerSecond;
    public float fireDamageDuration;
    public float electricEffectDuration;
    public bool canMove;
    public float moveSpeed;
    public bool canRotate;
    public bool canShoot;
    public bool playerOnSight;
    public float rotationSpeed;
    public float rotationAttackSpeed;
    public Explosion deathExplosion;

    [SerializeField]
    List<Transform> visibleTargets = new List<Transform>();
    float currentElectricEffectDuration;
    float currentFireEffectDuration;
    float currentFireDPSDuration;
    float currentElectricEPSDuration;
    public bool onElectricity;
    public bool onFire;
    public bool dead;
    float nearCount;
    GameObject currentElectricEffect;
    GameObject currentFireEffect;
    Vector2 areaSum = Vector2.zero;
    Player player;
    AbilityManager am;
    /*[SerializeField] DestinationSetter destination;
    [SerializeField] AIPath path;*/

    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        am = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();

        if (flankParent != null)
        {
            foreach (Transform child in flankParent)
                enemyFlanks.Add(child.GetComponent<Flank>());

            for (int i = 0; i < enemyFlanks.Count; i++)
            {
                enemyFlanks[i].entity = this;
                enemyFlanks[i].reloadTime = flankReload[i];
                enemyFlanks[i].damage = flankDamage[i];
                enemyFlanks[i].consumeMag = false;
            }
        }

        canMove = canRotate = canShoot = true;

        SetPathfindingData();
        StartCoroutine(FindTargets(.1f));
    }

    public override void Update()
    {   
        if (!dead)
        {
            base.Update();
            EnemyBehaviour();
            HandleElectricEffect();
            HandleFireEffect();
            GetStateDistanceFromPlayer();
        }

        else
        {
            HandleEnemyDeath();
            canMove = canRotate = canShoot = false;
        }
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
                        transform.Rotate(0f, 0f, rotationAttackSpeed * Time.deltaTime);
                    
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
            {
                //destination.target = player.transform;
            }
        }

        if (canShoot)
        {
            if (playerOnSight && (enemyState == EnemyState.Attacking || enemyState == EnemyState.Stop))
                foreach (Flank f in enemyFlanks)
                    f.Shoot();
        }
    }

    void MoveEnemy()
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
    }

    void SetPathfindingData()
    {
        /*path = GetComponent<AIPath>();
        destination = GetComponent<AIDestinationSetter>();

        path.slowdownDistance = stopDetectionDistance + 4f;
        path.endReachedDistance = stopDetectionDistance;
        path.maxSpeed = moveSpeed;*/
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

    public override void TriggerDeath()
    {
        base.TriggerDeath();

        box.enabled = false;
        onFire = onElectricity = false;

        if (!dead)
            dead = true;
    }

    void HandleEnemyDeath()
    {   
        if (dead)
        {
            Vector2 tmpScale = transform.localScale;

            if (tmpScale.x > 0.001f && tmpScale.y > 0.001f)
                tmpScale = Vector2.Lerp(tmpScale, Vector2.zero, deathDeltaTime * Time.deltaTime);

            else
            {
                if (deathExplosion != null)
                    Instantiate(deathExplosion.gameObject, transform.position, Quaternion.identity);

                Destroy(this.gameObject);
            }

            transform.localScale = tmpScale;
        }
    }

    public void TriggerElectricity()
    {
        if (!onElectricity)
        {
            onElectricity = true;
            
            if (currentElectricEffect == null)
            {
                currentElectricEffect = Instantiate(electrifiedEffect, transform.position, Quaternion.identity);
                currentElectricEffect.transform.parent = this.transform;
            }
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
        {
            onFire = true;
            
            if (currentFireEffect == null)
            {
                currentFireEffect = Instantiate(onFireEffect, transform.position, Quaternion.identity);
                currentFireEffect.transform.parent = this.transform;
            }
        }

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

                if (currentFireEffect != null)
                {
                    Destroy(currentFireEffect.gameObject);
                    currentFireEffect = null;
                }
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
                currentFireDPSDuration += Time.deltaTime;
        }
    }

    IEnumerator FindTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider2D[] nearTargets = Physics2D.OverlapCircleAll(transform.position, attackDetectionDistance, whatIsTarget);

        for (int i = 0; i < nearTargets.Length; i++)
        {
            Transform target = nearTargets[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.up, dirToTarget) < fieldOfView / 2f)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, whatIsObstacle))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        playerOnSight = visibleTargets.Contains(player.transform);
    }

    public virtual void OnDrawGizmosSelected() 
    {
        if (showDebugLimits)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDetectionDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDetectionDistance);

            if (Vector2.Distance(transform.position, player.gameObject.transform.position) <= attackDetectionDistance)
                Gizmos.DrawLine(transform.position, player.gameObject.transform.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stopDetectionDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyMoveSpace);
        }
    }

    public Vector2 DirFromAngle(float angle, bool global)
    {
        if (!global)
            angle -= transform.eulerAngles.z;

        return new Vector3(Mathf.Sin(angle * Mathf.Rad2Deg), Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}