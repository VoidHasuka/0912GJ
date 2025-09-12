using UnityEngine;

public static class WaveReflector
{
    /// <summary>
    /// 计算点 point 关于通过 linePoint 且方向为 lineDir 的直线的镜像对称点坐标。
    /// </summary>
    public static Vector2 ReflectPointAcrossLine(Vector2 point, Vector2 linePoint, Vector2 lineDir)
    {
        // 确保方向为单位向量
        lineDir.Normalize();
        // 将直线平移使 linePoint 位于原点，计算point相对于该原点的向量
        Vector2 relative = point - linePoint;
        // 计算 relative 在直线方向上的投影点
        float projLength = Vector2.Dot(relative, lineDir);
        Vector2 projPoint = projLength * lineDir;
        // 计算 relative 到直线的垂直向量
        Vector2 perp = relative - projPoint;
        // 镜像后的相对向量 = 投影向量 - 垂直向量
        Vector2 reflectedRelative = projPoint - perp;
        // 平移回原坐标系
        return linePoint + reflectedRelative;
    }

    /// <summary>
    /// 将向量 dir 绕给定轴向量 axis 对称反射。axis 必须是单位向量，表示镜像轴方向。
    /// 返回镜像后的方向向量（与原向量长度相同）。
    /// </summary>
    public static Vector2 ReflectVectorAcrossLine(Vector2 dir, Vector2 axis)
    {
        axis.Normalize();
        // 使用向量反射公式：v_reflect = 2*(v·axis)*axis - v
        Vector2 reflected = 2f * Vector2.Dot(dir, axis) * axis - dir;
        return reflected;
    }
}
