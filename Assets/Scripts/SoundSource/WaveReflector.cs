using UnityEngine;

public static class WaveReflector
{
    /// <summary>
    /// ����� point ����ͨ�� linePoint �ҷ���Ϊ lineDir ��ֱ�ߵľ���ԳƵ����ꡣ
    /// </summary>
    public static Vector2 ReflectPointAcrossLine(Vector2 point, Vector2 linePoint, Vector2 lineDir)
    {
        // ȷ������Ϊ��λ����
        lineDir.Normalize();
        // ��ֱ��ƽ��ʹ linePoint λ��ԭ�㣬����point����ڸ�ԭ�������
        Vector2 relative = point - linePoint;
        // ���� relative ��ֱ�߷����ϵ�ͶӰ��
        float projLength = Vector2.Dot(relative, lineDir);
        Vector2 projPoint = projLength * lineDir;
        // ���� relative ��ֱ�ߵĴ�ֱ����
        Vector2 perp = relative - projPoint;
        // ������������� = ͶӰ���� - ��ֱ����
        Vector2 reflectedRelative = projPoint - perp;
        // ƽ�ƻ�ԭ����ϵ
        return linePoint + reflectedRelative;
    }

    /// <summary>
    /// ������ dir �Ƹ��������� axis �ԳƷ��䡣axis �����ǵ�λ��������ʾ�����᷽��
    /// ���ؾ����ķ�����������ԭ����������ͬ����
    /// </summary>
    public static Vector2 ReflectVectorAcrossLine(Vector2 dir, Vector2 axis)
    {
        axis.Normalize();
        // ʹ���������乫ʽ��v_reflect = 2*(v��axis)*axis - v
        Vector2 reflected = 2f * Vector2.Dot(dir, axis) * axis - dir;
        return reflected;
    }
}
