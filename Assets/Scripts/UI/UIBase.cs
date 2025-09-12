using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase
{
    public GameObject uiGo;
    public UIBase(string name)
    {
        GameManager.Instance.uiManager.uiList.Add(this);
        uiGo = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + name), GameManager.Instance.uiManager.uiCanvas.transform);
    }


    public void Destroy()
    {
        //基础销毁
        GameObject.Destroy(uiGo);
        GameManager.Instance.uiManager.uiList.Remove(this);
    }
}
