using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager
{
    public List<UIBase> uiList;
    public Canvas uiCanvas;

    public MusicCheck musicCheck;

    public GameObject cursorGo;

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

        //隐藏原有cursor
        //Cursor.visible = false;
        //创建cursor，并设置为最高层级
        cursorGo = GameObject.Instantiate(Resources.Load<GameObject>("UI/CursorUI"), uiCanvas.transform);    
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

        //清理一些不在列表中的UI
    }

    //开始菜单UI
    public void StartMenuUI()
    {
        ClearUIList();

        //实例化开始菜单UI
        UIBase startButton = new UIBase("StartButton");

        // 获取RectTransform
        RectTransform rect = startButton.uiGo.GetComponent<RectTransform>();
        if (rect != null)
        {
            // 初始缩放为0
            rect.localScale = Vector3.zero;

            // 缩放
            rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetLink(rect.gameObject,LinkBehaviour.KillOnDestroy); 
            // 上下震动
            rect.DOShakeAnchorPos(0.2f, new Vector2(0, 30), 10, 90, false, true).SetLink(rect.gameObject, LinkBehaviour.KillOnDestroy);
        }
    }

    //关卡菜单UI
    public void LevelMenuUI()
    {
        ClearUIList();

        //实例化关卡菜单UI
        UIBase backButton = new UIBase("BackButton");

        //UIBase level_1Button = new UIBase("Level_1Button");
        //UIBase level_2Button = new UIBase("Level_2Button");
        //UIBase level_3Button = new UIBase("Level_3Button");
        //UIBase level_4Button = new UIBase("Level_4Button");
        //UIBase level_5Button = new UIBase("Level_5Button");
        //UIBase level_6Button = new UIBase("Level_6Button");
        //UIBase level_7Button = new UIBase("Level_7Button");
        //UIBase level_8Button = new UIBase("Level_8Button");



    }

    //局内游戏UI
    public void BasicGameUI()
    {
        ClearUIList();

        //实例化局内游戏UI
        UIBase backButton = new UIBase("BackButton");
        UIBase musicCheckUI = new UIBase("MusicCheckUI");

        UIBase resetButton = new UIBase("ResetButton");

        UIBase ShootUI = new UIBase("ShootUI");
        UIBase MouseUseUI = new UIBase("MouseUseUI");
        UIBase PlayUI = new UIBase("PlayUI");
    }

    //返回确认UI
    public void BackSureUI()
    {
        //实例化返回确认UI
        UIBase backSureUI = new UIBase("BackSureUI");

        // 获取RectTransform
        RectTransform rect = backSureUI.uiGo.GetComponent<RectTransform>();
        if (rect != null)
        {
            // 初始缩放为0
            rect.localScale = Vector3.zero;
            // 缩放
            rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetLink(rect.gameObject, LinkBehaviour.KillOnDestroy);
        }
    }

    //删除返回确认UI
    public void DestroyBackSureUI()
    {
        uiList.Find(ui => ui.uiGo.name == "BackSureUI(Clone)")?.Destroy();
        //在列表中移除
        uiList.RemoveAll(ui => ui.uiGo.name == "BackSureUI(Clone)");
    }
    
}
