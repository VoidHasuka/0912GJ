using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接收器
/// </summary>
public class Receiver : MonoBehaviour
{
    [Header("Visual Reaction")]
    public Renderer targetRenderer;     // 2D 可用 SpriteRenderer 也能拿到 .material
    public string expectedSourceTag = "SoundSource"; //声源标签

    Color _origColor;
    float _flashTime = 0.2f;
    float _timer;

    void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null) _origColor = targetRenderer.material.color; 
    }

    // 声源在扩散时调用
    public void OnHeard(GameObject source)
    {


        //Debug.Log("Receiver heard from " + source.name);

        // 识别标签：比起直接比较 .tag，官方推荐 CompareTag（性能更好/更安全）。
        // 确保触发对象不为空且标签匹配为声源
        bool tagMatched = source != null && source.CompareTag(expectedSourceTag);

        // 做点反馈：比如闪一下颜色
        if (tagMatched)
        {
            if (targetRenderer != null)
            {
                targetRenderer.material.color = tagMatched ? Color.green : Color.yellow;
                _timer = _flashTime;
            }
        }

        //Debug.Log($"Receiver heard! Source = {source?.name}, TagOK = {tagMatched}, Tag = {source?.tag}");
    }

    void Update()
    {
        if (_timer > 0f && targetRenderer != null)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                targetRenderer.material.color = _origColor;
            }
        }
    }
}
