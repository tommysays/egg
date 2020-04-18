using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFader : MonoBehaviour
{
    private float timePassed = 0f;
    private float attackDuration;
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        attackDuration = clips[0] != null ? clips[0].length : 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > attackDuration) {
            Destroy(gameObject);
        }
    }
}
