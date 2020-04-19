using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject MeleeColliderObj;
    public GameObject RangedAttackPrefab;
    public GameObject NightControllerObj;
    public PlayerState currentState = PlayerState.NONE;

    public int MeleeDamage;
    public int RangedDamage;

    private float movespeed = 1.2f;
    private const float minX = -3f,
                        minY = -2f,
                        maxX = 3f,
                        maxY = 2f;
    private const float stunDuration = 2f;
    private const float TOLERANCE = 0.0001f;
    private const float RANGED_X_OFFSET = 0.22f;
    private const float RANGED_Y_OFFSET = 0.11f;

    private float rangedAttackDelay = 0.4f;
    private float sacrificeDelay = 0.5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MeleeController meleeController;
    private NightController nightController;

    void Start()
    {
        nightController = NightControllerObj.GetComponent<NightController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        meleeController = MeleeColliderObj.GetComponent<MeleeController>();
        meleeController.Damage = MeleeDamage;
    }

    void Update()
    {
        if (currentState == PlayerState.NONE) {
            HandleMovement();
            if (Input.GetButtonDown("Fire1")) {
                currentState = PlayerState.ATTACKING;
                animator.SetTrigger("attackTrigger");
                meleeController.HasHit = false;
                meleeController.CheckForCollisions = true;
            } else if (Input.GetButtonDown("Fire2")) {
                currentState = PlayerState.ATTACKING;
                animator.SetTrigger("rangedAttackTrigger");
                StartCoroutine(RangedAttackSpawn());
            } else if (Input.GetButtonDown("Fire3") && nightController.CurrentHearts > 0) {
                currentState = PlayerState.SACRIFICING;
                animator.SetTrigger("sacrificeTrigger");
                StartCoroutine(SacrificeDelay());
            }
        }
    }

    public void DoneAttacking() {
        if (currentState == PlayerState.ATTACKING) {
            currentState = PlayerState.NONE;
            meleeController.HasHit = false;
            meleeController.CheckForCollisions = false;
        } else {
            Debug.LogWarning("Stopped attack without actually attacking?");
        }
    }

    public void DoneSacrificing() {
        if (currentState == PlayerState.SACRIFICING) {
            currentState = PlayerState.NONE;
        } else {
            Debug.LogWarning("Animator triggered stop sacrifice, but player was not sacrificing. PlayerState = " + currentState.ToString());
        }
    }

    private void HandleMovement() {
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(Horizontal, Vertical) * movespeed * Time.deltaTime;
        Vector3 currentPosition = transform.position;
        float newX = currentPosition.x + movement.x;
        float newY = currentPosition.y + movement.y;
        newX = newX < minX ? minX : newX > maxX ? maxX : newX;
        newY = newY < minY ? minY : newY > maxY ? maxY : newY;
        transform.position = new Vector3(newX, newY, newY);
        if (Horizontal < -TOLERANCE) {
            // Moving left, so make sure sprite is flipped.
            if (!spriteRenderer.flipX) {
                spriteRenderer.flipX = true;
                Vector3 position = MeleeColliderObj.transform.localPosition;
                MeleeColliderObj.transform.localPosition = new Vector3(-position.x, position.y, position.z);
            }
        } else if (Horizontal > TOLERANCE) {
            if (spriteRenderer.flipX) {
                spriteRenderer.flipX = false;
                Vector3 position = MeleeColliderObj.transform.localPosition;
                MeleeColliderObj.transform.localPosition = new Vector3(-position.x, position.y, position.z);
            }
        }
    }

    public void Stun() {
        if (currentState != PlayerState.STUNNED) {
            currentState = PlayerState.STUNNED;
            StartCoroutine(StunTimeout());
        }
    }

    private IEnumerator StunTimeout() {
        yield return new WaitForSeconds(stunDuration);
        currentState = PlayerState.NONE;
    }

    private IEnumerator RangedAttackSpawn() {
        yield return new WaitForSeconds(rangedAttackDelay);
        if (currentState != PlayerState.ATTACKING) {
            // Player was interrupted before they could complete ranged attack.
            yield break;
        }
        Vector3 position = transform.position;
        float deltaX = spriteRenderer.flipX ? -RANGED_X_OFFSET : RANGED_X_OFFSET;
        Vector3 newPosition = new Vector3(position.x + deltaX, position.y + RANGED_Y_OFFSET, position.z);
        GameObject rangedAttack = GameObject.Instantiate(RangedAttackPrefab, newPosition, Quaternion.identity);
        RangedAttackController controller = rangedAttack.GetComponent<RangedAttackController>();
        if (spriteRenderer.flipX) {
            rangedAttack.GetComponent<SpriteRenderer>().flipX = true;
            controller.MoveLeft = true;
        }
        controller.Damage = RangedDamage;
    }

    private IEnumerator SacrificeDelay() {
        yield return new WaitForSeconds(sacrificeDelay);
        if (currentState != PlayerState.SACRIFICING) {
            // Player was interrupted before they could complete sacrifice.
            yield break;
        }
        nightController.SacrificeHeart();
    }

    public enum PlayerState {
        NONE,
        ATTACKING,
        SACRIFICING,
        STUNNED
    };
}
