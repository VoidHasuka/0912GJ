using UnityEngine;
using UnityEngine.UI;

public class WorldToUIFollower : MonoBehaviour
{
    [Header("绑定：世界物体 & UI")]
    public Transform worldTarget;           // 要跟随的世界物体
    public RectTransform uiIcon;            // UI 图标/血条等
    public Canvas canvas;                   // 该 UI 所在 Canvas（ScreenSpace-Overlay/Camera 都可）

    [Header("可选")]
    public Vector2 uiOffset;                // UI 上的像素偏移（比如向上 30）
    public bool hideWhenBehindCamera = true;

    Camera cam;
    RectTransform canvasRect;

    void Awake()
    {
        if (!canvas) canvas = uiIcon.GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
              ? null                       // Overlay 模式不需要相机
              : (canvas.worldCamera ? canvas.worldCamera : Camera.main);
    }

    void LateUpdate()
    {
        if (!worldTarget || !uiIcon) return;

        // 将世界坐标转屏幕坐标
        Vector3 screenPos = (cam ? cam.WorldToScreenPoint(worldTarget.position)
                                 : Camera.main.WorldToScreenPoint(worldTarget.position));

        // 在相机后方：z < 0
        if (hideWhenBehindCamera && screenPos.z < 0f)
        {
            if (uiIcon.gameObject.activeSelf) uiIcon.gameObject.SetActive(false);
            return;
        }
        else if (!uiIcon.gameObject.activeSelf) uiIcon.gameObject.SetActive(true);

        // 屏幕坐标 -> Canvas 本地坐标
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, (Vector2)screenPos, cam, out localPoint);

        uiIcon.anchoredPosition = localPoint + uiOffset;
    }
}
