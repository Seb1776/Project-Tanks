using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikzEnemy : Enemy
{
    [Header("-----------")]
    [Header("Kamikz Unit")]
    public float timeToDie;
    public Animator lights;
    public GameObject kamikzExplosion;

    bool createdExp;
    float timeBtwScs;
    float currentTimeToDie;

    public override void Start()
    {
        base.Start();

        enemyFlanks.Clear();
    }

    public override void Update()
    {
        base.Update();
        HandleKamikz();
    }

    void HandleKamikz()
    {
        if (enemyState == EnemyState.Attacking || enemyState == EnemyState.Stop && !onElectricity && !dead)
        {
            if (currentTimeToDie >= timeToDie)
            {
                MakeDamage(currentHealth);

                if (!createdExp)
                {
                    Instantiate(kamikzExplosion, transform.position, Quaternion.identity);
                    createdExp = true;
                }
            }

            else
                currentTimeToDie += Time.deltaTime;
            
            if (timeBtwScs >= 1f)
            {
                lights.SetTrigger("blink");
                timeBtwScs = 0f;
            }

            else
                timeBtwScs += Time.deltaTime;
        }
    }
}
