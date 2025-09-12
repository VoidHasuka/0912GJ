using UnityEngine;

public class EchoSpawner : MonoBehaviour
{
    public Transform source;     // 声源
    public Transform echoPrefab; // 你的半圆回声 Prefab

    /// <summary>
    /// 对外入口：接收当前命中的 BoxCollider2D，在障碍中心生成回声，
    /// 并让“回声的本地 +Y 轴”垂直于“最面向声源的那条边”（法线指向源侧）。
    /// </summary>
    public void SpawnEchoWithObstacle(BoxCollider2D obstacle)
    {
        if (source == null || echoPrefab == null || obstacle == null)
        {
            Debug.LogError("[EchoSpawner] 参数不足（source/echoPrefab/obstacle）");
            return;
        }

        // 1) 找“最面向声源”的边
        int idxFacing = ObstacleEdgeUtility.GetFacingEdgeIndex(obstacle, source.position);
        ObstacleEdgeUtility.GetEdgeEndpoints(obstacle, idxFacing, out Vector2 E0, out Vector2 E1);
        ObstacleEdgeUtility.EdgeGeom(E0, E1, out Vector2 Emid, out Vector2 Edir, out _);

        // 2) 在障碍中心生成回声
        Vector2 center = obstacle.bounds.center;
        Transform echo = Instantiate(echoPrefab, center, Quaternion.identity);

        // 3) 法线（垂直于边），选“指向声源一侧”的那个
        Vector2 n1 = new Vector2(-Edir.y, Edir.x);
        Vector2 n2 = -n1;
        Vector2 toSrc = ((Vector2)source.position - Emid).normalized;
        Vector2 n = (Vector2.Dot(n1, toSrc) >= 0f) ? n1 : n2;

        // 4) 让“回声本地 +Y 轴”对齐该法线（+Y 对齐角 = 朝向角 - 90°）
        float ang = Mathf.Atan2(n.y, n.x) * Mathf.Rad2Deg;
        echo.rotation = Quaternion.Euler(0, 0, ang - 90f);
    }
}
