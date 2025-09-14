using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFixAspect : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // 目标比例（16:9）
        float targetAspect = 16f / 9f;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f) // 屏幕更高（如 16:10）
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f; // 上下留黑边
            cam.rect = rect;
        }
        else // 屏幕更宽
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f; // 左右留黑边
            rect.y = 0;
            cam.rect = rect;
        }
    }
}
