using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMove : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform parentTf;       // 要移动的容器（例如一个水平列表的父节点）
    public RectTransform selectBGUITf;   // 可选：你的选中高亮，不参与计算也行

    [Header("Bounds (anchored X)")]
    public float minX = -800f;          // 最左（更小的 X）
    public float maxX = 0f;              // 最右（更大的 X）

    [Header("Motion")]
    public float moveSpeed = 800f;       // 像素/秒
    public bool ignoreTimeScale = true;  // 过场/暂停时是否仍然移动

    // -1: 向左移动容器（看起来是“向右滚动内容”）; +1: 向右移动容器
    int _dir = 0;

    public float dirX = 0f;

    void Start()
    {
        if (parentTf == null)
            parentTf = GameManager.Instance.uiManager.parent.GetComponent<RectTransform>();
        if (selectBGUITf == null)
            selectBGUITf = GameManager.Instance.uiManager.selectBGUI.GetComponent<RectTransform>();

        // 确保 min/max 合法
        if (minX > maxX)
        {
            float t = minX; minX = maxX; maxX = t;
        }
    }

    void Update()
    {
        if (_dir == 0 || parentTf == null) return;

        float dt = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        float newX = parentTf.anchoredPosition.x + _dir * moveSpeed * dt;
        newX = Mathf.Clamp(newX, minX, maxX);
        parentTf.anchoredPosition = new Vector2(newX, parentTf.anchoredPosition.y);
        selectBGUITf.anchoredPosition = new Vector2(newX+dirX,selectBGUITf.anchoredPosition.y);
    }

    // —— 绑定到“右箭头按钮”的 OnPointerDown（或 EventTrigger PointerDown）
    public void OnClickDown_MoveToRight()
    {
        // 你的注释写的是“实质左移”，所以这里让容器向左（X 变小）
        _dir = -1;
    }

    // —— 绑定到“左箭头按钮”的 OnPointerDown
    public void OnClickDown_MoveToLeft()
    {
        // 容器向右（X 变大）
        _dir = +1;
    }

    // —— 绑定到两个按钮的 OnPointerUp / OnPointerExit
    public void OnPointerUp_Stop()
    {
        _dir = 0;
    }

    // 可选：一步一格的点击移动（不是长按）
    public float step = 300f;
    public void ClickStepRight()
    {
        float newX = Mathf.Clamp(parentTf.anchoredPosition.x - step, minX, maxX);
        parentTf.anchoredPosition = new Vector2(newX, parentTf.anchoredPosition.y);
    }
    public void ClickStepLeft()
    {
        float newX = Mathf.Clamp(parentTf.anchoredPosition.x + step, minX, maxX);
        parentTf.anchoredPosition = new Vector2(newX, parentTf.anchoredPosition.y);
    }
}
