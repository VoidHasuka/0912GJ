using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Judgeline : MonoBehaviour
{
    public Image img;
    public Sprite target;
    public Sprite show;
    public Sprite use;

    void Start()
    {
        img = this.GetComponent<Image>();
    }

    public void SetTargetImg()
    {
        img.sprite = target;
    }

    public void SetShowImg() 
    {
        img.sprite = show;
    }

    public void SetUseImg()
    {
        img.sprite = use;
    }

}
