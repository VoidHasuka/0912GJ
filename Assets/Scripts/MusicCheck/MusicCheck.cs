using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicCheck : MonoBehaviour
{
    private int levelIndex;

    public float faultToleranceTime = 0.2f; // 容错时间
    public float maxTime = 5f;             // 最大时间
    private float Timer = 0f;               //  计时器
    private bool allowTimerStart = false;


    // 密码器，存储WaveType与time的对应关系
    public List<WaveAndTime> passwordList = new List<WaveAndTime>();
    public List<GameObject> passwordGoList = new List<GameObject>();

    //输入器，存储玩家输入的WaveType与time的对应关系
    public List<WaveAndTime> inputList = new List<WaveAndTime>();
    public List<GameObject> inputGoList = new List<GameObject>();
    private int inputIndex = 0; //输入器索引

    private void Awake()
    {
        //注册到GameManager中
        GameManager.Instance.musicCheck = this;
    }

    //计时
    private void Update()
    {
        if(allowTimerStart)
        {
            Timer += Time.deltaTime;
        }


        //编辑模式，测试使用捏
        if(Input.GetKeyDown(KeyCode.A))
        {
            ReceiveInputNoPassword(WaveType.A);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ReceiveInputNoPassword(WaveType.B);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ReceiveInputNoPassword(WaveType.C);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MakeInputToPassword();
        }
    }


    //初始化
    public void Init(List<WaveAndTime> ps,int lix)
    {
        passwordList = ps;
        levelIndex = lix;


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
        Timer = 0f;
        allowTimerStart = false;
    }

    //接受输入并检查是否与密码器在允许范围内匹配
    public void ReceiveInput(WaveType waveType)
    {
        //比对时间是否正确，若正确则加入输入器，absult为绝对正确时间
        float absultTime = passwordList[inputIndex].time;
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

            //其他处理如UI创建等
        }
        //时间错误
        else
        {

        }

        //当前成功解密
        if (inputList.Count == passwordList.Count)
        {
            //由GameManager处理成功事件
        }

    }

    //接受输入但不与密码器对比，编辑模式使用
    public void ReceiveInputNoPassword(WaveType waveType)
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
