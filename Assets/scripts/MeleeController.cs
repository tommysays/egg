using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public int Damage;
    public bool HasHit = false;
    public bool CheckForCollisions = false;

    void Start()
    {
        
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (HasHit || !CheckForCollisions) {
            return;
        }
        if (collider.gameObject.tag == "Enemy") {
            EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
            HasHit = true;
            enemy.Health -= Damage;
        }
    }

}
