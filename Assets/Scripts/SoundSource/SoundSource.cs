using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载到声源物体上
/// </summary>
public class SoundSource : MonoBehaviour
{


    [Header("Audio")]
    public AudioSource audioSource;            // 拖引用
    public bool playAudioOnEmit = true;

    [Header("Wave Settings")]
    public GameObject wavePrefab;              // 声波可视化预制体
    public float originalRadius = 0.1f;        // 初始扩散半径，当发射器较大时可调大，以保证从表面开始扩散
    public float maxRadius = 10f;              // 最大扩散半径
    public WaveType waveType;                  // 声波类型

    //注册到SoundSourceManager中
    private void Start()
    {
        if(GameManager.Instance.soundSourceManager.soundSources!=null)
        {
            GameManager.Instance.soundSourceManager.soundSources.Add(this);
            //Debug.Log("注册声源:" + this.name);
        }
    }

    // 运行时存储碰撞体，避免重复触发
    private HashSet<Collider2D> _hitThisPulse = new HashSet<Collider2D>();

    // 播放声音并开始扩散
    // 发射声波：供UI按钮调用
    public void EmitWave()
    {
        if (wavePrefab == null)
        {
            Debug.LogWarning("WaveEmitter: 未设置 wavePrefab 引用！");
            return;
        }
        // 在声源位置生成波段对象
        GameObject waveObj = Instantiate(wavePrefab, transform.position, Quaternion.identity);
        // 可选：设置波段对象的标记，便于接收器识别
        waveObj.tag = "Wave";
        // 初始化波段（非反射，正常波）
        WaveSegment waveSegment = waveObj.GetComponent<WaveSegment>();
        if (waveSegment != null)
        {
            waveSegment.isReflection = false;
            // 可根据需要设置波段扩散速度等参数，这里使用预制体默认值
        }
    }
}
