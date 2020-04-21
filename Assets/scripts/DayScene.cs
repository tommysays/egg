using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayScene : MonoBehaviour
{
    public static int DayTime = 0;
    public static int TimeinaDay = 8;
    public static int MaxHealth;
    public static int MaxAccelerant;
    public static int AccelerantInHand;
    public static int MeleeWeaponDmg;
    public static int RangeWeaponDmg;
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

    public bool IntroFading = false;

    public Color32 FadeCanvasStartingColor;
    public Color32 FadeCanvasEndingColor = new Color32(0, 0, 0, 255);
    public Color32 StartBackgroundColor = new Color32(129, 218, 255, 251);
    public Color32 EndBackgroundColor = new Color32(45, 79, 119, 255);
    public GameObject BackUpPanel;

    public GameObject Egg;
    public Sprite[] eggSprites;
    public Vector3 EggStart;
    public Vector3 EggPriorLocation;
    public Vector3 EggWiggle;

    public bool EggWiggling = false;

    public bool FinalTransition = false;
    public float FinalTransitionTimer = 0.0f;
    public float LagFinalTransitionTime = 2.5f;
    public GameObject[] ObjectsMovingInFinalTransition;
    List<float> ObjectFinalTransitionXoffset = new List<float>();
    List<float> ObjectFinalTransitionYoffset = new List<float>();





    public bool timeron = false;

    public void Reset()
    {
        DayTime = 0;

        DayScene.MaxAccelerant = GlobalDataScript.MaxAccelerant;
        DayScene.AccelerantInHand = GlobalDataScript.AccelerantInHand;
        DayScene.MaxHealth = GlobalDataScript.MaxHealth;
        DayScene.MeleeWeaponDmg = GlobalDataScript.MeleeWeaponDmg;
        DayScene.RangeWeaponDmg = GlobalDataScript.RangeWeaponDmg;
        Day = GlobalDataScript.Day;


        this.canvases[3].GetComponentInChildren<Text>().text = "" + "\tMagic Circle Max Health: " + DayScene.MaxHealth + "\n\tHeart Pouches: " + DayScene.MaxAccelerant +
             "\n\tSacrificial Hearts: " + DayScene.AccelerantInHand + "\n\tMelee Attack: " + DayScene.MeleeWeaponDmg + "\n\tRanged Attack: " + DayScene.RangeWeaponDmg;
        SpeedBonus = (TimeinaDay - DayScene.DayTime) * 10;


        this.canvases[4].GetComponentInChildren<Text>().text = "Rest Until Dawn\n(Gain Temporary " + DayScene.SpeedBonus + "% Speed Bonus)";
        SelectPanel.SetActive(true);
        SelectorIcon.SetActive(true);
        texts[0].text = " Day " + (Day+1) + " -- " + (TimeinaDay - DayTime) + " Hours Until Nightfall"; ;
        SelectPossible = true;
        SelectState = SelectStates.Displaying;
        SelectedPanelNumber = 0;
        FadeCanvasStartingColor = new Color32(0, 0, 0, 0);
        StartBackgroundColor = new Color32(129, 218, 255, 251);
        EndBackgroundColor = new Color32(45, 79, 119, 255);
        FadeCanvasEndingColor = new Color32(0, 0, 0, 255);
        SelectPanel.transform.localPosition = canvases[0].transform.localPosition;
        BackUpPanel.transform.localPosition = new Vector3(canvases[0].transform.localPosition.x, canvases[0].transform.localPosition.y - (BackUpPanel.GetComponent<RectTransform>().rect.height/2) -(canvases[0].GetComponent<RectTransform>().rect.height / 4), canvases[0].transform.localPosition.z);
        CanvasHoverState = CanvasHoverStates.GoingUp;
        CanvasHoverStartingY = canvases[0].transform.localPosition.y;
        CanvasHoverEndingY = CanvasHoverStartingY + (canvases[0].GetComponent<RectTransform>().rect.height/15);
        Egg.GetComponent<Image>().sprite = eggSprites[Day];
        EggStart = Egg.transform.localPosition;
        EggWiggle = new Vector3(EggStart.x + Random.Range(-2f, 2f), EggStart.y + Random.Range(-1f, 1f), EggStart.z);
        EggPriorLocation = EggStart;
        EggWiggling = false;
        FinalTransition = false;
        FinalTransitionTimer = 0.0f;
        LagFinalTransitionTime = 2.5f;
        ObjectFinalTransitionXoffset.Clear();
        ObjectFinalTransitionYoffset.Clear();
        canvases[6].GetComponent<Image>().color = FadeCanvasStartingColor;
        LERPtimer = 0;

        for (int x = 0; x < ObjectsMovingInFinalTransition.Length; x++)
        {
            ObjectFinalTransitionXoffset.Add(0f);
            ObjectFinalTransitionYoffset.Add(0f);
            FinalTransitionOffset(x);

        }

}



public void FinalTransitionOffset(int x)
    {
        Vector3 vector3;
        vector3 = new Vector3(EggStart.x - ObjectsMovingInFinalTransition[x].transform.localPosition.x, EggStart.y - ObjectsMovingInFinalTransition[x].transform.localPosition.y, EggStart.z - ObjectsMovingInFinalTransition[x].transform.localPosition.z);

        ObjectFinalTransitionXoffset[x] =  ObjectsMovingInFinalTransition[x].transform.localPosition.x - EggStart.x;
        ObjectFinalTransitionYoffset[x] = ObjectsMovingInFinalTransition[x].transform.localPosition.y - EggStart.y;

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
       IntroFading = true;
    }

    void Update()
    {

        if (IntroFading)
        {
            FinalTransitionTimer += Time.deltaTime;
            if (FinalTransitionTimer >= LagFinalTransitionTime)
            {
                IntroFading = false;
                FinalTransitionTimer = 0.0f;
            }
            else
            {
                canvases[10].GetComponent<Image>().color = Color32.Lerp(new Color32(255,255,255,255), new Color32(255, 255, 255, 0), FinalTransitionTimer / (LagFinalTransitionTime-1f));
            }
        }

        if (CanvasHoverState == CanvasHoverStates.GoingUp && (DayTime < TimeinaDay))
        {
            CanvasHoverTimer += Time.deltaTime;
            if (CanvasHoverTimer >= LagCanvasHoverTimer)
            {
                CanvasHoverTimer = 0.0f;
                CanvasHoverState = CanvasHoverStates.GoingDown;
                float x = Random.Range(0, 3);
                if (x > 1)
                {
                    EggPriorLocation = Egg.transform.localPosition;
                    EggWiggle = new Vector3(EggStart.x + Random.Range(-3f, 3f), EggStart.y, EggStart.z);
                    EggWiggling = true;
                }

            }
            else
            {
                canvases[SelectedPanelNumber].transform.localPosition = Vector3.Lerp(OffsetVector(CanvasHoverStartingY), OffsetVector(CanvasHoverEndingY), CanvasHoverTimer / LagCanvasHoverTimer);

                SelectPanel.transform.localPosition = canvases[SelectedPanelNumber].transform.localPosition;
                CorrectSelectorIcon();
            }
        }
        else if (CanvasHoverState == CanvasHoverStates.GoingDown && (DayTime < TimeinaDay))
        {
            if (EggWiggling)
            {
                Egg.transform.localPosition = Vector3.Lerp(EggPriorLocation, EggWiggle, CanvasHoverTimer / LagCanvasHoverTimer);
            }
            CanvasHoverTimer += Time.deltaTime;
            if (CanvasHoverTimer >= LagCanvasHoverTimer)
            {
                CanvasHoverTimer = 0.0f;
                CanvasHoverState = CanvasHoverStates.GoingUp;
                EggWiggling = false;
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
                FinalTransition = true;
                texts[0].text = "Night Arrives";


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
            texts[0].text = " Night Arrives"; ;

        }
        else
        {
            texts[0].text = " Day " + (Day+1) + " -- " + (TimeinaDay - DayTime) + " Hours Until Nightfall"; ;
            if ((TimeinaDay - DayTime) == 1)
            {
                texts[0].text = " Day " + (Day+1) + " -- " + "1 Hour Until Nightfall"; ;

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
                SelectorIcon.GetComponent<Image>().color = Color32.Lerp(new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 0), SelectPanelTimer / SelectPanelFadeTime);
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

        }
        else if (Input.GetKeyUp("w") && canvases[5].GetComponentInChildren<SunScript>().Moving == false)
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



        //Final Scene Timer (moves visible objects like egg to center of screen

        if (FinalTransition)
        {
            FinalTransitionTimer += Time.deltaTime;
            if (FinalTransitionTimer >= LagFinalTransitionTime)
            {
                //FinalTransition = false;
                Fade();

            }
            else
            {
                Egg.transform.localPosition = Vector3.Lerp(EggPriorLocation, new Vector3(2, 80, 0), FinalTransitionTimer / LagFinalTransitionTime);

                for (int x = 0; x < ObjectsMovingInFinalTransition.Length; x++)
                {
                    Vector3 startVector = new Vector3(EggPriorLocation.x + ObjectFinalTransitionXoffset[x], EggPriorLocation.y + ObjectFinalTransitionYoffset[x], 0);
                    Vector3 endVector = new Vector3(0 + ObjectFinalTransitionXoffset[x] + 2, 0 + ObjectFinalTransitionYoffset[x]+ 80, 0);
                    ObjectsMovingInFinalTransition[x].transform.localPosition = Vector3.Lerp(startVector, endVector, FinalTransitionTimer / LagFinalTransitionTime);
                }

                EggWiggling = false;
                CanvasHoverTimer = 0.0f;
            }
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

       

        GlobalDataScript.MaxAccelerant = DayScene.MaxAccelerant;
        GlobalDataScript.AccelerantInHand = DayScene.AccelerantInHand;
        GlobalDataScript.MaxHealth = DayScene.MaxHealth;
        GlobalDataScript.MeleeWeaponDmg = DayScene.MeleeWeaponDmg;
        GlobalDataScript.RangeWeaponDmg = DayScene.RangeWeaponDmg;
        GlobalDataScript.SpeedBonus = DayScene.SpeedBonus;

        StartCoroutine(LoadYourAsyncScene());
     
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("NightScene");


        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


}
