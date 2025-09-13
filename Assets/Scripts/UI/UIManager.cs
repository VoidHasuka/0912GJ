using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIManager
{
    public List<UIBase> uiList;
    public Canvas uiCanvas;

    public MusicCheck musicCheck;

    public GameObject cursorGo;

    //不注册到UILIST中，游戏过程始终保留
    public GameObject parent;
    public GameObject selectBGUI;

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
        Cursor.visible = false;
        //创建cursor，并设置为最高层级
        cursorGo = GameObject.Instantiate(Resources.Load<GameObject>("UI/CursorUI"), uiCanvas.transform);    

        //创建parent并挂载到uiCanvas上
        parent = GameObject.Instantiate(Resources.Load<GameObject>("UI/parent"),uiCanvas.transform);

        //创建selectBGUI并挂载到uiCanvas上
        selectBGUI = GameObject.Instantiate(Resources.Load<GameObject>("UI/SelectBGUI"), uiCanvas.transform);
        selectBGUI.transform.SetAsFirstSibling();
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

        //SelectBGUI位置 重置
        selectBGUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(840f, 0);
    }

    //关卡菜单UI
    public void LevelMenuUI()
    {

        bool existStartUI = false;
        foreach (var ui in uiList)
        {
            if ((ui.uiGo.name == "StartButton(Clone)"))
            {
                existStartUI = true;

                //透明然后删除掉这个StartButton
                uiList.Remove(ui);
                ui.uiGo.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f,0f), 1f).OnComplete(() => { ui.Destroy(); });
                break;
            }
        }
        if (!existStartUI)
        {
            ClearUIList();
        }



        //UIBase level_1Button = new UIBase("Level_1Button");
        //UIBase level_2Button = new UIBase("Level_2Button");
        //UIBase level_3Button = new UIBase("Level_3Button");
        //UIBase level_4Button = new UIBase("Level_4Button");
        //UIBase level_5Button = new UIBase("Level_5Button");
        //UIBase level_6Button = new UIBase("Level_6Button");
        //UIBase level_7Button = new UIBase("Level_7Button");
        //UIBase level_8Button = new UIBase("Level_8Button");

        List<UIBase> levelButtonList = new List<UIBase>();
        for (int i = 0; i <= GameManager.Instance.lastLevelIndex; i++)
        {
            int tmpValue = i + 1;
            UIBase level_Button = new UIBase("Level_" + tmpValue.ToString() + "Button");
            levelButtonList.Add(level_Button);
        }
        for(int i = 0; i < levelButtonList.Count-1; i++)
        {
            var levelButtonUI = levelButtonList[i].uiGo.GetComponent<LevelButtonUI>();
            levelButtonUI.Finished();
        }

        

        //激活BGUI
        if(!selectBGUI.activeSelf)selectBGUI.SetActive(true);


        //进行平滑移动
        if (existStartUI)
        {
            //将所有ui添加为parent的子物体
            foreach (var ui in uiList) 
            {
                ui.uiGo.transform.SetParent(parent.transform, false);
            }
            parent.transform.DOLocalMoveX(-uiCanvas.gameObject.GetComponent<RectTransform>().rect.width, 1.5f).From(0f,true);

            selectBGUI.transform.DOLocalMoveX(-800f, 1f).From(840f,true);
        }
        else
        {
            //将所有ui添加为parent的子物体
            foreach (var ui in uiList)
            {   
                ui.uiGo.transform.SetParent(parent.transform, false);
            }
            parent.transform.localPosition = new Vector3(-uiCanvas.gameObject.GetComponent<RectTransform>().rect.width,0f,0f);

            selectBGUI.transform.localPosition = new Vector3(-800f,0f,0f);
        }

        //实例化关卡菜单UI
        UIBase backButton = new UIBase("BackButton");
        //实例化导航UI
        UIBase targetUI = new UIBase("targetUI");
        //加入parent
        targetUI.uiGo.transform.parent = parent.transform;
        targetUI.uiGo.transform.position = levelButtonList[GameManager.Instance.currentLevelIndex].uiGo.transform.position;
    }

    //局内游戏UI
    public void BasicGameUI()
    {
        ClearUIList();

        //禁用BGUI
        if (selectBGUI.activeSelf)
        {
            selectBGUI.SetActive(false);
        }

        //实例化局内游戏UI
        UIBase backButton = new UIBase("BackButton");
        UIBase musicCheckUI = new UIBase("MusicCheckUI");

        //UIBase resetButton = new UIBase("ResetButton");

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

    //生成关卡结束UI
    public void LevelSuccessUI(string t)
    {
        UIBase LevelEndUI = new UIBase("LevelEndUI");

        LevelEndUI.uiGo.GetComponentInChildren<TextMeshProUGUI>().text = t;

        //执行补间动画
        RectTransform rt = LevelEndUI.uiGo.GetComponent<RectTransform>();

        //scale变化
        rt.DOScaleX(1f, 0.85f);

        GameManager.Instance.InvokeAfterDelay(() =>
        {
            GameManager.Instance.EnterNextLevel();
        }, 2.85f);
    }
}
