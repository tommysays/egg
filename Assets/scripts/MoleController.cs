using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleController : EnemyController
{
    public float StartDelay;
    private float startTimer = 0f;

    public override void Start() {
        currentState = EnemyState.SPAWNING;
        base.Start();
    }
    // Update is called once per frame
    public override void Update()
    {
        if (startTimer > StartDelay) {
            if (currentState == EnemyState.SPAWNING) {
                currentState = EnemyState.NONE;
            }
            base.Update();
        } else {
            startTimer += Time.deltaTime;
        }
    }
}
