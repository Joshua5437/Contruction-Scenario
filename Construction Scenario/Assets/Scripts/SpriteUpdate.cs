using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpriteUpdate : MonoBehaviour
{
    private int count = 0;
    public Image Image;
    public Sprite newSprite, newSprite2, newSprite3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpriteChange()
    {
        Image.sprite = newSprite;
        newSprite = newSprite2;
        newSprite2 = newSprite3;
        count++;

        if(count == 3) {
            GameObject.Find("Button").GetComponentInChildren<Text>().text = "Done";
        }
    }
}
