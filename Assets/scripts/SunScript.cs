using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunScript : MonoBehaviour
{

    public bool Moving = false;
    public Vector3 StartingPosition;
    public Vector3 EndingPosition;
    public float LerpTime = 0.0f;
    public float LagTime = 1.5f;
    DayScene dayScene;

    void Start()
    {
        dayScene = FindObjectOfType<DayScene>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Moving)
        {
            LerpTime += Time.deltaTime;
            if (LerpTime >= LagTime)
            {
                Moving = false;
                transform.position = EndingPosition;
                dayScene.canvases[3].GetComponentInChildren<Text>().text = "\tMagic Circle Max Health: " + DayScene.MaxHealth + "\n\tHeart Pouches: " + DayScene.MaxAccelerant +
                    "\n\tSacrificial Hearts: " + DayScene.AccelerantInHand + "\n\tMelee Attack: " + DayScene.MeleeWeaponDmg + "\n\tRanged Attack: " + DayScene.RangeWeaponDmg;
                if (DayScene.DayTime >= DayScene.TimeinaDay)
                {
                    dayScene.timeron = true;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(StartingPosition, EndingPosition, LerpTime / LagTime);
            }
        }
    }

    public void Move(int x)
    {
        LagTime = (x * 0.4f);
        StartingPosition = this.transform.position;
        EndingPosition = new Vector3(StartingPosition.x + (100 * x), this.transform.position.y, this.transform.position.z);
        Moving = true;
        LerpTime = 0.0f;
    }
}
