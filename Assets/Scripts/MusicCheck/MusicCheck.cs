using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicCheck : MonoBehaviour
{
    private int levelIndex;
    [Header("编辑模式")]
    public bool EditMode = false; // 编辑模式开关


    public float faultToleranceTime = 0.2f; // 容错时间
    private float maxTime = 15f;             // 最大时间
    private float Timer = 0f;               //  计时器
    private bool allowTimerStart = false;


    // 密码器，存储WaveType与time的对应关系
    private List<WaveAndTime> passwordList = new List<WaveAndTime>();
    private List<GameObject> passwordGoList = new List<GameObject>();

    //输入器，存储玩家输入的WaveType与time的对应关系
    private List<WaveAndTime> inputList = new List<WaveAndTime>();
    private List<GameObject> inputGoList = new List<GameObject>();
    private int inputIndex = 0; //输入器索引

    //判定线
    private GameObject phigrosLine;

    private void Awake()
    {
        //注册到GameManager中
        GameManager.Instance.musicCheck = this;

        //创建判定线
        phigrosLine = GameObject.Instantiate(Resources.Load<GameObject>("UI/PhigrosLine"), this.gameObject.transform);
        //位置设定
        phigrosLine.transform.localPosition = new Vector3(-400, 0, 0);
    }

    //计时
    private void Update()
    {



        if(allowTimerStart && Timer<=maxTime)
        {
            Timer += Time.deltaTime;
            //更新判定线
            phigrosLine.transform.localPosition = new Vector3(-400 + Timer / maxTime * 800, 0, 0);
        }

        //if (Timer > maxTime)
        //{
        //    ResetInput()
        //}


        //编辑模式，测试使用捏
        if (EditMode)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ReceiveInputWithoutCheck(WaveType.A);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                ReceiveInputWithoutCheck(WaveType.B);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ReceiveInputWithoutCheck(WaveType.C);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MakeInputToPassword();
            }
        }
    }


    //初始化
    public void Init(List<WaveAndTime> ps,int lix,float LevelTime)
    {
        passwordList = ps;
        levelIndex = lix;
        maxTime = LevelTime;

        //我们编辑模式是这样的
        if (EditMode)
        {
            maxTime = 15f;
        }

        if (passwordList == null) return;
        Transform parentTf = GameObject.Find("Password").transform;
        //依据passwordList初始化UI
        for(int i=0;i< passwordList.Count;i++)
        {
            var Go = GameObject.Instantiate(Resources.Load<GameObject>("UI/word" + passwordList[i].waveType), parentTf);
            Color oldColor = Go.GetComponent<Image>().color;
            Go.GetComponent<Image>().color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.3f);
            Go.transform.localPosition = new Vector3(-400+passwordList[i].time/maxTime*800, 0, 0);
            passwordGoList.Add(Go);
        }
    }

    //重置输入器，清理UI，计时归零，安排在按钮上
    public void ResetInput()
    {
        inputList.Clear();
        ClearInputUI();
        Timer = 0f;
        allowTimerStart = false;

        //位置设定
        phigrosLine.transform.localPosition = new Vector3(-400, 0, 0);
    }


    public void ReceiveInput(WaveType type)
    {
        if (EditMode)
        {
            ReceiveInputWithoutCheck(type);
        }
        else
        {
            ReceiveInputWithCheck(type);
        }
    }



    //接受输入并检查是否与密码器在允许范围内匹配
    public void ReceiveInputWithCheck(WaveType waveType)
    {
        //比对时间是否正确，若正确则加入输入器，absult为绝对正确时间
        if (passwordList == null) return;
        float absultTime = passwordList[inputIndex].time;

        //创建UI
        Transform parentTf = GameObject.Find("Input").transform;
        var Go = GameObject.Instantiate(Resources.Load<GameObject>("UI/word" + waveType), parentTf);
        Go.transform.localPosition = new Vector3(-400 + Timer / maxTime * 800, 0, 0);
        inputGoList.Add(Go);

        if (absultTime-faultToleranceTime/2 <= Timer && Timer <= absultTime + faultToleranceTime / 2)
        {
            //时间正确，加入输入器
            WaveAndTime wat = new WaveAndTime();
            wat.waveType = waveType;
            wat.time = Timer;
            inputList.Add(wat);
            inputIndex++;

            //开始首次计时，并启动UI
            if (!allowTimerStart)
            {
                allowTimerStart = true;

            }

           
        }
        //时间错误
        else
        {

        }

        //当前成功解密
        if (inputList.Count == passwordList.Count)
        {
            //由GameManager处理成功事件
            Debug.Log("当前关卡解密成功");
        }

    }

    //接受输入但不与密码器对比，编辑模式使用
    public void ReceiveInputWithoutCheck(WaveType waveType)
    {
        //加入输入器
        WaveAndTime wat = new WaveAndTime();
        wat.waveType = waveType;
        wat.time = Timer;
        inputList.Add(wat);
        inputIndex++;
        //开始首次计时，并启动UI
        if (!allowTimerStart)
        {
            allowTimerStart = true;
        }
        //其他处理

        //创建UI
        Transform parentTf = GameObject.Find("Input").transform;
        var Go = GameObject.Instantiate(Resources.Load<GameObject>("UI/word" + waveType), parentTf);
        Go.transform.localPosition = new Vector3(-400 + Timer / maxTime * 800, 0, 0);
        inputGoList.Add(Go);
    }

    //将输入器转换为密码器，编辑模式使用
    public void MakeInputToPassword()
    {
        PassWord pw = new PassWord();
        pw.levelIndex = levelIndex;
        pw.levelPassWord = new List<WaveAndTime>(inputList);
        pw.levelTime = inputList[inputList.Count - 1].time + 0.5f; //密码时间为最后一个输入时间+0.5秒
        //存储到ScriptableObject中
        List<PassWord> pws = GameManager.Instance.passwordSO.passWords;
        foreach(var item in pws)
        {
            if(item.levelIndex==levelIndex)
            {
                pws.Remove(item);
                break;
            }
        }
        pws.Add(pw);
        Debug.Log(levelIndex.ToString()+"已将输入器转换为密码器");
        //重置UI
        //还没写，干脆就直接返回上一菜单得了
        GameManager.Instance.ChangeGameState(GameState.Level);
    }

    //清理UI
    public void ClearPasswordUI()
    {
        foreach(var go in passwordGoList)
        {
            GameObject.Destroy(go);
        }
        passwordGoList.Clear();
    }

    public void ClearInputUI() 
    { 
        foreach (var go in inputGoList)
        {
            GameObject.Destroy(go);       
        }
        inputGoList.Clear();
    }

    public void DEstroyMusicCheck()
    {
        ClearInputUI();
        ClearPasswordUI();
    }

}
