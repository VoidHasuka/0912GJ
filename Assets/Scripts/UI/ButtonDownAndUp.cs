using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDownAndUp : MonoBehaviour
{
    //挂载到具有两种状态的按钮上
    public Sprite downImg;
    public Sprite upImg;

    public void OnClickDown()
    {
        gameObject.GetComponent<Image>().sprite = downImg;
    }

    public void OnClickUp() 
    {
        gameObject.GetComponent<Image>().sprite = upImg;
    }
}
