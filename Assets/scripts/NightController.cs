using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NightController : MonoBehaviour
{
    public GameObject PlayerObj;
    public GameObject FireObj;
    public GameObject[] EnemyPrefabs;
    public GameObject FireMeterObj;
    public GameObject EggsplosionObj;
    public GameObject Egg;
    public GameObject Hatchling;
    public Sprite[] EggSprits;
    private FireController fireController;
    private FireMeterController fireMeterController;
    public TextAsset[] Levels;
    public Text HeartText;
    public GameObject FadeWhitePanel;
    public GameObject FadeWhiteTextPanel;
    public GameObject FadeBlackPanel;
    public GameObject FadeBlackTextPanel;
    private CanvasGroup fadeWhiteGroup;
    private CanvasGroup fadeWhiteTextGroup;
    private CanvasGroup fadeBlackGroup;
    private CanvasGroup fadeBlackTextGroup;

    public AudioClip audioBossMusic;
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
            if (currentState != GameState.NONE) {
                return;
            }
            if (value > MaxFireValue) {
                value = MaxFireValue;
            } else if (value < 0) {
                value = 0;
                currentState = GameState.LOST;
                //create eggsplosion
                GameObject eggsplosion = GameObject.Instantiate(EggsplosionObj, FireObj.transform.position, Quaternion.identity);
                StartCoroutine(DestroyEgg(.2f));

                shouldFade = true;
                FadeBlackPanel.SetActive(true);
            }
            fireController.CurrentValue = value;
            fireMeterController.NextValue = value;
            _fireValue = value;
        }
    }
    private int _fireValue;

    // Only enabled when the last monster spawns.
    public bool canWin = false;
    public GameState currentState = GameState.NONE;
    // It's jarring if you win right after defeating the last enemy, so we wait a bit before letting you know.
    private float winDelay = 1.5f;

    private bool shouldFade = false;
    private float fadeTimer = 0f;
    private const float FADE_DURATION = 1.5f;

    // Start is called before the first frame update
    void Start() {
        MaxHearts = GlobalDataScript.MaxAccelerant;
        CurrentHearts = GlobalDataScript.AccelerantInHand;
        MaxFireValue = GlobalDataScript.MaxHealth;
        PlayerObj.GetComponent<PlayerController>().MeleeDamage = GlobalDataScript.MeleeWeaponDmg;
        PlayerObj.GetComponent<PlayerController>().RangedDamage = GlobalDataScript.RangeWeaponDmg;
        //speed is handled in playercontroller start

        fadeWhiteGroup = FadeWhitePanel.GetComponent<CanvasGroup>();
        fadeWhiteTextGroup = FadeWhiteTextPanel.GetComponent<CanvasGroup>();
        fadeBlackGroup = FadeBlackPanel.GetComponent<CanvasGroup>();
        fadeBlackTextGroup = FadeBlackTextPanel.GetComponent<CanvasGroup>();

        fireController = FireObj.GetComponent<FireController>();
        fireController.MaxValue = MaxFireValue;
        fireMeterController = FireMeterObj.GetComponent<FireMeterController>();
        fireMeterController.MaxValue = MaxFireValue;
        FireValue = MaxFireValue;
        if (GlobalDataScript.Day >= 4) {
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = audioBossMusic;
            audio.Play();
        }
        Level level = LevelLoader.LoadLevel(Levels[GlobalDataScript.Day]);
        FindLastEnemySpawnTime(level);
        LaunchLevel(level);
        Egg.GetComponent<SpriteRenderer>().sprite = EggSprits[GlobalDataScript.Day];
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

    private IEnumerator DestroyEgg(float DestoryDelay)
    {
        yield return new WaitForSeconds(DestoryDelay);
        Destroy(Egg);
    }

    private IEnumerator EnableWinCondition(float maxSpawnTime) {
        yield return new WaitForSeconds(maxSpawnTime);
        canWin = true;
    }

    public void CheckForWinCondition() {
        if (!canWin || currentState != GameState.NONE) {
            return;
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies) {
            EnemyController controller = enemyObj.GetComponent<EnemyController>();
            if (controller.currentState != EnemyController.EnemyState.DYING) {
                return;
            }
        }
        currentState = GameState.WON;

        if (GlobalDataScript.Day >= 4)
        {
                      
            GameObject.Instantiate(Hatchling, FireObj.transform.position, Quaternion.identity);
            Destroy(Egg);
            winDelay = 10f;
            ToMainMenu();
        }
        else
        {
            StartCoroutine(DelayedWinScreen());
        }
    }

    private IEnumerator DelayedWinScreen() {

        
        yield return new WaitForSeconds(winDelay);
        GlobalDataScript.MaxAccelerant = MaxHearts;
        GlobalDataScript.AccelerantInHand = CurrentHearts;
        GlobalDataScript.MaxHealth = MaxFireValue;
        GlobalDataScript.MeleeWeaponDmg = PlayerObj.GetComponent<PlayerController>().MeleeDamage;
        GlobalDataScript.RangeWeaponDmg = PlayerObj.GetComponent<PlayerController>().RangedDamage;
        GlobalDataScript.Day++;
        shouldFade = true;
        FadeWhitePanel.SetActive(true);
    }

    private IEnumerator LoadYourAsyncScene()
    {
        
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DayScene");
     

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ToMainMenuButtonHandler() {
        StartCoroutine(ToMainMenu());
    }

    private IEnumerator ToMainMenu() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MenuScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void Update() {
        if (shouldFade) {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > FADE_DURATION) {
                if (currentState == GameState.WON) {
                    fadeWhiteGroup.alpha = 1f;
                    if (fadeTimer > FADE_DURATION * 2) {
                        shouldFade = false;
                        fadeWhiteTextGroup.alpha = 0;
                        
                            StartCoroutine(LoadYourAsyncScene());
                       
                    } else {
                        fadeWhiteTextGroup.alpha = 2f - fadeTimer / FADE_DURATION;
                    }
                } else {
                    fadeBlackGroup.alpha = 1f;
                    if (fadeTimer > FADE_DURATION * 2) {
                        shouldFade = false;
                        fadeBlackTextGroup.alpha = 1f;
                    } else {
                        fadeBlackTextGroup.alpha = fadeTimer / FADE_DURATION - 1f;
                    }
                }
            } else {
                if (currentState == GameState.WON) {
                    fadeWhiteGroup.alpha = fadeTimer / FADE_DURATION;
                } else {
                    fadeBlackGroup.alpha = fadeTimer / FADE_DURATION;
                }
            }
        }
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
        bool isBoss = id == 6;
        foreach (Vector2 spawn in enemyPayload.Spawns) {
            StartCoroutine(LaunchEnemy(enemyPrefab, (int)spawn.x, (int)spawn.y, attachPlayerObject, isBoss));
        }
    }

    private IEnumerator LaunchEnemy(GameObject prefab, int delay, int count, bool attachPlayerObject, bool isBoss) {
        yield return new WaitForSeconds(delay);

        // Don't bother spawning if the player already lost.
        if (currentState == GameState.LOST) {
            yield break;
        }

        while (count --> 0) {
            SpawnFromSide(prefab, attachPlayerObject, isBoss);
        }
    }

    private void SpawnFromSide(GameObject prefab, bool attachPlayerObject, bool isBoss) {
        float x = Random.value < 0.5f ? -3 : 3;
        float y = Random.Range(-2f, 2f);
        if (isBoss) {
            x = Random.value < 0.5f ? -2.5f : 2.5f;
            y = Random.Range(-1.5f, 1.5f);
        }
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

    public enum GameState {
        NONE,
        WON,
        LOST
    }
}
