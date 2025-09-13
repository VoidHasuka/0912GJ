using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TriangleMaskArea : MonoBehaviour
{
    // 所有激活中的 TriangleMaskArea
    public static readonly HashSet<TriangleMaskArea> Active = new HashSet<TriangleMaskArea>();

    // 三角形三个世界坐标点
    public Vector2 A, B, C;

    void OnEnable() { Active.Add(this); }
    void OnDisable() { Active.Remove(this); }

    /// <summary>由摆放器在实例化/更新时设置世界三点</summary>
    public void SetTriangleWorld(Vector2 a, Vector2 b, Vector2 c)
    {
        A = a; B = b; C = c;
    }

    public bool ContainsCircleFully(CircleCollider2D circle)
    {
        if (circle == null) return false;

        // 世界中心与半径（Circle 在非等比缩放下，Unity 会用 max(|sx|,|sy|) 缩放半径）
        var t = circle.transform;
        Vector2 centerWorld = t.TransformPoint(circle.offset);
        float rWorld = circle.radius * Mathf.Max(Mathf.Abs(t.lossyScale.x), Mathf.Abs(t.lossyScale.y));

        // 三条边都满足“内侧且距离 >= r”即可判定整圆在三角内
        return InsideWithMargin(centerWorld, rWorld, A, B, C) &&
               InsideWithMargin(centerWorld, rWorld, B, C, A) &&
               InsideWithMargin(centerWorld, rWorld, C, A, B);
    }

    /// <summary>
    /// 判断点 p 是否位于边 ab 的“内侧”，并且到该边的有符号距离 ≥ r。
    /// 通过使用“对点 o”（第三个顶点）确定边的内外侧，不依赖三角形顺/逆时针。
    /// Sign(x,a,b) 的绝对值 = |ab| * 点到直线的距离；符号表示在 ab 的哪一侧。
    /// </summary>
    private static bool InsideWithMargin(Vector2 p, float r, Vector2 a, Vector2 b, Vector2 o)
    {
        float sO = Sign(o, a, b);                // 对点在 ab 的哪一侧 = “内侧”
        float sP = Sign(p, a, b);                // 圆心在 ab 的哪一侧
        float edgeLen = (b - a).magnitude;
        float need = r * edgeLen;                // 需要的“有符号面积值”= r * |ab|

        // 若 o 在“正侧”，则圆心也必须在正侧且 sP ≥ need；反之亦然
        if (sO >= 0f) return sP >= need;
        else return sP <= -need;
    }

    // 你已有的工具：Sign/PointInTri 等保持不变

    // --------- helpers ---------

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
