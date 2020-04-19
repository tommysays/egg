using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMeterController : MonoBehaviour
{
    public GameObject fillObj;
    private RectTransform rect;
    private RectTransform fillRect;

    private float changeTimer = 0f;
    private float changeDuration = 0.25f;

    public float MaxValue;
    public float NextValue {
        get {
            return _nextValue;
        }
        set {
            if (value < 0) {
                value = 0;
            } else if (value > MaxValue) {
                value = MaxValue;
            }

            if (isChanging) {
                // If we're mid-change, we should set the current lerp as the last value.
                Debug.Log("Changing fire meter mid-change.");
                lastValue = Mathf.Lerp(lastValue, _nextValue, changeTimer / changeDuration);
            } else {
                lastValue = _nextValue;
            }

            _nextValue = value;
            changeTimer = 0f;
            isChanging = true;
        }
    }
    private float _nextValue;
    private float lastValue;

    private float maxWidth;
    private float maxHeight;
    private bool isChanging = false;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        fillRect = fillObj.GetComponent<RectTransform>();
        maxWidth = rect.sizeDelta.x;
        maxHeight = fillRect.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChanging) {
            float width = 0f;
            changeTimer += Time.deltaTime;
            if (changeTimer > changeDuration) {
                isChanging = false;
                width = maxWidth * (NextValue / MaxValue);
            } else {
                width = maxWidth * Mathf.Lerp(lastValue, NextValue, changeTimer / changeDuration) / MaxValue;
            }
            fillRect.sizeDelta = new Vector2(width, maxHeight);
        }
    }
}
