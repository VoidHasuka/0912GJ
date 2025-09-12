using UnityEngine;

public class TriangleMaskPlacer : MonoBehaviour
{
    [Header("Inputs")]
    public Transform source;                // 声源
    public SpriteMask triangleMaskPrefab;   // 三角遮罩 Prefab

    [Header("Triangle Sprite 假设")]
    [Tooltip("三角底边沿本地X轴、顶点在本地+Y；若不符，请在Prefab内预旋转。")]
    public bool triangleBaseAlongLocalX = true;

    [Header("尺寸匹配")]
    [Tooltip("三角 sprite 在未缩放时的‘底边宽度’（世界单位），用于匹配障碍对边长度。")]
    public float triangleBaseWorldWidth = 1f;
    [Tooltip("三角高向源侧延伸的比例；实际 sy = sx * heightScale。")]
    public float heightScale = 1.2f;

    [Header("参考点改为‘底边中点’")]
    [Tooltip("从 Sprite 的 pivot 指向‘底边中点(base-mid)’的本地位移向量。\n若你的 Sprite pivot 已在底边中点，此值为 (0,0)。\n例如：若 pivot 在几何中心且单位高为1，底边中点相对质心大约在 (0, -1/3)。")]
    public Vector2 baseMidLocalFromPivot = Vector2.zero;

    [Header("方向控制")]
    [Tooltip("勾选：关于自身x轴对称（等价于本地Y缩放取反）")]
    public bool mirrorAboutLocalX = true;

    /// <summary>
    /// 保留旧签名：仅摆放遮罩（参考点=底边中点）
    /// </summary>
    public void PlaceMaskWithObstacle(BoxCollider2D obstacle)
    {
        PlaceMaskWithObstacle(obstacle, null);
    }

    /// <summary>
    /// 摆放三角遮罩并可选对齐“碰到的那个物体”的 transform（仅位置+旋转）。
    /// 参考点：三角形底边中点（而非 pivot / 质心）。
    /// </summary>
    public void PlaceMaskWithObstacle(BoxCollider2D obstacle, Transform alsoMove)
    {
        if (source == null || triangleMaskPrefab == null || obstacle == null)
        {
            Debug.LogError("[TriangleMaskPlacer] 参数不足（source/triangleMaskPrefab/obstacle）");
            return;
        }

        // 1) 找“最面向声源”的边 → 取其对边作为三角底边
        int idxFacing = ObstacleEdgeUtility.GetFacingEdgeIndex(obstacle, source.position);
        int idxOpp = (idxFacing + 2) % 4;

        // 2) 取对边端点/方向/长度/中点
        ObstacleEdgeUtility.GetEdgeEndpoints(obstacle, idxOpp, out Vector2 B0, out Vector2 B1);
        ObstacleEdgeUtility.EdgeGeom(B0, B1, out Vector2 Bmid, out Vector2 Bdir, out float Blen);

        // 3) 基准旋转：本地+X沿对边方向；若+Y未朝向source则翻180°
        float edgeAng = Mathf.Atan2(Bdir.y, Bdir.x) * Mathf.Rad2Deg;
        Quaternion baseRot = Quaternion.Euler(0, 0, edgeAng);

        Vector2 toSrc = ((Vector2)source.position - Bmid).normalized;
        Vector2 yAxisWorld = new Vector2(-Bdir.y, Bdir.x);
        if (Vector2.Dot(yAxisWorld, toSrc) < 0f)
            baseRot *= Quaternion.Euler(0, 0, 180f);

        // 4) 若需要，也把“碰到的那个物体”的 transform 对齐到这条对边（不改scale）
        if (alsoMove != null)
        {
            alsoMove.position = Bmid;
            alsoMove.rotation = baseRot;
        }

        // 5) 实例化遮罩并做镜像/缩放
        SpriteMask mask = Instantiate(triangleMaskPrefab);
        mask.name = "TriangleMask_AlignedToOppEdge";
        mask.transform.rotation = baseRot;

        float sx = (triangleBaseWorldWidth <= 1e-6f) ? 1f : (Blen / triangleBaseWorldWidth);
        float syAbs = Mathf.Abs(sx * heightScale);
        int mirrorY = mirrorAboutLocalX ? -1 : 1;
        mask.transform.localScale = new Vector3(sx * 0.7f, syAbs * mirrorY * 1.5f, 1f);

        // 6) 位置：以“底边中点”为参考对齐到 Bmid
        //    baseMidLocalFromPivot 是 pivot→底边中点 的本地向量；
        //    镜像后Y轴方向反转，因此其Y分量也随之取反
        Vector2 baseMidLocalAfterMirror = new Vector2(
            baseMidLocalFromPivot.x,
            baseMidLocalFromPivot.y * mirrorY
        );

        // 本地→世界：先缩放（取 syAbs 作尺度），再旋转
        Vector2 worldOffsetPivotToBaseMid = (Vector2)(mask.transform.rotation * new Vector3(
            baseMidLocalAfterMirror.x * sx * 0.5f,
            baseMidLocalAfterMirror.y * syAbs,
            0f
        ));

        // 令 底边中点(world) = Bmid ⇒ pivot(world) 应放在：Bmid - worldOffset
        mask.transform.position = Bmid - worldOffsetPivotToBaseMid;
    }
}
