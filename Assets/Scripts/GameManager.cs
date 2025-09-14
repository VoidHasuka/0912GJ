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
    public int currentLevelIndex = 0;
    public int lastLevelIndex = 0;
    public MusicCheck musicCheck;
    private Receiver receiver;
    private ReceiverMove souceMove;

    [Header("Managers")]
    [Tooltip("游戏中各个管理器的实例，方便全局访问，由GameManager自动创建")]
    public SoundSourceManager soundSourceManager;
    public UIManager uiManager;
    public EffectManager effectManager;
    public AudioManager audioManager;

    [Header("锁定移动")]
    public bool lockMove = false;

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
        audioManager = new AudioManager();

        //初始化管理器
        soundSourceManager.Init();
        uiManager.Init();
        effectManager.Init();
        audioManager.Init();
    }

    private void Start()
    {
        //启动时显示开始菜单UI
        ChangeGameState(GameState.Start);
    }
 
    private void Update()
    {
        //更新cursor

        uiManager.cursorGo.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2);
        uiManager.cursorGo.transform.SetAsLastSibling();


        if (currentState == GameState.Play)
        {
            if (!lockMove)
            {
                //左键移动位置
                receiver.GetComponent<ReceiverMove>()?.ReceiverMoveByMouse();
                if (souceMove != null) souceMove.ReceiverMoveByMouse();
                //右键启动所有声源
                if (Input.GetMouseButtonUp(1))
                {
                    //musicCheck.ClearPasswordUI();
                    //ClearPasswordUI
                    PlayAllSound();
                    lockMove = true;
                }
            }
        }
    }

    public void PlayAllSound()
    {
        soundSourceManager.DeleteAllWave();
        musicCheck.ResetInput();
        soundSourceManager.EmitAll();
    }

    public void ChangeGameState(GameState newState)
    {
        //清理
        if(receiver!=null)
        {
            Destroy(receiver.gameObject);
        }
        //摄像机位置更新
        Camera.main.transform.position = new Vector3(0, 0, -10);
        //UI动画

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

                //锁定Move
                lockMove = true;

                //一定间隔后启动PlayPassword
                InvokeAfterDelay(() => {
                    musicCheck.ResetInput();
                    soundSourceManager.DeleteAllWave();
                    musicCheck.PlayPassword();               
                },0.75f);

                //一定间隔后解锁Move
                InvokeAfterDelay(() =>
                {
                    lockMove = false;
                }, 5f);


                break;
            case GameState.End:

                // 进入游戏结束状态的逻辑
                GameManager.Instance.uiManager.ClearUIList();
                var EndCanvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/End"));

                break;
            default:
                Debug.LogWarning("未知的游戏状态: " + newState);
                break;
        }
    }

    //根据index加载关卡
    public void InitLevel(int index)
    {

        SwitchCameraPosition(index);

        currentLevelIndex = index;

        foreach (var pw in passwordSO.passWords)
        {
            if(pw.levelIndex==index)
            {
                musicCheck.Init(pw.levelPassWord, index,pw.levelTime);
                return;
            }
        }
        musicCheck.Init(null, index,LevelTime:15f);
    }

    //根据index切换摄像机位置
    private void SwitchCameraPosition(int index)
    {
        //创建接收器
        receiver = GameObject.Instantiate(Resources.Load<Receiver>("Prefab/Receiver")).GetComponent<Receiver>();

        if(index == 7)
        {
            Destroy(receiver.GetComponent<ReceiverMove>());
            souceMove = GameObject.Find("SourceWave_Move").GetComponent<ReceiverMove>();
            if(souceMove==null)souceMove=GameObject.Find("SourceWave_Move").AddComponent<ReceiverMove>();
        }
        else
        {
            if(souceMove!=null)GameObject.Destroy(souceMove);
        }

            //动画UI


            //假设有一个预设的摄像机位置列表
            //按类似的方式，可以扩展为更多位置
            List<Vector3> cameraPositions = new List<Vector3>()
        {
            new Vector3(500,500,-10),
            new Vector3(1000,1000,-10),
            new Vector3(1500,1500,-10),
            new Vector3(2000,2000,-10),
            new Vector3(2500,2500,-10),
            new Vector3(3000,3000,-10),
            new Vector3(3500,3500,-10),
            new Vector3(4000,4000,-10)
        };
        if(index>=0 && index<cameraPositions.Count)
        {
            //位置更新
            Camera.main.transform.position = cameraPositions[index];
            Vector3 offset = new Vector3(0, 0, 0);

            if (index == 7)
            {
                offset = new Vector3(4.2f, -2f, 0);
            }
            
            receiver.transform.position = new Vector3(cameraPositions[index].x, cameraPositions[index].y, 1) + offset;
        }
        else
        {
            Debug.LogWarning("无效的摄像机位置索引: " + index);
        }
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

    public void LevelSuccess()
    {
        lastLevelIndex++;

        //计算准度
        float accuracyRate =(1-musicCheck.ComputeAccuracy()) * 100;

        string text = "准确度" + ((int)accuracyRate).ToString() + "%！";

        //弹出UI
        uiManager.LevelSuccessUI(text);

        //生成切关特效
        GameObject Go = GameObject.Instantiate(Resources.Load("Prefab/SuccessEffect")) as GameObject;
        Go.transform.position = Camera.main.transform.position;

        if(currentLevelIndex == 7)
        {
            //结束游戏
            ChangeGameState(GameState.End);
        }

    }

    public void EnterNextLevel()
    {

        ChangeGameState(GameState.Level);

        InvokeAfterDelay(() =>
        {
            ChangeGameState(GameState.Play);
            InitLevel(lastLevelIndex);
        }, 0f);
    }
}
