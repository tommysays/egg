using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightController : MonoBehaviour
{
    public GameObject FireObj;
    public GameObject[] EnemyPrefabs;
    public GameObject FireMeterObj;
    private FireController fireController;

    public int MaxFireValue;

    public int CurrentHearts;
    public int MaxHearts;
    private int HeartRecoveryAmount = 20;
    public int BuffCost = 15;

    public int FireValue {
        get {
            return _fireValue;
        }
        set {
            if (value > MaxFireValue) {
                value = MaxFireValue;
            } else if (value < 0) {
                value = 0;
            }
            fireController.CurrentValue = value;
            fireMeterController.NextValue = value;
            _fireValue = value;
        }
    }
    private int _fireValue;

    private FireMeterController fireMeterController;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHearts = MaxHearts;
        fireController = FireObj.GetComponent<FireController>();
        fireMeterController = FireMeterObj.GetComponent<FireMeterController>();
        fireMeterController.MaxValue = MaxFireValue;
        FireValue = MaxFireValue;
        SpawnFromSide(EnemyPrefabs[0]);
        SpawnFromSide(EnemyPrefabs[0]);
        SpawnFromSide(EnemyPrefabs[1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnFromSide(GameObject prefab) {
        float x = Random.value < 0.5f ? -3 : 3;
        float y = Random.Range(-2f, 2f);
        Vector3 spawnPosition = new Vector3(x, y, y);
        GameObject spider = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
        EnemyController enemyController = spider.GetComponent<EnemyController>();
        enemyController.nightController = this;
    }

    public void DamageFire(int damage) {
        FireValue -= damage;
    }

    public void SacrificeHeart() {
        if (CurrentHearts <= 0) {
            Debug.LogWarning($"Failed to sacrifice heart - not enough hearts! (${CurrentHearts} / ${MaxHearts}");
            return;
        }
        CurrentHearts--;
        FireValue += HeartRecoveryAmount;
    }

    public bool SpendFireForBuff() {
        if (FireValue > BuffCost) {
            FireValue -= BuffCost;
            return true;
        }
        return false;
    }
}
