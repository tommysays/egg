﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayScene : MonoBehaviour
{
    public static int DayTime = 0;
    public static int TimeinaDay = 8;
    public static int MaxHealth = 100;
    public static int MaxAccelerant = 3;
    public static int AccelerantInHand = 2;
    public static int MeleeWeaponDmg = 5;
    public static int RangeWeaponDmg = 3;
    public static int SpeedBonus = 0;
    public Canvas[] canvases;

    public static float LERPtimer = 0.0f;
    public float LagTimeFade = 1.5f;
    public Color32 FadeCanvasStartingColor;
    public Color32 FadeCanvasEndingColor = new Color32(0, 0, 0, 255);

    public bool timeron = false;

    public void Reset()
    {
        DayTime = 0;
             this.canvases[3].GetComponentInChildren<Text>().text = "" + "\tMagic Circle Max Health: " + DayScene.MaxHealth + "\n\tPouches for Sacrifices: " + DayScene.MaxAccelerant +
             "\n\tSacrificial Critters: " + DayScene.AccelerantInHand + "\n\tMelee Attack: " + DayScene.MeleeWeaponDmg + "\n\tRanged Attack: " + DayScene.RangeWeaponDmg;
        DayScene.SpeedBonus = (TimeinaDay - DayScene.DayTime) * 10;
        this.canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn\n(Gain Speed " + DayScene.SpeedBonus + "% Bonus)";
    }


    void Start()
    {
        this.canvases[3].GetComponentInChildren<Text>().text = "" + "\tMagic Circle Max Health: " + DayScene.MaxHealth + "\n\tPouches for Sacrifices: " + DayScene.MaxAccelerant +
             "\n\tSacrificial Critters: " + DayScene.AccelerantInHand + "\n\tMelee Attack: " + DayScene.MeleeWeaponDmg + "\n\tRanged Attack: " + DayScene.RangeWeaponDmg;
        DayScene.SpeedBonus = (TimeinaDay - DayScene.DayTime) * 10;
        this.canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn\n(Gain Speed " + DayScene.SpeedBonus + "% Bonus)";
        FadeCanvasStartingColor = this.GetComponentInParent<Image>().color;
}

    void Update()
    {
        if (timeron)
        {
       
            LERPtimer += Time.deltaTime;
            if (LERPtimer >= LagTimeFade)
            {
                timeron = false;
                Fade();
            }
            else
            {
                canvases[6].GetComponent<Image>().color = Color32.Lerp(FadeCanvasStartingColor, FadeCanvasEndingColor, LERPtimer / LagTimeFade);
            }
        }
    }

    public void Fade()
    {
        //return to night scene
    }
}