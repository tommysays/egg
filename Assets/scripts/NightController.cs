using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightController : MonoBehaviour
{
    public GameObject PlayerObj;
    public GameObject FireObj;
    public GameObject[] EnemyPrefabs;
    public GameObject FireMeterObj;
    private FireController fireController;
    private FireMeterController fireMeterController;
    public TextAsset[] Levels;
    public Text HeartText;

    public int MaxFireValue;

    public int CurrentHearts {
        get {
            return _currentHearts;
        }
        set {
            if (value < 0) {
                value = 0;
            } else if (value > MaxHearts) {
                value = MaxHearts;
            }
            _currentHearts = value;
            HeartText.text = value.ToString();
        }
    }
    private int _currentHearts;
    public int MaxHearts;
    private int HeartRecoveryAmount = 20;
    public int BuffCost = 15;

    public int FireValue {
        get {
            return _fireValue;
        }
        set {
            if (hasWon || hasLost) {
                return;
            }
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
        int id = enemyPayload.EnemyId;
        GameObject enemyPrefab = EnemyPrefabs[id];
        // This list determines which enemies can chase player.
        bool attachPlayerObject = id == 0 || id == 1;
        foreach (Vector2 spawn in enemyPayload.Spawns) {
            StartCoroutine(LaunchEnemy(enemyPrefab, (int)spawn.x, (int)spawn.y, attachPlayerObject));
        }
    }

    private IEnumerator LaunchEnemy(GameObject prefab, int delay, int count, bool attachPlayerObject) {
        yield return new WaitForSeconds(delay);

        // Don't bother spawning if the player already lost.
        if (hasLost) {
            yield break;
        }

        while (count --> 0) {
            SpawnFromSide(prefab, attachPlayerObject);
        }
    }

    private void SpawnFromSide(GameObject prefab, bool attachPlayerObject) {
        // TODO make it only spawn from the edges.
        float x = Random.value < 0.5f ? -3 : 3;
        float y = Random.Range(-2f, 2f);
        Vector3 spawnPosition = new Vector3(x, y, y);
        GameObject enemy = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.nightController = this;
        if (attachPlayerObject) {
            enemyController.PlayerObj = PlayerObj;
        }
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
