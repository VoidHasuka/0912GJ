using UnityEngine;

public class Receiver : MonoBehaviour
{
    private SpriteRenderer sr;
    // 碰撞进入时的反馈颜色
    public Color triggeredColor = Color.red;
    // 原始颜色
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入触发范围的对象是否是波段（通过Tag识别）
        if (other.CompareTag("Wave"))
        {
            Debug.Log($"{name} 收到了波段碰撞: {other.name}");
            if (sr != null)
            {
                //// 将接收器着色为反馈颜色
                //sr.color = triggeredColor;
                //// 短暂延时后恢复原色
                //Invoke(nameof(ResetColor), 0.5f);
            }

            // 检测到是接收器
            if (other.GetComponent<WavePropagation2D>() != null)
            {
                //Debug.Log("具有 WavePropagation2D 组件");
                //执行逻辑

                //音效播放
                GameManager.Instance.audioManager.PlayWaveAudio(other.GetComponent<WavePropagation2D>().itsWaveType);
                //UI更新
                GameManager.Instance.musicCheck.ReceiveInput(other.GetComponent<WavePropagation2D>().itsWaveType);
            }
        }

        //检测进入触发范围的对象是否是回声（通过Tag识别）
        else if (other.CompareTag("Echo"))
        {
            Debug.Log($"{name} 收到了回声碰撞: {other.name}");
            if (sr != null)
            {
                // 将接收器着色为反馈颜色
                sr.color = triggeredColor;
                // 短暂延时后恢复原色
                Invoke(nameof(ResetColor), 0.5f);
            }
            // 检测到是回声
            //if (other.GetComponent<Echo>() != null)
            //{
            //    //执行逻辑
            //    //音效播放
            //    //UI更新
            //    GameManager.Instance.musicCheck.ReceiveInput(other.GetComponent<Echo>().itsWaveType);
            //}
        }
    }

    void ResetColor()
    {
        if (sr != null)
        {
            sr.color = originalColor;
        }
    }
}
