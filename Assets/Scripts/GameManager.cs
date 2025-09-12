using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveType
{
    A,
    B,
    C
}
public enum GameState
{
   Start,
   Level,
   Play,
   End
}
[System.Serializable]
public struct PassWord
{
    public int levelIndex;
    public float levelTime;
    public List<WaveAndTime> levelPassWord;
}
[System.Serializable]
public struct WaveAndTime
{
    public WaveType waveType;
    public float time;
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [Header("Game Settings")]
    [Tooltip("游戏设置相关")]
    public float expendSpeed = 5f; // 声波扩散速度
    public float LifeTime = 7f; //声波存在时间
    public PasswordSO passwordSO; //密码数据

    [Header("Game State")]
    public GameState currentState;
    public MusicCheck musicCheck;

    [Header("Managers")]
    [Tooltip("游戏中各个管理器的实例，方便全局访问，由GameManager自动创建")]
    public SoundSourceManager soundSourceManager;
    public UIManager uiManager;
    public EffectManager effectManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保证切换场景时不被销毁
        }
        else
        {
            Destroy(gameObject); // 保证只有一个实例存在
        }

        //创建管理器
        soundSourceManager = new SoundSourceManager();
        uiManager = new UIManager();
        effectManager = new EffectManager();

        //初始化管理器
        soundSourceManager.Init();
        uiManager.Init();
        effectManager.Init();
    }

    private void Start()
    {
        //启动时显示开始菜单UI
        ChangeGameState(GameState.Start);
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
        // 根据新的游戏状态执行相应的操作
        switch (newState)
        {
            case GameState.Start:

                // 进入开始状态的逻辑
                uiManager.StartMenuUI();

                break;
            case GameState.Level:

                // 进入关卡选择状态的逻辑
                uiManager.LevelMenuUI();

                break;
            case GameState.Play:

                // 进入游戏玩法状态的逻辑
                uiManager.BasicGameUI();

                break;
            case GameState.End:

                // 进入游戏结束状态的逻辑


                break;
            default:
                Debug.LogWarning("未知的游戏状态: " + newState);
                break;
        }
    }

    //根据index加载关卡
    public void InitLevel(int index)
    {
        foreach(var pw in passwordSO.passWords)
        {
            if(pw.levelIndex==index)
            {
                musicCheck.Init(pw.levelPassWord, index,pw.levelTime);
                return;
            }
        }
        musicCheck.Init(null, index,LevelTime:15f);
    }

    //传入事件与时间，在指定时间后触发事件
    public void InvokeAfterDelay(System.Action action, float delay)
    {
        // 使用DOTween的延时回调
        DOVirtual.DelayedCall(delay, () =>
        {
            action?.Invoke();
        });
    }
}
