﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightCanvasControls : MonoBehaviour
{
    public GameObject ControlsCanvas;
    public GameObject ControlsHelpText;

    // Start is called before the first frame update
    void Start()
    {
        ControlsCanvas.SetActive(false);
        ControlsHelpText.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Help"))
        {
            ControlsCanvas.SetActive(true);
            ControlsHelpText.SetActive(false);

        }
        if (Input.GetButtonUp("Help"))
        {
            ControlsCanvas.SetActive(false);
            ControlsHelpText.SetActive(true);
        }


    }
}
