using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingTextScript : MonoBehaviour
{
    public Vector3 StartingPosition;
    public Vector3 EndingPosition;
    public float LerpTime = 0.0f;
    public float LagTime = 1.5f;
    public static bool IsMoving = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoving)
        {
            LerpTime += Time.deltaTime;
            if (LerpTime >= LagTime)
            {
                IsMoving = false;
                transform.position = EndingPosition;
                this.GetComponent<Text>().color = new Color32(50, 50, 50, 0);

            }
            else
            {
                transform.position = Vector3.Lerp(StartingPosition, EndingPosition, LerpTime / LagTime);
            }
        }
    }


    public void Move(Vector3 Start, float timelag, Vector3 end)
    {
        this.transform.position = Start;
        LagTime = timelag;
        StartingPosition = this.transform.position;
        EndingPosition = end;
        IsMoving = true;
        LerpTime = 0.0f;
        this.GetComponent<Text>().color = new Color32(140, 33, 22, 255);

    }
}
