using UnityEngine;

/// <summary>
/// BoxCollider2D 辅助：求世界角点、选“最面向声源”的边、取对边几何等。
/// 纯静态；无需挂载；不创建父子关系。
/// </summary>
public static class ObstacleEdgeUtility
{
    /// <summary>顺时针返回四角：BL, TL, TR, BR（世界坐标）。</summary>
    public static Vector2[] GetWorldCorners(BoxCollider2D box)
    {
        Transform t = box.transform;
        Vector2 o = box.offset;
        float hw = box.size.x * 0.5f, hh = box.size.y * 0.5f;
        Vector2 BL = t.TransformPoint(o + new Vector2(-hw, -hh));
        Vector2 TL = t.TransformPoint(o + new Vector2(-hw, hh));
        Vector2 TR = t.TransformPoint(o + new Vector2(hw, hh));
        Vector2 BR = t.TransformPoint(o + new Vector2(hw, -hh));
        return new[] { BL, TL, TR, BR };
    }

    /// <summary>
    /// 选择“最面向声源”的边索引（0:BL->TL, 1:TL->TR, 2:TR->BR, 3:BR->BL）。
    /// 判定：取边的外法线，并与 (source - 边中点) 的夹角最小（点积最大）。
    /// </summary>
    public static int GetFacingEdgeIndex(BoxCollider2D box, Vector2 sourcePos)
    {
        var c = GetWorldCorners(box);
        Vector2 center = box.bounds.center;
        float best = -1e9f; int bestIdx = 0;

        for (int i = 0; i < 4; i++)
        {
            Vector2 a = c[i];
            Vector2 b = c[(i + 1) % 4];
            Vector2 mid = (a + b) * 0.5f;
            Vector2 dir = (b - a).normalized;

            // 两个候选法线：与矩形中心相反者为“外法线”
            Vector2 nA = new Vector2(dir.y, -dir.x);
            Vector2 nB = new Vector2(-dir.y, dir.x);
            Vector2 n = (Vector2.Dot(nA, (Vector2)center - mid) < 0) ? nA : nB;

            float facing = Vector2.Dot(n.normalized, (sourcePos - mid).normalized);
            if (facing > best) { best = facing; bestIdx = i; }
        }
        return bestIdx;
    }

    /// <summary>取某条边的两个端点（世界），顺时针序。</summary>
    public static void GetEdgeEndpoints(BoxCollider2D box, int edgeIndex, out Vector2 p0, out Vector2 p1)
    {
        var c = GetWorldCorners(box);
        p0 = c[edgeIndex];
        p1 = c[(edgeIndex + 1) % 4];
    }

    /// <summary>给定边端点，求边中点、单位方向与长度。</summary>
    public static void EdgeGeom(Vector2 p0, Vector2 p1, out Vector2 mid, out Vector2 dir, out float len)
    {
        mid = (p0 + p1) * 0.5f;
        dir = (p1 - p0).normalized;
        len = Vector2.Distance(p0, p1);
    }
}
