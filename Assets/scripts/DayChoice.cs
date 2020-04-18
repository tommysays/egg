using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class DayChoice : MonoBehaviour, IPointerClickHandler
{
    public int ChoiceCost;
    public enum BonusEnum { Health, Sewing, Accelerant, Rest}
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
            if (Bonus == BonusEnum.Health)
            {
                DayScene.MaxHealth += 10;
                success = true;
                vectorformovingtext = dayScene.canvases[0].transform.position;
                vectorformovingtext.x = dayScene.canvases[0].transform.position.x+200;
                vectorformovingtext.y = dayScene.canvases[0].transform.position.y;

            }
            else if (Bonus == BonusEnum.Sewing)
            {
                DayScene.MaxAccelerant += 1;
                success = true;
                vectorformovingtext = dayScene.canvases[1].transform.position;
                vectorformovingtext.x = dayScene.canvases[1].transform.position.x+200;
                vectorformovingtext.y = dayScene.canvases[1].transform.position.y;
            }
            else if ((Bonus == BonusEnum.Accelerant) && (DayScene.MaxAccelerant > DayScene.AccelerantInHand))
            {
                DayScene.AccelerantInHand += 1;
                success = true;
                vectorformovingtext = dayScene.canvases[2].transform.position;
                vectorformovingtext.x = dayScene.canvases[2].transform.position.x+200;
                vectorformovingtext.y = dayScene.canvases[2].transform.position.y;
            }
            else if (Bonus == BonusEnum.Rest)
            {
                success = true;
                
            }
            //Play animation?
            if (success)
            {
                
                DayScene.DayTime += ChoiceCost;
                DayScene.SpeedBonus = (DayScene.TimeinaDay - DayScene.DayTime) * 10;
                dayScene.canvases[5].GetComponentInChildren<SunScript>().Move(ChoiceCost);
                if ((Bonus != BonusEnum.Rest))
                {
                    FindObjectOfType<MovingTextScript>().Move(vectorformovingtext, ChoiceCost * 0.3f, new Vector3(dayScene.canvases[3].transform.position.x + 50, dayScene.canvases[4].transform.position.y + 50, dayScene.canvases[4].transform.position.z));
                }
                dayScene.canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn\n(Gain Speed " + DayScene.SpeedBonus + "% Bonus)";
            }
            
        }
    }


}
