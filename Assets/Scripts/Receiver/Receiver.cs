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
                // 将接收器着色为反馈颜色
                sr.color = triggeredColor;
                // 短暂延时后恢复原色
                Invoke(nameof(ResetColor), 0.5f);
            }
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
