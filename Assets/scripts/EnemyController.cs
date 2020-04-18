using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Health;
    public float Movespeed;
    public float Range;
    public int Damage;
    public float AttackSpeed;

    public NightController nightController;

    private EnemyState currentState = EnemyState.NONE;
    private float attackTimer = 0f;

    private SpriteRenderer spriteRenderer;
    //private bool isFlipped;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //isFlipped = spriteRenderer.flipX;
        Flip();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.NONE) {
            // Approach.
            float speed = Movespeed * Time.deltaTime;
            Vector3 destination = new Vector3(0, 0, transform.position.z);
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, destination, speed);
            transform.position = nextPosition;


            // Check if in range.
            if (Vector3.Distance(transform.position, destination) < Range) {
                currentState = EnemyState.REACHED_FIRE;
            }
        } else {
            // Attack fire.
            attackTimer += Time.deltaTime;
            if (attackTimer > AttackSpeed) {
                // TODO Attack animation?
                attackTimer = 0f;
                nightController.DamageFire(Damage);
            }
        }
    }

    /// <summary>
    /// Flip sprite if heading left instead of right.
    /// </summary>
    private void Flip() {
        bool shouldFlip = transform.position.x > 0;
        if (spriteRenderer.flipX != shouldFlip) {
            spriteRenderer.flipX = shouldFlip;
        }

    }

    public enum EnemyState {
        NONE,
        REACHED_FIRE
    }
}
