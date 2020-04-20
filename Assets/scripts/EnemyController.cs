using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Health {
        get {
            return _health;
        }
        set {
            if (value < 0) {
                value = 0;
            }
            _health = value;
            if (_health == 0) {
                currentState = EnemyState.DYING;
                nightController.CheckForWinCondition();
            }
        }
    }
    private int _health;

    public float DelayAfterHittingPlayer;
    public int MaxHealth;
    public float Movespeed;
    public float Range;
    public int Damage;
    public float AttackSpeed;
    public GameObject AttackPrefab;
    /// For determining how far in front of the enemy the attack animation should spawn.
    public float AttackSpawnDistance = 0.1f;

    public NightController nightController;
    public GameObject PlayerObj {
        get {
            return _playerObj;
        }
        set {
            if (value != null) {
                player = value.GetComponent<PlayerController>();
            } else {
                player = null;
            }
            _playerObj = value;
        }
    }
    private GameObject _playerObj;

    public EnemyState currentState = EnemyState.APPROACHING_FIRE;
    protected float attackTimer = 0f;
    protected Vector3 firePosition;
    protected float deathFadeTimer = 0f;
    protected float deathFadeSeconds = 0.25f;

    protected SpriteRenderer spriteRenderer;
    protected PlayerController player;

    // After this interval, reevaluate whether to go to fire or chase player.
    private float reevaluationInterval = 0.5f;
    private float reevaluationTimer = 0f;

    public virtual void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        firePosition = new Vector3(0, -0.1f, 0);
        Health = MaxHealth;
        Flip();
    }

    public virtual void Update() {
        // Determine what to chase.
        reevaluationTimer += Time.deltaTime;
        if (reevaluationTimer >= reevaluationInterval) {
            ReevaluateLifeDecisions();
            reevaluationTimer = 0f;
        }

        switch (currentState) {
            case EnemyState.APPROACHING_FIRE:
                // Approach.
                float speed = Movespeed * Time.deltaTime;
                Vector3 nextPosition = Vector3.MoveTowards(transform.position, firePosition, speed);
                transform.position = nextPosition;
                // Check if in range.
                if (Vector3.Distance(transform.position, firePosition) < Range) {
                    currentState = EnemyState.REACHED_FIRE;
                }
            break;
            case EnemyState.REACHED_FIRE:
                // Attack fire.
                attackTimer += Time.deltaTime;
                if (attackTimer > AttackSpeed) {
                    SpawnAttackAnimation(firePosition);
                    attackTimer = 0f;
                    nightController.DamageFire(Damage);
                }
                break;
            case EnemyState.APPROACHING_PLAYER:
                if (player == null) {
                    currentState = EnemyState.APPROACHING_FIRE;
                } else {
                    // If we reached the player, attack them. Otherwise, keep moving towards them.
                    if (Vector3.Distance(player.transform.position, transform.position) < Range) {
                        currentState = EnemyState.ATTACKING_PLAYER;
                        player.Stun();
                        SpawnAttackAnimation(player.transform.position);
                        StartCoroutine(WaitAfterAttackingPlayer());
                    } else {
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Movespeed * Time.deltaTime);
                    }
                }
                break;
            case EnemyState.ATTACKING_PLAYER:
                break;
            case EnemyState.DYING:
                deathFadeTimer += Time.deltaTime;
                if (deathFadeTimer >= deathFadeSeconds) {
                    Destroy(gameObject);
                    return;
                }
                Color color = spriteRenderer.color;
                color.a = 1 - (deathFadeTimer / deathFadeSeconds);
                spriteRenderer.color = color;
                break;
        }
    }

    private void ReevaluateLifeDecisions() {
        if (player == null || currentState == EnemyState.ATTACKING_PLAYER || currentState == EnemyState.DYING) {
            return;
        }
        if (Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(transform.position, firePosition)) {
            currentState = EnemyState.APPROACHING_PLAYER;
        } else {
            currentState = EnemyState.APPROACHING_FIRE;
        }
    }

    private IEnumerator WaitAfterAttackingPlayer() {
        yield return new WaitForSeconds(DelayAfterHittingPlayer);
        currentState = EnemyState.APPROACHING_PLAYER;
    }

    private void SpawnAttackAnimation(Vector3 target) {
        Vector3 spawnPosition =  Vector3.MoveTowards(transform.position, target, AttackSpawnDistance);
        Vector3 delta = target - transform.position;
        GameObject attackObj = GameObject.Instantiate(AttackPrefab, spawnPosition, Quaternion.identity);
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        attackObj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
        APPROACHING_FIRE,
        SPAWNING,
        REACHED_FIRE,
        APPROACHING_PLAYER,
        ATTACKING_PLAYER,
        DYING
    }
}
