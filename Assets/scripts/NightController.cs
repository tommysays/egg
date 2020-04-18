using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightController : MonoBehaviour
{
    public GameObject FireObject;
    public GameObject SpiderPrefab;
    private FireController fireController;

    // Start is called before the first frame update
    void Start()
    {
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
}
