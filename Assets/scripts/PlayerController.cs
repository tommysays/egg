using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerState currentState = PlayerState.NONE;

    private float movespeed = 1.2f;
    private const float minX = -3f,
                        minY = -2f,
                        maxX = 3f,
                        maxY = 2f;
    private const float stunDuration = 2f;
    private const float TOLERANCE = 0.0001f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (currentState == PlayerState.NONE) {
            HandleMovement();
            if (Input.GetButtonDown("Fire1")) {
                currentState = PlayerState.ATTACKING;
                animator.SetTrigger("attackTrigger");
            }
        }
    }

    public void DoneAttacking() {
        if (currentState == PlayerState.ATTACKING) {
            currentState = PlayerState.NONE;
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
        transform.position = new Vector3(newX, newY, currentPosition.z);
        if (Horizontal < -TOLERANCE) {
            // Moving left, so make sure sprite is flipped.
            if (!spriteRenderer.flipX) {
                spriteRenderer.flipX = true;
            }
        } else if (Horizontal > TOLERANCE) {
            if (spriteRenderer.flipX) {
                spriteRenderer.flipX = false;
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

    public enum PlayerState {
        NONE,
        ATTACKING,
        STUNNED
    };
}
