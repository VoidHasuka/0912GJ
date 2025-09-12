using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveType
{
    A,
    B,
    C
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [Header("Game Settings")]
    [Tooltip("游戏设置相关")]
    public float expendSpeed = 5f; // 声波扩散速度
    public float LifeTime = 7f; //声波存在时间


    [Header("Managers")]
    [Tooltip("游戏中各个管理器的实例，方便全局访问，由GameManager自动创建")]
    public SoundSourceManager soundSourceManager;
    public UIManager uiManager;

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

        //初始化管理器
        soundSourceManager.Init();
        uiManager.Init();
    }

    private void Start()
    {
        //启动时显示开始菜单UI
        uiManager.StartMenuUI();
    }

}
