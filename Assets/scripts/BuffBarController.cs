using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is all some hard-coded bullshit that you should never do.
/// </summary>
public class BuffBarController : MonoBehaviour
{
    public GameObject[] CellObjs;
    public float buffDuration;
    private float buffTimer;
    private SpriteRenderer[] cellRenderers;

    void Start() {
        cellRenderers = new SpriteRenderer[10];
        for (int i = 0; i < 10; i++) {
            cellRenderers[i] = CellObjs[i].GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (buffTimer > 0f) {
            buffTimer -= Time.deltaTime;
            float ratio = buffTimer / buffDuration;
            int i = 0;
            // Set all preceding cells to full color.
            for (; i < ratio * 10 - 1; i++) {
                Color color = cellRenderers[i].color;
                color.a = 1f;
                cellRenderers[i].color = color;
            }
            // Set last visible cell to some amount of transparent.
            if (i < 10) {
                Color color = cellRenderers[i].color;
                color.a = ratio * 10f - (float)i;
                cellRenderers[i].color = color;
            }
            // Set remaining cells to transparent.
            for (i++; i < 10; i++) {
                Color color = cellRenderers[i].color;
                color.a = 0f;
                cellRenderers[i].color = color;
            }
        }
    }

    public void ResetBuff() {
        buffTimer = buffDuration;
    }
}
