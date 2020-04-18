using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of the fire value, and updates the sprite accordingly.
/// </summary>
public class FireController : MonoBehaviour
{
    public int MaxValue;
    public int CurrentValue {
        get {
            return _currentValue;
        }
        set {
            if (value < 0) {
                // TODO Trigger game over.
                Debug.Log("Fire was put out! Game over?");
                value = 0;
            }
            UpdateState(value);
            _currentValue = value;
        }
    }
    private int _currentValue;

    public Sprite[] Sprites;

    private FireState CurrentState {
        get {
            return _currentState;
        }
        set {
            if (value != _currentState) {
                UpdateSprite(value);
            }
            _currentState = value;
        }
    }
    private FireState _currentState = FireState.FULL;

    private SpriteRenderer spriteRenderer;

    void Start() {
        CurrentValue = MaxValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void UpdateState(int value)
    {
        if (value <= MaxValue * 0.10) {
            CurrentState = FireState.CRITICAL;
        } else if (value <= MaxValue * 0.35) {
            CurrentState = FireState.LOW;
        } else if (value <= MaxValue * 0.75) {
            CurrentState = FireState.MID;
        } else if (value <= MaxValue * 0.95) {
            CurrentState = FireState.HIGH;
        } else {
            CurrentState = FireState.FULL;
        }
    }

    private void UpdateSprite(FireState state) {
        switch (state) {
            case FireState.FULL:
                spriteRenderer.sprite = Sprites[0];
                break;
            case FireState.HIGH:
                spriteRenderer.sprite = Sprites[1];
                break;
            case FireState.MID:
                spriteRenderer.sprite = Sprites[2];
                break;
            case FireState.LOW:
                spriteRenderer.sprite = Sprites[3];
                break;
            case FireState.CRITICAL:
                spriteRenderer.sprite = Sprites[4];
                break;
        }
    }

    public enum FireState {
        FULL,
        HIGH,
        MID,
        LOW,
        CRITICAL
    }
}
