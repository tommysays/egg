﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBreakScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void BreakThatEgg()
    {
        NightController nightController = FindObjectOfType<NightController>();
        DestroyObject(nightController.Egg);
    }
}
