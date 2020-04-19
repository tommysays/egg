using System.Collections;
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
    public Text[] texts;
    public static int Day = 0;

    public GameObject SelectPanel;
    public int SelectedPanelNumber = 0;
    public bool SelectPossible = false;
    public enum SelectStates { FadingIn, Displaying, FadingOut }
    SelectStates SelectState = SelectStates.Displaying;
    public float SelectPanelTimer = 0.0f;
    public float SelectPanelFadeTime = 0.5f;
    public float SelectPanelDisplayTime = 0.5f;
    public Color32 StartingSelectColor = new Color32(255, 255, 255, 60);
    public Color32 EndingSelectColor = new Color32(255, 255, 255, 0);
    public GameObject SelectorIcon;

    public Vector3 vectorformovingtext;

    public enum CanvasHoverStates { GoingUp, GoingDown, Off }

    public CanvasHoverStates CanvasHoverState;
    public static float CanvasHoverTimer = 0.0f;
    public float LagCanvasHoverTimer = .4f;
    public float CanvasHoverStartingY = 0.0f;
    public float CanvasHoverEndingY = 0.0f;


    public static float LERPtimer = 0.0f;
    public float LagTimeFade = 3.5f;
    public Color32 FadeCanvasStartingColor;
    public Color32 FadeCanvasEndingColor = new Color32(0, 0, 0, 255);
    public Color32 StartBackgroundColor = new Color32(129, 218, 255, 251);
    public Color32 EndBackgroundColor = new Color32(45, 79, 119, 255);
    public GameObject BackUpPanel;




    public bool timeron = false;

    public void Reset()
    {
        DayTime = 0;
             this.canvases[3].GetComponentInChildren<Text>().text = "" + "\tMagic Circle Max Health: " + DayScene.MaxHealth + "\n\tHeart Pouches: " + DayScene.MaxAccelerant +
             "\n\tSacrificial Hearts: " + DayScene.AccelerantInHand + "\n\tMelee Attack: " + DayScene.MeleeWeaponDmg + "\n\tRanged Attack: " + DayScene.RangeWeaponDmg;
        SpeedBonus = (TimeinaDay - DayScene.DayTime) * 10;
        this.canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn\n(Gain Temporary " + DayScene.SpeedBonus + "% Speed Bonus)";
        SelectPanel.SetActive(true);
        SelectorIcon.SetActive(true);
        texts[0].text = " Day " + Day + " -- " + (TimeinaDay - DayTime) + " Hours Until Nightfall"; ;
        SelectPossible = true;
        SelectState = SelectStates.Displaying;
        SelectedPanelNumber = 0;
        SelectPanel.transform.localPosition = canvases[0].transform.localPosition;
        BackUpPanel.transform.localPosition = new Vector3(canvases[0].transform.localPosition.x, canvases[0].transform.localPosition.y - (BackUpPanel.GetComponent<RectTransform>().rect.height/2) -(canvases[0].GetComponent<RectTransform>().rect.height / 4), canvases[0].transform.localPosition.z);
        CanvasHoverState = CanvasHoverStates.GoingUp;
        CanvasHoverStartingY = canvases[0].transform.localPosition.y;
        CanvasHoverEndingY = CanvasHoverStartingY + (canvases[0].GetComponent<RectTransform>().rect.height/15);
    }


    public Vector3 OffsetVector(float BaseY)
    {
        Vector3 vector3;
        Vector3 localvector3 = canvases[0].transform.localPosition;
        float yoffset = canvases[0].GetComponent<RectTransform>().rect.height;

        vector3 = new Vector3(0, 0, 0);

        if (SelectedPanelNumber == 0)
        {
            vector3  = new Vector3(localvector3.x, BaseY, localvector3.z);
        }
        else if (SelectedPanelNumber == 1)
        {
            localvector3 = canvases[1].transform.localPosition;
            vector3 = new Vector3(localvector3.x, BaseY - yoffset, localvector3.z);
        }
        else if (SelectedPanelNumber == 2)
        {
            localvector3 = canvases[1].transform.localPosition;
            vector3 = new Vector3(localvector3.x, BaseY - (yoffset*2), localvector3.z);
        }
        else if (SelectedPanelNumber == 7)
        {
            localvector3 = canvases[1].transform.localPosition;
            vector3 = new Vector3(localvector3.x, BaseY - (yoffset * 3), localvector3.z);
        }
        else if (SelectedPanelNumber == 8)
        {
            localvector3 = canvases[1].transform.localPosition;
            vector3 = new Vector3(localvector3.x, BaseY - (yoffset * 4), localvector3.z);
        }
        else if (SelectedPanelNumber == 4)
        {
            localvector3 = canvases[1].transform.localPosition;
            vector3 = new Vector3(localvector3.x, BaseY - (yoffset * 5), localvector3.z);
        }
        return vector3;
    }

    void Start()
    {
        Reset();
       // FadeCanvasStartingColor = new Color32(129, 218, 255, 251);

    }

    void Update()
    {

        if (CanvasHoverState == CanvasHoverStates.GoingUp)
        {
            CanvasHoverTimer += Time.deltaTime;
            if (CanvasHoverTimer >= LagCanvasHoverTimer)
            {
                CanvasHoverTimer = 0.0f;
                CanvasHoverState = CanvasHoverStates.GoingDown;
            }
            else 
            {
                canvases[SelectedPanelNumber].transform.localPosition = Vector3.Lerp(OffsetVector(CanvasHoverStartingY), OffsetVector(CanvasHoverEndingY), CanvasHoverTimer / LagCanvasHoverTimer);
                SelectPanel.transform.localPosition = canvases[SelectedPanelNumber].transform.localPosition;
                CorrectSelectorIcon();
            }
        } else if (CanvasHoverState == CanvasHoverStates.GoingDown)
        {
            CanvasHoverTimer += Time.deltaTime;
            if (CanvasHoverTimer >= LagCanvasHoverTimer)
            {
                CanvasHoverTimer = 0.0f;
                CanvasHoverState = CanvasHoverStates.GoingUp;
            }
            else
            {

                canvases[SelectedPanelNumber].transform.localPosition = Vector3.Lerp(OffsetVector(CanvasHoverEndingY), OffsetVector(CanvasHoverStartingY), CanvasHoverTimer / LagCanvasHoverTimer);
                SelectPanel.transform.localPosition = canvases[SelectedPanelNumber].transform.localPosition;
            }
            CorrectSelectorIcon();
        }

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
                texts[0].text = "Night Approaches";
                texts[0].color = Color32.Lerp(new Color32(139, 32, 221, 255), FadeCanvasEndingColor, LERPtimer / LagTimeFade);

            }
        }
        if (TimeinaDay <= DayTime)
        {
            texts[0].text = " Night " + Day + " Arrives"; ;
           
        }
        else
        {
            texts[0].text = " Day " + Day + " -- " + (TimeinaDay-DayTime) + " Hours Until Nightfall"; ;
            if ((TimeinaDay - DayTime) == 1)
            {
                texts[0].text = " Day " + Day + " -- " + "1 Hour Until Nightfall"; ;

            }
        }

        if (SelectPossible)
        {
            SelectPanelTimer += Time.deltaTime;
            if ((SelectState == SelectStates.Displaying) && (SelectPanelTimer >= SelectPanelDisplayTime))
            {
                SelectPanelTimer = 0.0f;
                SelectState = SelectStates.FadingOut;
            }
            else if ((SelectState == SelectStates.FadingOut) && (SelectPanelTimer >= SelectPanelFadeTime))
            {
                SelectPanelTimer = 0.0f;
                SelectState = SelectStates.FadingIn;
            }
            else if ((SelectState == SelectStates.FadingOut) && (SelectPanelTimer < SelectPanelFadeTime))
            {
                SelectPanel.GetComponent<Image>().color = Color32.Lerp(StartingSelectColor, EndingSelectColor, SelectPanelTimer / SelectPanelFadeTime);
                SelectorIcon.GetComponent<Image>().color = Color32.Lerp(new Color32(255,255,255,255), new Color32(255, 255, 255, 0), SelectPanelTimer / SelectPanelFadeTime);
            }
            else if ((SelectState == SelectStates.FadingIn) && (SelectPanelTimer >= SelectPanelFadeTime))
            {
                SelectPanelTimer = 0.0f;
                SelectState = SelectStates.Displaying;
            }
            else if ((SelectState == SelectStates.FadingIn) && (SelectPanelTimer < SelectPanelFadeTime))
            {
                SelectPanel.GetComponent<Image>().color = Color32.Lerp(EndingSelectColor, StartingSelectColor, SelectPanelTimer / SelectPanelFadeTime);
                SelectorIcon.GetComponent<Image>().color = Color32.Lerp(new Color32(255, 255, 255, 0), new Color32(255, 255, 255, 255), SelectPanelTimer / SelectPanelFadeTime);

            }
        }




        if (canvases[5].GetComponentInChildren<SunScript>().Moving == false && ((DayTime + canvases[SelectedPanelNumber].GetComponent<DayChoice>().ChoiceCost) <= TimeinaDay) && (Input.GetKeyUp("space") || Input.GetKeyUp("return")))
        {

            MakeSelection(canvases[SelectedPanelNumber].GetComponent<DayChoice>());
        }

        if (Input.GetKeyUp("s") && canvases[5].GetComponentInChildren<SunScript>().Moving == false)
        {

            canvases[SelectedPanelNumber].transform.localPosition = OffsetVector(CanvasHoverStartingY);

            if (SelectedPanelNumber == 0)
            {

                SelectedPanelNumber = 1;
                SelectPanel.transform.localPosition = canvases[1].transform.localPosition;
              }
            else if (SelectedPanelNumber == 1)
            {
                SelectedPanelNumber = 2;
                SelectPanel.transform.localPosition = canvases[2].transform.localPosition;

            }
            else if (SelectedPanelNumber == 2)
            {
                SelectedPanelNumber = 7;
                SelectPanel.transform.localPosition = canvases[7].transform.localPosition;

            }
            else if (SelectedPanelNumber == 7)
            {
                SelectedPanelNumber = 8;
                SelectPanel.transform.localPosition = canvases[8].transform.localPosition;

            }
            else if (SelectedPanelNumber == 8)
            {
                SelectedPanelNumber = 4;

                SelectPanel.transform.localPosition = canvases[4].transform.localPosition;
                
            }
            else if (SelectedPanelNumber == 4)
            {
                SelectedPanelNumber = 0;
                SelectPanel.transform.localPosition = canvases[0].transform.localPosition;

            }
            CorrectSelectorIcon();

        }else if (Input.GetKeyUp("w") && canvases[5].GetComponentInChildren<SunScript>().Moving == false)
        {
            canvases[SelectedPanelNumber].transform.localPosition = OffsetVector(CanvasHoverStartingY);

            if (SelectedPanelNumber == 0)
            {
                SelectedPanelNumber = 4;
                SelectPanel.transform.localPosition = canvases[4].transform.localPosition;
            }
            else if (SelectedPanelNumber == 1)
            {
                SelectedPanelNumber = 0;
                SelectPanel.transform.localPosition = canvases[0].transform.localPosition;

            }
            else if (SelectedPanelNumber == 2)
            {
                SelectedPanelNumber = 1;
                SelectPanel.transform.localPosition = canvases[1].transform.localPosition;

            }
            else if (SelectedPanelNumber == 7)
            {
                SelectedPanelNumber = 2;
                SelectPanel.transform.localPosition = canvases[2].transform.localPosition;

            }
            else if (SelectedPanelNumber == 8)
            {
                SelectedPanelNumber = 7;

                SelectPanel.transform.localPosition = canvases[7].transform.localPosition;

            }
            else if (SelectedPanelNumber == 4)
            {
                SelectedPanelNumber = 8;
                SelectPanel.transform.localPosition = canvases[8].transform.localPosition;

            }
            CorrectSelectorIcon();

        }



    }

    public void MakeSelection(DayChoice dayChoice)
    {

        bool success = false;
        string movingstring = "";
        if (dayChoice.Bonus == DayChoice.BonusEnum.Health)
        {
            SelectPanel.transform.localPosition = canvases[0].transform.localPosition;
            MaxHealth += 10;
            success = true;
            movingstring = "+10 Magic Fire Shield";
            vectorformovingtext = canvases[0].transform.position;
            vectorformovingtext.x = canvases[0].transform.position.x + 200;
            vectorformovingtext.y = canvases[0].transform.position.y;
            SelectedPanelNumber = 0;

        }
        else if (dayChoice.Bonus == DayChoice.BonusEnum.Sewing)
        {
            SelectPanel.transform.localPosition = canvases[1].transform.localPosition;
            MaxAccelerant += 1;
            success = true;
            movingstring = "+1 Heart Slot";
            vectorformovingtext = canvases[1].transform.position;
            vectorformovingtext.x = canvases[1].transform.position.x + 200;
            vectorformovingtext.y = canvases[1].transform.position.y;
            SelectedPanelNumber = 1;
        }
        else if ((dayChoice.Bonus == DayChoice.BonusEnum.Accelerant) && (MaxAccelerant > AccelerantInHand))
        {
            SelectPanel.transform.localPosition = canvases[2].transform.localPosition;
            AccelerantInHand += 1;
            success = true;
            movingstring = "+1 Sacrifical Heart";
            vectorformovingtext = canvases[2].transform.position;
            vectorformovingtext.x = canvases[2].transform.position.x + 200;
            vectorformovingtext.y = canvases[2].transform.position.y;
            SelectedPanelNumber = 2;
        }
        else if (dayChoice.Bonus == DayChoice.BonusEnum.Melee)
        {
            MeleeWeaponDmg += 3;
            movingstring = "+3 Melee Damage";
            SelectPanel.transform.localPosition = canvases[7].transform.localPosition;
            success = true;
            vectorformovingtext = canvases[7].transform.position;
            vectorformovingtext.x = canvases[7].transform.position.x + 200;
            vectorformovingtext.y = canvases[7].transform.position.y;
            SelectedPanelNumber = 7;
        }
        else if (dayChoice.Bonus == DayChoice.BonusEnum.Ranged)
        {
            RangeWeaponDmg += 2;
            movingstring = "+2 Ranged Damage";
            SelectPanel.transform.localPosition = canvases[8].transform.localPosition;
            success = true;
            vectorformovingtext = canvases[8].transform.position;
            vectorformovingtext.x = canvases[8].transform.position.x + 200;
            vectorformovingtext.y = canvases[8].transform.position.y;
            SelectedPanelNumber = 8;

        }
        else if (dayChoice.Bonus == DayChoice.BonusEnum.Rest)
        {
            success = true;
            dayChoice.ChoiceCost = (TimeinaDay - DayTime);
            movingstring = "+" + ((TimeinaDay - DayTime) * 10) + "% One Night Speed Bonus";
            SpeedBonus = (TimeinaDay - DayTime) * 10;
            vectorformovingtext = canvases[4].transform.position;
            vectorformovingtext.x = canvases[4].transform.position.x;
            vectorformovingtext.y = canvases[4].transform.position.y;
            SelectedPanelNumber = 4;

        }
        //Play animation?
        if (success)
        {
            DayScene.DayTime += dayChoice.ChoiceCost;
            canvases[5].GetComponentInChildren<SunScript>().Move(dayChoice.ChoiceCost);
            FindObjectOfType<MovingTextScript>().GetComponent<Text>().text = movingstring;
            FindObjectOfType<MovingTextScript>().Move(vectorformovingtext, 1.0f, new Vector3(canvases[3].transform.position.x, canvases[3].transform.position.y, canvases[3].transform.position.z), dayChoice.ChoiceCost);
            canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn";
            if (DayTime < TimeinaDay)
            {
                canvases[4].GetComponentInChildren<Text>().text = canvases[4].GetComponentInChildren<Text>().text + "\n(Gain Temporary " + ((TimeinaDay - DayTime) * 10) + "% Speed Bonus)";
            }
        }
        CorrectSelectorIcon();
        if (TimeinaDay <= DayTime)
        {
            SelectPanel.SetActive(false);
            SelectorIcon.SetActive(false);
        }
    }

    private void CorrectSelectorIcon()
    {
        Vector3 vector3 = new Vector3(0, 0, 0);
        float offset = ((SelectPanel.GetComponent<RectTransform>().rect.width / 2)) + (SelectorIcon.GetComponent<RectTransform>().rect.width);
        vector3 = SelectorIcon.transform.localPosition;
        vector3.x = SelectPanel.transform.localPosition.x - offset;
        vector3.y = SelectPanel.transform.localPosition.y;
        SelectorIcon.transform.localPosition = vector3;
    }
    public void Fade()
    {
        texts[0].text = "Night Arrives";

        //return to night scene
    }
}
