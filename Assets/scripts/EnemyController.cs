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
    public GameObject AttackPrefab;

    public NightController nightController;

    private EnemyState currentState = EnemyState.NONE;
    private float attackTimer = 0f;
    private Vector3 destination;

    private SpriteRenderer spriteRenderer;
    //private bool isFlipped;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        destination = new Vector3(0, -0.1f, 0);
        Flip();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.NONE) {
            // Approach.
            float speed = Movespeed * Time.deltaTime;
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
                Vector3 spawnPosition =  Vector3.MoveTowards(transform.position, destination, 0.1f);
                GameObject attackObj = GameObject.Instantiate(AttackPrefab, new Vector3(spawnPosition.x, spawnPosition.y, -1f),
                    Quaternion.identity);
                attackObj.transform.right = destination - transform.position;
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
