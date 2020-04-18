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
    private const int stunDuration = 2000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == PlayerState.NONE) {
            HandleMovement();
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
