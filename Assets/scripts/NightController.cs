using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightController : MonoBehaviour
{
    public GameObject FireObj;
    public GameObject[] EnemyPrefabs;
    public GameObject FireMeterObj;
    private FireController fireController;
    private FireMeterController fireMeterController;
    public TextAsset[] Levels;

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
                hasLost = true;
            }
            fireController.CurrentValue = value;
            fireMeterController.NextValue = value;
            _fireValue = value;
        }
    }
    private int _fireValue;

    // Only enabled when the last monster spawns.
    public bool canWin = false;
    // Prevents the win from triggering multiple times.
    public bool hasWon = false;
    public bool hasLost = false;
    private float winDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHearts = MaxHearts;
        fireController = FireObj.GetComponent<FireController>();
        fireMeterController = FireMeterObj.GetComponent<FireMeterController>();
        fireMeterController.MaxValue = MaxFireValue;
        FireValue = MaxFireValue;
        // TODO Use night number instead of 0.
        Level level = LevelLoader.LoadLevel(Levels[0]);
        FindLastEnemySpawnTime(level);
        LaunchLevel(level);
    }

    #region Win condition

    private void FindLastEnemySpawnTime(Level level) {
        float maxSpawnTime = 0f;
        foreach(Wave wave in level.Waves) {
            float waveStart = wave.StartTime;
            foreach(EnemyPayload enemy in wave.Enemies) {
                foreach(Vector2 spawn in enemy.Spawns) {
                    if (waveStart + spawn.x > maxSpawnTime) {
                        maxSpawnTime = waveStart + spawn.x;
                    }
                }
            }
        }
        StartCoroutine(EnableWinCondition(maxSpawnTime));
    }

    private IEnumerator EnableWinCondition(float maxSpawnTime) {
        yield return new WaitForSeconds(maxSpawnTime);
        canWin = true;
    }

    public void CheckForWinCondition() {
        if (!canWin || hasWon || hasLost) {
            return;
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies) {
            EnemyController controller = enemyObj.GetComponent<EnemyController>();
            if (controller.currentState != EnemyController.EnemyState.DYING) {
                return;
            }
        }
        hasWon = true;
        StartCoroutine(DelayedWinScreen());
    }

    private IEnumerator DelayedWinScreen() {
        yield return new WaitForSeconds(winDelay);
        // TODO Do things on win, like show a win screen.
        Debug.Log("Player won the night!");
    }

    #endregion

    #region Level loading and enemy spawning

    private void LaunchLevel(Level level) {
        foreach(Wave wave in level.Waves) {
            StartCoroutine(LaunchWave(wave));
        }
    }

    private IEnumerator LaunchWave(Wave wave) {
        yield return new WaitForSeconds(wave.StartTime);
        foreach (EnemyPayload payload in wave.Enemies) {
            LaunchEnemyPayload(payload);
        }
    }

    private void LaunchEnemyPayload(EnemyPayload enemyPayload) {
        GameObject enemyPrefab = EnemyPrefabs[enemyPayload.EnemyId];
        foreach (Vector2 spawn in enemyPayload.Spawns) {
            StartCoroutine(LaunchEnemy(enemyPrefab, (int)spawn.x, (int)spawn.y));
        }
    }

    private IEnumerator LaunchEnemy(GameObject prefab, int delay, int count) {
        yield return new WaitForSeconds(delay);

        // Don't bother spawning if the player already lost.
        if (hasLost) {
            yield break;
        }

        while (count --> 0) {
            SpawnFromSide(prefab);
        }
    }

    private void SpawnFromSide(GameObject prefab) {
        // TODO make it only spawn from the edges.
        float x = Random.value < 0.5f ? -3 : 3;
        float y = Random.Range(-2f, 2f);
        Vector3 spawnPosition = new Vector3(x, y, y);
        GameObject spider = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
        EnemyController enemyController = spider.GetComponent<EnemyController>();
        enemyController.nightController = this;
    }

    #endregion

    #region Fire-related Actions

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

    #endregion
}
