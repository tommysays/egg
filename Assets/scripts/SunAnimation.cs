using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public float timer = 0.0f;
    public float animationtime = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= animationtime)
        {
            
            if (this.GetComponent<Image>().sprite == sprites[0])
            {
                this.GetComponent<Image>().sprite = sprites[1];
            }
            else
            {
                this.GetComponent<Image>().sprite = sprites[0];
            }

            timer = 0.0f;

        }
        
    }
}
