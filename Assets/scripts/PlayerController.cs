using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject MeleeColliderObj;
    public GameObject RangedAttackPrefab;
    public GameObject RangedAttackBuffedPrefab;
    public GameObject NightControllerObj;
    public GameObject BuffBarObj;
    public GameObject PlayerSpriteObj;
    public PlayerState currentState = PlayerState.NONE;

    public int MeleeDamage;
    public int RangedDamage;

    private float movespeed;
    private const float minX = -3f,
                        minY = -2f,
                        maxX = 3f,
                        maxY = 2f;
    private const float stunDuration = 1f;
    private float flashTimer = 0f;
    private Color stunColor = Color.yellow;
    private Color invulnerableColor = new Color(1f, 1f, 1f, 0.4f);
    private Color invulnerableColor2 = new Color(1f, 1f, 1f, 0.6f);
    private Color regularColor = Color.white;
    private const float invulnerableDuration = 2f;
    private bool isInvulnerable = false;
    private const float TOLERANCE = 0.0001f;
    private const float RANGED_X_OFFSET = 0.22f;
    private const float RANGED_Y_OFFSET = 0.41f;

    private float rangedAttackDelay = 0.4f;
    private float sacrificeDelay = 0.5f;
    private float buffDelay = 0.5f;

    private float buffTimer = 0f;
    private float buffDuration = 10f;
    private const float BUFF_MULTIPLIER = 1.2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MeleeController meleeController;
    private NightController nightController;
    private BuffBarController buffBarController;

    void Start() {
        nightController = NightControllerObj.GetComponent<NightController>();
        spriteRenderer = PlayerSpriteObj.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        meleeController = MeleeColliderObj.GetComponent<MeleeController>();
        buffBarController = BuffBarObj.GetComponent<BuffBarController>();
        buffBarController.buffDuration = buffDuration;
        meleeController.Damage = MeleeDamage;
        movespeed = 1.2f + (1.2f * (GlobalDataScript.SpeedBonus/100f));
    }

    void Update() {
        bool hasBuff = buffTimer > 0f;
        if (hasBuff) {
            buffTimer -= Time.deltaTime;
        } else {
            BuffBarObj.SetActive(false);
        }
        if (currentState == PlayerState.NONE) {
            HandleMovement();
            if (Input.GetButtonDown("Fire1")) {
                currentState = PlayerState.ATTACKING;
                if (hasBuff) {
                    animator.SetTrigger("attackBuffedTrigger");
                } else {
                    animator.SetTrigger("attackTrigger");
                }
                meleeController.HasHit = false;
                meleeController.CheckForCollisions = true;
                // Update damage in case this is a buffed attack.
                meleeController.Damage = (int)Mathf.Round(MeleeDamage * (hasBuff ? BUFF_MULTIPLIER : 1));
            } else if (Input.GetButtonDown("Fire2")) {
                currentState = PlayerState.ATTACKING;
                if (hasBuff) {
                    animator.SetTrigger("rangedAttackBuffedTrigger");
                } else {
                    animator.SetTrigger("rangedAttackTrigger");
                }
                StartCoroutine(RangedAttackSpawn(hasBuff));
            } else if (Input.GetButtonDown("Fire3") && nightController.CurrentHearts > 0) {
                currentState = PlayerState.SACRIFICING;
                animator.SetTrigger("sacrificeTrigger");
                StartCoroutine(SacrificeDelay());
            } else if (Input.GetButtonDown("Fire4")) {
                // If we're able to spend fire for buff, then do so and set buff timer.
                if (nightController.SpendFireForBuff()) {
                    currentState = PlayerState.BUFFING;
                    animator.SetTrigger("buffingTrigger");
                    StartCoroutine(BuffDelay());
                }
            }
        }
        if (currentState != PlayerState.STUNNED && isInvulnerable) {
            flashTimer += Time.deltaTime;
            int val = (int)(flashTimer * 10);
            int mod = val % 2;
            float prevA = spriteRenderer.color.a;
            if ((int)(flashTimer * 10) % 2 == 0) {
                spriteRenderer.color = invulnerableColor;
            } else {
                spriteRenderer.color = invulnerableColor2;
            }
            //Debug.Log($"val:{val}, mod:{mod}, a:{spriteRenderer.color.a}, prevA:{prevA}");
        }
    }

    public void DoneAttacking() {
        if (currentState == PlayerState.ATTACKING) {
            currentState = PlayerState.NONE;
            meleeController.HasHit = false;
            meleeController.CheckForCollisions = false;
        }
    }

    public void DoneSacrificing() {
        if (currentState == PlayerState.SACRIFICING) {
            currentState = PlayerState.NONE;
        }
    }

    public void DoneBuffing() {
        if (currentState == PlayerState.BUFFING) {
            currentState = PlayerState.NONE;
        }
    }

    private void HandleMovement() {
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");

        bool isMoving = Mathf.Abs(Horizontal) + Mathf.Abs(Vertical) > TOLERANCE;
        animator.SetBool("isMoving", isMoving);

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
        if (currentState != PlayerState.STUNNED && !isInvulnerable) {
            currentState = PlayerState.STUNNED;
            isInvulnerable = true;
            flashTimer = 0f;
            animator.SetTrigger("stunnedTrigger");
            StartCoroutine(StunTimeout());
            StartCoroutine(InvulnerabilityTimeout());
        }
    }

    private IEnumerator StunTimeout() {
        yield return new WaitForSeconds(stunDuration);
        currentState = PlayerState.NONE;
        animator.SetTrigger("unStunnedTrigger");
        spriteRenderer.color = regularColor;
    }

    private IEnumerator InvulnerabilityTimeout() {
        yield return new WaitForSeconds(invulnerableDuration);
        isInvulnerable = false;
        spriteRenderer.color = regularColor;
    }

    private IEnumerator RangedAttackSpawn(bool isBuffed) {
        yield return new WaitForSeconds(rangedAttackDelay);
        if (currentState != PlayerState.ATTACKING) {
            // Player was interrupted before they could complete ranged attack.
            yield break;
        }
        Vector3 position = transform.position;
        float deltaX = spriteRenderer.flipX ? -RANGED_X_OFFSET : RANGED_X_OFFSET;
        Vector3 newPosition = new Vector3(position.x + deltaX, position.y + RANGED_Y_OFFSET, position.z);
        GameObject rangedAttack = GameObject.Instantiate(isBuffed ? RangedAttackBuffedPrefab : RangedAttackPrefab, newPosition, Quaternion.identity);
        RangedAttackController controller = rangedAttack.GetComponent<RangedAttackController>();
        if (spriteRenderer.flipX) {
            rangedAttack.GetComponent<SpriteRenderer>().flipX = true;
            controller.MoveLeft = true;
        }
        int newdmg = (int)Mathf.Round(RangedDamage * (isBuffed ? BUFF_MULTIPLIER : 1));
        controller.Damage = (int)Mathf.Round(RangedDamage * (isBuffed ? BUFF_MULTIPLIER : 1));
    }

    private IEnumerator SacrificeDelay() {
        yield return new WaitForSeconds(sacrificeDelay);
        if (currentState != PlayerState.SACRIFICING) {
            // Player was interrupted before they could complete sacrifice.
            yield break;
        }
        nightController.SacrificeHeart();
    }

    private IEnumerator BuffDelay() {
        yield return new WaitForSeconds(buffDelay);
        if (currentState != PlayerState.BUFFING) {
            // Player was interrupted before they could complete buff.
            yield break;
        }
        BuffBarObj.SetActive(true);
        buffTimer = buffDuration;
        buffBarController.ResetBuff();
    }

    public enum PlayerState {
        NONE,
        ATTACKING,
        SACRIFICING,
        BUFFING,
        STUNNED
    };
}
