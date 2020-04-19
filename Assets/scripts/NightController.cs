using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightController : MonoBehaviour
{
    public GameObject FireObject;
    public GameObject SpiderPrefab;
    private FireController fireController;

    public int CurrentHearts;
    public int MaxHearts;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHearts = MaxHearts;
        fireController = FireObject.GetComponent<FireController>();
        SpawnSpider();
        SpawnSpider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnSpider() {
        float x = Random.value < 0.5f ? -3 : 3;
        float y = Random.Range(-2f, 2f);
        Vector3 spawnPosition = new Vector3(x, y, 0);
        GameObject spider = GameObject.Instantiate(SpiderPrefab, spawnPosition, Quaternion.identity);
        EnemyController enemyController = spider.GetComponent<EnemyController>();
        enemyController.nightController = this;
    }

    public void DamageFire(int damage) {
        fireController.CurrentValue -= damage;
    }

    public void SacrificeHeart() {
        if (CurrentHearts <= 0) {
            Debug.LogWarning($"Failed to sacrifice heart - not enough hearts! (${CurrentHearts} / ${MaxHearts}");
            return;
        }
        CurrentHearts--;
        fireController.CurrentValue += 20;
    }
}
