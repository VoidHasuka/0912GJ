using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public List<UIBase> uiList;
    public Canvas uiCanvas;

    public void Init()
    {
        if(!GameObject.Find("UICanvas"))
        {
            Debug.LogError("未找到UICanvas，请在场景中添加！");
        }
        else
        {
            uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        }

        uiList = new List<UIBase>();
    }

    private void ClearUIList()
    {
        foreach(var ui in uiList)
        {
            if(ui!=null)
            {
                ui.Destroy();
            }
        }
        uiList.Clear();
    }

    //开始菜单UI
    public void StartMenuUI()
    {
        ClearUIList();

        //实例化开始菜单UI
        UIBase startButton = new UIBase("StartButton");
    }

    //关卡菜单UI
    public void LevelMenuUI()
    {
        ClearUIList();


    }

    //局内游戏UI
    public void BasicGameUI()
    {
        ClearUIList();


    }
}
