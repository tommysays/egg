using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    // Start is called before the first frame update
    private Animator animator;
    public override void Start()
    {
        animator = GetComponent<Animator>();


        //currentState = EnemyState.SPAWNING;
        //base.Start();



        animator.SetTrigger("attackTrigger");
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
