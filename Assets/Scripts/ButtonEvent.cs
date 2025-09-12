using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 按钮事件管理
/// </summary>
public class ButtonEvent : MonoBehaviour
{
    //触发声源扩散
    public void OnClickAllEmit()
    {
        GameManager.Instance.soundSourceManager.EmitAll();
    }

    //进入关卡选择界面
    public void OnClickEnterLevelSelect()
    {
        GameManager.Instance.uiManager.LevelMenuUI();
    }
}
