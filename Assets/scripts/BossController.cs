using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    // Start is called before the first frame update
    private Animator animator;

    private float timer = 0f;
    private float IdleToPlantingInterval = 4f;
    private float ProducingEnemiesInterval = 2f;
    private BossState bossState = BossState.SPAWNING;

    public override void Start()
    {
        animator = GetComponent<Animator>();


        currentState = EnemyState.SPAWNING;
        base.Start();
        nightController = FindObjectOfType<NightController>(); ;


    }

    public void GetHit()
    {
        animator.SetTrigger("attackedTrigger");
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (bossState == BossState.IDLE)
        {
            if (timer > IdleToPlantingInterval)
            {
                timer = 0f;
                animator.SetTrigger("plantTrigger");
                bossState = BossState.PLANTING;
            }
        }
        else if (bossState == BossState.PRODUCING)
        {
            if (timer > ProducingEnemiesInterval)
            {
                timer = 0f;
                ///Spawn enemy
                int x = nightController.EnemyPrefabs.Length;

                Vector3 spawnPosition = this.transform.position;
                GameObject enemy = GameObject.Instantiate(nightController.EnemyPrefabs[(int)Random.Range(0,x-1)], spawnPosition, Quaternion.identity);
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.nightController = nightController;
               
                
            }
        }
    }
    

    public void SpawningDone()
    {
        bossState = BossState.IDLE;
        timer = 0f;
    }

    public void IdleDone()
    {
        bossState = BossState.PLANTING;
        timer = 0f;
    }

    public void PlantingDone()
    {
        bossState = BossState.PRODUCING;
        timer = 0f;
    }

    public enum BossState
    {
        SPAWNING,
        IDLE,
        PLANTING,
        PRODUCING
    }
}
