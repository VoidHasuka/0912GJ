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
        GameManager.Instance.ChangeGameState(GameState.Level);
    }
    //进入关卡
    public void OnClickEnterLevel(int index)
    {
        GameManager.Instance.ChangeGameState(GameState.Play);
        //依据index加载关卡
        //GameManager.Instance.InvokeAfterDelay(() => GameManager.Instance.InitLevel(index), 0.1f);
        GameManager.Instance.InitLevel(index);

    }

    //引出确认界面
    public void OnClickBackNotSure()
    {
        GameManager.Instance.uiManager.BackSureUI();
    }
    //返回界面，确认返回
    public void OnClickBackYes()
    {
        OnClickBackSure();
    }
    //返回界面，取消返回
    public void OnClickBackNo()
    {
        GameManager.Instance.uiManager.DestroyBackSureUI();
    }

    //执行返回
    public void OnClickBackSure()
    {
        switch (GameManager.Instance.currentState)
        {
            case GameState.Level:
                // 从关卡选择界面返回到开始菜单
                GameManager.Instance.ChangeGameState(GameState.Start);
                break;
            case GameState.Play:
                // 从游戏界面返回到关卡选择界面
                GameManager.Instance.ChangeGameState(GameState.Level);
                break;
            case GameState.End:
                // 从结算界面返回到开始菜单
                GameManager.Instance.ChangeGameState(GameState.Start);
                break;
            case GameState.Start:
            default:
                // 已在开始菜单，可根据需求添加退出或提示
                break;
        }
    }

    //重置MusicCheck
    public void OnClickResetMusicCheck()
    {
        GameManager.Instance.musicCheck.ResetInput();
        GameManager.Instance.soundSourceManager.DeleteAllWave();
    }

    //启动所有声源
    public void OnClickPlayAllSound()
    {
        GameManager.Instance.PlayAllSound();
    }

    public void OnClickPlayPasword()
    {
        GameManager.Instance.musicCheck.PlayPassword();
    }

    public void OnClickNextLevel()
    {
        GameManager.Instance.currentLevelIndex++;
        GameManager.Instance.ChangeGameState(GameState.Level);
        GameManager.Instance.ChangeGameState(GameState.Play);
        GameManager.Instance.InitLevel(GameManager.Instance.currentLevelIndex);
    }
}
