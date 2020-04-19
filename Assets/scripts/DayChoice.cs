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
            dayScene.MakeSelection(this.GetComponent<DayChoice>());


               
        }
    }


}
