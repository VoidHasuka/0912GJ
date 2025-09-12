using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase
{
    public GameObject uiGo;
    public UIBase(string name,Transform tf=null)
    {
        GameManager.Instance.uiManager.uiList.Add(this);
        Transform parentTf = tf == null ? GameManager.Instance.uiManager.uiCanvas.transform : tf;
        uiGo = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + name), parentTf);
    }


    public void Destroy()
    {
        //基础销毁
        GameObject.Destroy(uiGo);
    }
}
