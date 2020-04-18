using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class DayChoice : MonoBehaviour, IPointerClickHandler
{
    public int ChoiceCost;
    public enum BonusEnum { Health, Sewing, Accelerant, Rest, Melee, Ranged}
    public BonusEnum Bonus;
    public DayScene dayScene;
    public Vector3 vectorformovingtext;


    void Start()
    {
        dayScene = FindObjectOfType<DayScene>();

    }

    void Update()
    {

    }


    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if ((DayScene.DayTime + ChoiceCost <= DayScene.TimeinaDay) && (dayScene.canvases[5].GetComponentInChildren<SunScript>().Moving == false))
        {
            bool success = false;
            string movingstring = "";
            if (Bonus == BonusEnum.Health)
            {
                DayScene.MaxHealth += 10;
                success = true;
                movingstring = "+10 Magic Fire Shield";
                vectorformovingtext = dayScene.canvases[0].transform.position;
                vectorformovingtext.x = dayScene.canvases[0].transform.position.x+200;
                vectorformovingtext.y = dayScene.canvases[0].transform.position.y;

            }
            else if (Bonus == BonusEnum.Sewing)
            {
                DayScene.MaxAccelerant += 1;
                success = true;
                movingstring = "+1 Heart Slot";
                vectorformovingtext = dayScene.canvases[1].transform.position;
                vectorformovingtext.x = dayScene.canvases[1].transform.position.x+200;
                vectorformovingtext.y = dayScene.canvases[1].transform.position.y;
            }
            else if ((Bonus == BonusEnum.Accelerant) && (DayScene.MaxAccelerant > DayScene.AccelerantInHand))
            {
                DayScene.AccelerantInHand += 1;
                success = true;
                movingstring = "+1 Sacrifical Heart";
                vectorformovingtext = dayScene.canvases[2].transform.position;
                vectorformovingtext.x = dayScene.canvases[2].transform.position.x+200;
                vectorformovingtext.y = dayScene.canvases[2].transform.position.y;
            }
            else if (Bonus == BonusEnum.Melee)
            {
                DayScene.MeleeWeaponDmg += 3;
                movingstring = "+3 Melee Damage";

                success = true;
                vectorformovingtext = dayScene.canvases[7].transform.position;
                vectorformovingtext.x = dayScene.canvases[7].transform.position.x + 200;
                vectorformovingtext.y = dayScene.canvases[7].transform.position.y;
            }
            else if (Bonus == BonusEnum.Ranged)
            {
                DayScene.RangeWeaponDmg += 2;
                movingstring = "+2 Ranged Damage";

                success = true;
                vectorformovingtext = dayScene.canvases[8].transform.position;
                vectorformovingtext.x = dayScene.canvases[8].transform.position.x + 200;
                vectorformovingtext.y = dayScene.canvases[8].transform.position.y;


            }
            else if (Bonus == BonusEnum.Rest)
            {
                success = true;
                ChoiceCost = (DayScene.TimeinaDay - DayScene.DayTime);
                movingstring = "+" + ((DayScene.TimeinaDay - DayScene.DayTime) * 10) + "% One Night Speed Bonus";
                DayScene.SpeedBonus = (DayScene.TimeinaDay - DayScene.DayTime) * 10;
                vectorformovingtext = dayScene.canvases[4].transform.position;
                vectorformovingtext.x = dayScene.canvases[4].transform.position.x;
                vectorformovingtext.y = dayScene.canvases[4].transform.position.y;
            }
            //Play animation?
            if (success)
            {
                DayScene.DayTime += ChoiceCost;
                dayScene.canvases[5].GetComponentInChildren<SunScript>().Move(ChoiceCost);
                FindObjectOfType<MovingTextScript>().GetComponent<Text>().text = movingstring;
                FindObjectOfType<MovingTextScript>().Move(vectorformovingtext, 1.0f, new Vector3(dayScene.canvases[3].transform.position.x, dayScene.canvases[3].transform.position.y + 100, dayScene.canvases[3].transform.position.z));
                
                dayScene.canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn";
                if (DayScene.DayTime < DayScene.TimeinaDay)
                {
                    dayScene.canvases[4].GetComponentInChildren<Text>().text = dayScene.canvases[4].GetComponentInChildren<Text>().text + "\n(Gain Temporary " + ((DayScene.TimeinaDay - DayScene.DayTime) * 10) + "% Speed Bonus)";
                }
            }
            
        }
    }


}
