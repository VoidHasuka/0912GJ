using UnityEngine;

public class TriangleMaskPlacer : MonoBehaviour
{
    [Header("Inputs")]
    public Transform source;
    public SpriteMask triangleMaskPrefab;

    [Header("Triangle Sprite 假设")]
    [Tooltip("三角底边沿本地X轴、顶点在本地+Y；若不符，请在Prefab内预旋转。")]
    public bool triangleBaseAlongLocalX = true;

    [Header("尺寸匹配")]
    [Tooltip("三角 sprite 底边的世界宽度（用于匹配障碍对边长度）。")]
    public float triangleBaseWorldWidth = 1f;
    [Tooltip("三角高向源侧延伸的比例。")]
    public float heightScale = 1.2f;

    [Header("几何中心（质心）设置")]
    [Tooltip("当前 Sprite 的 pivot 相对三角形几何中心(质心)的本地偏移；若Sprite的pivot已在几何中心，则(0,0)。\n典型：若pivot在底边中点且apex在y=1，则质心在(0,1/3)，此处应填(0, +1/3)。")]
    public Vector2 centroidLocalOffsetFromPivot = Vector2.zero;

    [Header("方向控制")]
    [Tooltip("勾选后：关于自身X轴对称（等价于本地Y轴取反）。")]
    public bool mirrorAboutLocalX = true;

    /// <summary>
    /// 将三角形底边对齐到“最面向 source 的边”的对边，且三角形几何中心对齐到对边中点。
    /// 若 mirrorAboutLocalX=true，则在对齐完成后再关于自身x轴对称（本地Y翻转），
    /// 并结合镜像修正质心位置，保证位置仍然对齐。
    /// </summary>
    public void PlaceMaskWithObstacle(BoxCollider2D obstacle)
    {
        if (source == null || triangleMaskPrefab == null || obstacle == null)
        {
            Debug.LogError("[TriangleMaskPlacer] 参数不足（source/triangleMaskPrefab/obstacle）");
            return;
        }

        // 1) 找“最面向声源”的边 → 取其对边作为三角底边
        int idxFacing = ObstacleEdgeUtility.GetFacingEdgeIndex(obstacle, source.position);
        int idxOpp = (idxFacing + 2) % 4;

        // 取对边端点/方向/长度/中点
        ObstacleEdgeUtility.GetEdgeEndpoints(obstacle, idxOpp, out Vector2 B0, out Vector2 B1);
        ObstacleEdgeUtility.EdgeGeom(B0, B1, out Vector2 Bmid, out Vector2 Bdir, out float Blen);

        // 2) 实例化遮罩
        SpriteMask mask = Instantiate(triangleMaskPrefab);
        mask.name = "TriangleMask_AlignedToOppEdge";
        mask.transform.position = Bmid;

        // 3) 旋转：本地X 对齐对边方向；若本地+Y 未朝向声源，则反相
        float edgeAng = Mathf.Atan2(Bdir.y, Bdir.x) * Mathf.Rad2Deg;
        mask.transform.rotation = Quaternion.Euler(0, 0, edgeAng);

        Vector2 toSrc = ((Vector2)source.position - Bmid).normalized;
        Vector2 yAxisWorld = new Vector2(-Bdir.y, Bdir.x);
        if (Vector2.Dot(yAxisWorld, toSrc) < 0f)
        {
            mask.transform.rotation *= Quaternion.Euler(0, 0, 180f);
            yAxisWorld = -yAxisWorld;
        }

        // 4) 缩放：底边长度匹配障碍对边
        float sx = (triangleBaseWorldWidth <= 1e-6f) ? 1f : (Blen / triangleBaseWorldWidth);
        float sy = sx * heightScale;

        // 5) 如需“关于自身x轴对称”，等价于把本地Y缩放取反
        //    注意：镜像会使本地坐标系的 +Y 反向，因此后续质心偏移的Y也要同向取反
        int mirrorY = mirrorAboutLocalX ? -1 : 1;
        mask.transform.localScale = new Vector3(sx, sy * mirrorY, 1f);

        // 6) 质心对齐修正（镜像后也精确对齐到 Bmid）
        //    centroidLocalOffsetFromPivot 是“pivot 到 质心”的本地向量。
        //    镜像后，其Y分量随本地Y翻转而改变，等价于 (x, y*mirrorY)。
        Vector2 centroidLocalAfterMirror = new Vector2(
            centroidLocalOffsetFromPivot.x,
            centroidLocalOffsetFromPivot.y * mirrorY
        );

        Vector2 worldOffset = (Vector2)(mask.transform.rotation * new Vector3(
            centroidLocalAfterMirror.x * sx,
            centroidLocalAfterMirror.y * Mathf.Abs(sy), // 取绝对值：长度缩放不关心镜像符号
            0f
        ));
        // 期望：pivot + worldOffset == Bmid →  pivot 应移动到 Bmid - worldOffset
        mask.transform.position = Bmid - worldOffset;

        // 说明：若你的 Sprite pivot 已在几何中心，可将 centroidLocalOffsetFromPivot 设为(0,0)。
        //       镜像开关不会改变“对齐到对边中点”的结果。
    }
}
