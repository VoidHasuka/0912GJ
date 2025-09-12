using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    public SoundSourceManager soundSourceManager;

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
    }

    private void Start()
    {
        //初始化
        soundSourceManager = new SoundSourceManager();
    }


}
