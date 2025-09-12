using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载到声源物体上
/// </summary>
public class SoundSource : MonoBehaviour
{
    //注册到SoundSourceManager中
    private void Start()
    {
        if(GameManager.Instance.soundSourceManager.soundSources!=null)
        {
            GameManager.Instance.soundSourceManager.soundSources.Add(this);
            //Debug.Log("注册声源:" + this.name);
        }
    }

    [Header("Audio")]
    public AudioSource audioSource;            // 拖引用
    public bool playAudioOnEmit = true;

    [Header("Wave Settings")]
    public Transform waveVisual;               // 可选：波纹可视化节点（圆形Sprite/LineRenderer根） 波纹对象的Transform
    public float maxRadius = 10f;              // 最大扩散半径
    public float expandSpeed = 8f;             // 半径扩散速度（单位：单位/秒）
    public LayerMask receiverLayer;            // 接收器所在层（可选：留空代表全部）
    public float hitCooldownPerReceiver = 0.2f;// 同一接收器命中去抖，暂未使用

    // 运行时存储碰撞体，避免重复触发
    private HashSet<Collider2D> _hitThisPulse = new HashSet<Collider2D>();

    // 播放声音并开始扩散
    public void Emit()
    {
        if (playAudioOnEmit && audioSource != null)
        {
            audioSource.Play(); // 播放声音
        }
        StopAllCoroutines();
        StartCoroutine(EmitRoutine());
    }

    //协程扩散
    private IEnumerator EmitRoutine()
    {

        _hitThisPulse.Clear();
        float radius = 0f;

        // 可视化初始化
        if (waveVisual != null) waveVisual.localScale = Vector3.zero;

        while (radius < maxRadius)
        {
            radius += expandSpeed * Time.deltaTime;

            // 1) 命中检测：当前半径内的所有 Collider2D
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, receiverLayer);
            // 说明：OverlapCircleAll 会返回圆内所有2D碰撞体。频繁使用可考虑 NonAlloc 版本优化。:contentReference[oaicite:5]{index=5}

            foreach (var col in hits)
            {

                if (_hitThisPulse.Contains(col)) continue; // 避免本次脉冲重复触发
                //Debug.Log("接受");
                _hitThisPulse.Add(col);

                // 通知接收器
                var receiver = col.GetComponent<Receiver>();
                if (receiver != null)
                {
                    receiver.OnHeard(gameObject); //触发接收器接受检测并把“声源对象”传给接收器
                }
            }

            // 2) 可视化：缩放一个圆形Sprite/圆环
            if (waveVisual != null)
            {
                float visualScale = radius * 2f; // 假设贴图单位直径=1，这里把scale按直径算
                waveVisual.localScale = new Vector3(visualScale, visualScale, 1f);
            }

            yield return null; // 下一帧继续扩散
        }

        // 可选：扩散结束后隐藏可视化
        if (waveVisual != null) waveVisual.localScale = Vector3.zero;
    }

    // 可选：支持“延时发声”
    public void EmitWithDelay(float delaySeconds)
    {
        StartCoroutine(EmitDelay(delaySeconds));
    }

    private IEnumerator EmitDelay(float t)
    {
        yield return new WaitForSeconds(t); // 协程里等待 t 秒（受 Time.timeScale 影响）。:contentReference[oaicite:6]{index=6}
        Emit();
    }
}
