using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : MonoBehaviour
{
    public bool MoveLeft = false;
    public int Damage;

    private const float MOVESPEED = 2.0f;
    // How long the projectile stays alive.
    private const float LIFESPAN = 2f;
    private float currentLife = 0f;


    void Update() {
        currentLife += Time.deltaTime;
        if (currentLife > LIFESPAN) {
            Destroy(gameObject);
            return;
        }
        float deltaX = MOVESPEED * Time.deltaTime;
        transform.Translate(MoveLeft ? -deltaX : deltaX, 0, 0);
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (collider.gameObject.tag == "Enemy") {
            EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
            enemy.Health -= Damage;
            Destroy(gameObject);
    }   }
}
