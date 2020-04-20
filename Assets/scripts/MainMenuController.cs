using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    public GameObject sceneContainer;
    public GameObject MainMenuPanel;
    public GameObject ControlsPanel;

    private RectTransform mainMenuRectTransform;
    private RectTransform controlsRectTransform;

    public GameObject FadeBlackPanel;
    private CanvasGroup fadeBlackGroup;

    private bool playClicked = false;

    private float transitionTimer = 0f;
    private const float TRANSITION_DURATION = 0.5f;
    private const float UI_TRANSITION_X = 700f;
    private bool isTransitioning = false;
    private bool invert = true;
    private float mainMenuStartingX;
    private float controlsStartingX;
    private float mainMenuEndingX;
    private float controlsEndingX;
    private float sceneStartingX = 0f;
    private float sceneEndingX = -2f;

    private bool shouldFade = false;
    private float fadeTimer = 0f;
    private const float FADE_DURATION = 1.5f;

    void Start() {
        fadeBlackGroup = FadeBlackPanel.GetComponent<CanvasGroup>();
        mainMenuRectTransform = MainMenuPanel.GetComponent<RectTransform>();
        controlsRectTransform = ControlsPanel.GetComponent<RectTransform>();
        mainMenuStartingX = mainMenuRectTransform.localPosition.x;
        mainMenuEndingX = mainMenuStartingX - UI_TRANSITION_X;
        controlsStartingX = controlsRectTransform.localPosition.x;
        controlsEndingX = controlsStartingX - UI_TRANSITION_X;
    }

    void Update() {
        if (Input.GetButtonDown("Help")) {
            ToggleTransition();
        } else if (Input.GetButtonDown("Submit")) {
            HandlePlayClicked();
        }

        if (isTransitioning) {
            transitionTimer += Time.deltaTime;
            float ratio = transitionTimer / TRANSITION_DURATION;
            if (transitionTimer >= TRANSITION_DURATION) {
                ratio = 1f;
                transitionTimer = 0f;
                isTransitioning = false;
            }
            if (invert) {
                Vector3 position = mainMenuRectTransform.localPosition;
                position.x = Mathf.SmoothStep(mainMenuEndingX, mainMenuStartingX, ratio);
                mainMenuRectTransform.localPosition = position;
                position.x = Mathf.SmoothStep(controlsEndingX, controlsStartingX, ratio);
                controlsRectTransform.localPosition = position;

                position = sceneContainer.transform.position;
                position.x = Mathf.SmoothStep(sceneEndingX, sceneStartingX, ratio);
                sceneContainer.transform.position = position;
            } else {
                Vector3 position = mainMenuRectTransform.localPosition;
                position.x = Mathf.SmoothStep(mainMenuStartingX, mainMenuEndingX, ratio);
                mainMenuRectTransform.localPosition = position;
                position.x = Mathf.SmoothStep(controlsStartingX, controlsEndingX, ratio);
                controlsRectTransform.localPosition = position;

                position = sceneContainer.transform.position;
                position.x = Mathf.SmoothStep(sceneStartingX, sceneEndingX, ratio);
                sceneContainer.transform.position = position;
            }
        }
        if (shouldFade) {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > FADE_DURATION) {
                fadeBlackGroup.alpha = 1f;
                if (fadeTimer > FADE_DURATION * 2) {
                    shouldFade = false;
                    StartCoroutine(LoadDayScene());
                }
            } else {
                fadeBlackGroup.alpha = fadeTimer / FADE_DURATION;
            }
        }
    }

    public void ToggleTransition() {
        // Don't allow toggle if already in transition.
        if (isTransitioning) {
            return;
        }
        isTransitioning = true;
        invert = !invert;
    }

    public void HandlePlayClicked() {
        if (playClicked) {
            return;
        }
        playClicked = true;
        shouldFade = true;
        fadeTimer = 0f;
        FadeBlackPanel.SetActive(true);
    }
    private IEnumerator LoadDayScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("NightScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
