using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of the fire value, and updates the sprite accordingly.
/// </summary>
public class FireController : MonoBehaviour
{
    public GameObject[] FireObjs;
    public int MaxValue;
    public int CurrentValue {
        get {
            return _currentValue;
        }
        set {
            if (value < 0) {
                // Game over is handled in NightController.
                value = 0;
            }
            UpdateState(value);
            _currentValue = value;
        }
    }
    private int _currentValue;

    private SpriteRenderer[] spriteRenderers;

    void Start() {
        spriteRenderers = new SpriteRenderer[FireObjs.Length];
        for (int i = 0; i < FireObjs.Length; i++) {
            spriteRenderers[i] = FireObjs[i].GetComponent<SpriteRenderer>();
        }
        CurrentValue = MaxValue;
    }

    private void UpdateState(int value)
    {
        if (spriteRenderers == null) {
            return;;
        }
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            Color color = spriteRenderer.color;
            color.a = (float)value / MaxValue;
            spriteRenderer.color = color;
        }
    }
}
