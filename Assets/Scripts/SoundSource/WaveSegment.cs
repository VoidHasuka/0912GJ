using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class WaveSegment : MonoBehaviour
{
    // 波段扩散速度（每秒放大的比例）
    private float expansionSpeed = 5f;
    // 最大存活时间（秒），超出后自动销毁
    private float lifeTime = 5f;
    // 标记是否为镜像反射产生的波段
    [HideInInspector] public bool isReflection = false;
    // 内部计时
    private float age = 0f;
    // 表示是否已经发生过一次反射
    private bool hasReflected = false;
    // 初始PolygonCollider2D形状的顶点集合（单位圆形，多边形近似）
    private List<Vector2> baseCirclePoints;
    // PolygonCollider2D组件
    private PolygonCollider2D polyCol;

    void Awake()
    {
        polyCol = GetComponent<PolygonCollider2D>();
        // 初始化PolygonCollider2D为圆形轮廓（近似圆形的多边形）
        InitializeCirclePolygon();
        // 设置刚体属性：波段使用Kinematic刚体，这样只触发事件不受物理力影响
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.simulated = true;
            rb.useFullKinematicContacts = true;
            rb.gravityScale = 0;
        }
        // 将碰撞体设置为触发器，以便触发Enter事件而不发生物理阻挡
        polyCol.isTrigger = true;
    }

    private void Start()
    {
        //初始化声波速度
        expansionSpeed = GameManager.Instance.expendSpeed;
        //初始化声波寿命
        lifeTime = GameManager.Instance.LifeTime;
    }

    void Update()
    {
        // 随时间扩大波纹（修改缩放实现扩散）
        float scaleDelta = expansionSpeed * Time.deltaTime;
        transform.localScale += new Vector3(scaleDelta, scaleDelta, 0f);

        age += Time.deltaTime;
        if (age >= lifeTime)
        {
            Destroy(gameObject); // 超过寿命销毁波段对象
        }
    }

    // 初始化多边形碰撞器为单位圆近似形状
    void InitializeCirclePolygon(int segments = 60)
    {
        baseCirclePoints = new List<Vector2>();
        float angleStep = 2 * Mathf.PI / segments;
        float radius = 0.5f; // 默认Circle精灵直径为1单位，因此半径0.5
        for (int i = 0; i < segments; i++)
        {
            float theta = i * angleStep;
            baseCirclePoints.Add(new Vector2(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius));
        }
        polyCol.SetPath(0, baseCirclePoints.ToArray());
    }

    // 当波段进入触发区域时调用（障碍物或接收器）
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否碰到障碍物（通过Tag或组件类型识别）
        if (!hasReflected && other.CompareTag("Obstacle"))
        {
            Debug.Log($"波段 {name} 碰到障碍物 {other.name}");
            // 计算被遮挡部分的角度范围，并产生镜像波段
            BoxCollider2D obstacle = other as BoxCollider2D;
            if (obstacle != null)
            {
                ReflectWave(obstacle);
            }
        }
        // 接收器的碰撞逻辑由Receiver脚本自身处理（Receiver脚本将检测 "Wave" 标签）
    }

    // 波段镜像反射逻辑：计算被障碍遮挡的角度段，生成新的反射波段
    void ReflectWave(BoxCollider2D obstacle)
    {
        hasReflected = true; // 标记已反射，防止重复反射

        // 获取障碍物中心和主轴方向（长边方向单位向量）
        Transform obsTransform = obstacle.transform;
        Vector2 obsCenter = obsTransform.position;
        // 通过比较BoxCollider2D尺寸确定长边方向（假定BoxCollider2D无缩放或各向均匀缩放）
        Vector2 mainAxis;
        if (obstacle.size.x >= obstacle.size.y)
            mainAxis = obsTransform.right.normalized;   // 水平长边为主轴
        else
            mainAxis = obsTransform.up.normalized;      // 垂直长边为主轴

        // 1. 计算波源（当前波段中心）相对于障碍物的遮挡角度范围
        Vector2 sourcePos = transform.position;
        // 获取障碍物四个角和四条边中心点的世界坐标
        Vector2 halfSize = obstacle.size * 0.5f;
        // 考虑障碍物旋转，将局部坐标角转换到世界
        Vector2 corner1 = obsCenter + mainAxis * halfSize.x + (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 corner2 = obsCenter + mainAxis * halfSize.x - (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 corner3 = obsCenter - mainAxis * halfSize.x + (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 corner4 = obsCenter - mainAxis * halfSize.x - (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        // 边中心点（障碍物上下左右中点）
        Vector2 upMid = obsCenter + (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 downMid = obsCenter - (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 rightMid = obsCenter + mainAxis * halfSize.x;
        Vector2 leftMid = obsCenter - mainAxis * halfSize.x;
        // 收集这些关键点相对于波源的方向角
        List<float> angles = new List<float>();
        Vector2[] points = new Vector2[] { corner1, corner2, corner3, corner4, upMid, downMid, leftMid, rightMid };
        foreach (Vector2 pt in points)
        {
            Vector2 dir = pt - (Vector2)sourcePos;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // 转换到0~360度范围
            if (ang < 0) ang += 360f;
            angles.Add(ang);
        }
        // 也计算从波源指向障碍物中心的角度（中心轴方向）
        Vector2 centerDir = obsCenter - sourcePos;
        float centerAng = Mathf.Atan2(centerDir.y, centerDir.x) * Mathf.Rad2Deg;
        if (centerAng < 0) centerAng += 360f;
        angles.Sort();
        // 找到围绕 centerAng 的两个相邻方向边界（障碍物遮挡的左右边缘角）
        int insertIndex = angles.FindIndex(a => a > centerAng);
        // 如果centerAng大于列表中所有角度，则插入点为0
        if (insertIndex < 0) insertIndex = angles.Count;
        // 相邻边界：prevAng和nextAng（注意处理列表循环）
        float prevAng = angles[(insertIndex - 1 + angles.Count) % angles.Count];
        float nextAng = angles[insertIndex % angles.Count];
        // 确保 prevAng 是遮挡扇形起始角，nextAng 是结束角（使得centerAng落入该扇形内部）
        // 计算直接从prevAng到nextAng经过的角度
        float directDiff = (nextAng >= prevAng) ? (nextAng - prevAng) : (360f - prevAng + nextAng);
        // 检查centerAng是否在此direct扇形内
        bool centerInDirect = false;
        if (prevAng < nextAng)
            centerInDirect = (centerAng >= prevAng && centerAng <= nextAng);
        else
            centerInDirect = (centerAng >= prevAng || centerAng <= nextAng);
        // 若centerAng不在directDiff范围内，则反转边界（交换prevAng和nextAng）
        if (!centerInDirect)
        {
            float temp = prevAng;
            prevAng = nextAng;
            nextAng = temp;
            // 重新计算directDiff为另一方向
            directDiff = (nextAng >= prevAng) ? (nextAng - prevAng) : (360f - prevAng + nextAng);
        }
        // 此时prevAng和nextAng定义了被遮挡的角度范围（包含centerAng）
        float blockedStartAng = prevAng;
        float blockedEndAng = nextAng;
        float blockedArcAngle = directDiff;
        // 2. 从障碍物主轴获取镜像对称位置和角度范围
        // 计算波源相对于障碍轴的镜像点（反射源位置）
        Vector2 mirrorSourcePos = WaveReflector.ReflectPointAcrossLine(sourcePos, obsCenter, mainAxis);
        // 计算被遮挡角度范围的镜像角度（相对于世界坐标）
        // 取遮挡范围的边界方向向量并镜像
        Vector2 vStart = new Vector2(Mathf.Cos(blockedStartAng * Mathf.Deg2Rad), Mathf.Sin(blockedStartAng * Mathf.Deg2Rad));
        Vector2 vEnd = new Vector2(Mathf.Cos(blockedEndAng * Mathf.Deg2Rad), Mathf.Sin(blockedEndAng * Mathf.Deg2Rad));
        // 将方向向量分别绕障碍物轴对称
        Vector2 vStartRef = WaveReflector.ReflectVectorAcrossLine(vStart, mainAxis);
        Vector2 vEndRef = WaveReflector.ReflectVectorAcrossLine(vEnd, mainAxis);
        float reflectStartAng = Mathf.Atan2(vStartRef.y, vStartRef.x) * Mathf.Rad2Deg;
        float reflectEndAng = Mathf.Atan2(vEndRef.y, vEndRef.x) * Mathf.Rad2Deg;
        if (reflectStartAng < 0) reflectStartAng += 360f;
        if (reflectEndAng < 0) reflectEndAng += 360f;
        // 确保镜像角度范围与原遮挡扇形角度大小一致
        float diffRef = 0f;
        if (reflectStartAng <= reflectEndAng)
            diffRef = reflectEndAng - reflectStartAng;
        else
            diffRef = 360f - reflectStartAng + reflectEndAng;
        // 若方向反了（得到的diffRef不是原blockedArcAngle），则交换以取另一侧弧
        if (Mathf.Abs(diffRef - blockedArcAngle) > 1e-2)
        {
            // 交换
            float tempAng = reflectStartAng;
            reflectStartAng = reflectEndAng;
            reflectEndAng = tempAng;
            if (reflectStartAng < 0) reflectStartAng += 360f;
            if (reflectEndAng < 0) reflectEndAng += 360f;
            // 重新计算差值
            if (reflectStartAng <= reflectEndAng)
                diffRef = reflectEndAng - reflectStartAng;
            else
                diffRef = 360f - reflectStartAng + reflectEndAng;
        }
        // 3. 更新当前波段的碰撞体，使其不再覆盖被遮挡的扇形区域（生成“缺口”形状）
        CreateSectorShape(polyCol, blockedStartAng, blockedEndAng, removeSector: true);
        // 4. 生成新的镜像反射波段对象
        GameObject newWaveObj = Instantiate(gameObject, mirrorSourcePos, Quaternion.identity);
        // 设置镜像波段属性
        WaveSegment newWave = newWaveObj.GetComponent<WaveSegment>();
        if (newWave != null)
        {
            newWave.isReflection = true;
            newWave.hasReflected = true; // 镜像波段不再进行二次反射
            // 将镜像波段的初始扩散半径设置为当前波段半径，从对称位置继续扩散
            newWaveObj.transform.localScale = transform.localScale;
            // 更新镜像波段的碰撞形状，仅保留原遮挡扇形对应的区域
            newWave.CreateSectorShape(newWave.polyCol, reflectStartAng, reflectEndAng, removeSector: false);
        }
    }

    // 根据起止角度创建部分圆形（扇形）碰撞形状
    // removeSector=true 表示移除该角度范围的扇形（保留其余部分）；false表示仅保留该扇形（移除其他部分）
    public void CreateSectorShape(PolygonCollider2D collider, float angleStart, float angleEnd, bool removeSector)
    {
        // 确保角度在0-360范围内
        if (angleStart < 0) angleStart += 360f;
        if (angleEnd < 0) angleEnd += 360f;
        angleStart %= 360f;
        angleEnd %= 360f;
        // 拷贝基础圆形顶点，用于计算
        if (baseCirclePoints == null || baseCirclePoints.Count == 0)
        {
            Debug.LogWarning("WaveSegment: 基础圆形顶点未初始化！");
            return;
        }
        List<Vector2> newPoints = new List<Vector2>();
        // 定义一个函数用于获取角度对应向量
        System.Func<float, Vector2> pointOnCircle = (deg) =>
        {
            float rad = deg * Mathf.Deg2Rad;
            // 注意：baseCirclePoints默认半径0.5，对应transform.scale=1
            // 因此用单位圆0.5半径即可，缩放由transform控制
            return new Vector2(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f);
        };
        // 规范化角度差计算
        float diff = 0f;
        bool wraps = false;
        if (angleStart <= angleEnd)
            diff = angleEnd - angleStart;
        else
        {
            diff = 360f - angleStart + angleEnd;
            wraps = true;
        }
        // 如果需要移除指定扇形，则保留其余部分；反之保留指定扇形
        bool invert = removeSector;
        // 我们总是按逆时针顺序生成多边形顶点
        if (!invert)
        {
            // 保留 angleStart->angleEnd 之间的扇形
            // 添加中心点
            newPoints.Add(Vector2.zero);
            // 添加起始边沿点
            newPoints.Add(pointOnCircle(angleStart));
            // 沿圆弧采样点直到结束角
            float stepAngle = 360f / baseCirclePoints.Count;
            float current = angleStart;
            while (true)
            {
                // 前进步长
                current += stepAngle;
                if (wraps)
                {
                    if (current >= 360f)
                        current -= 360f;
                    // 当即将越过angleEnd时跳出
                    if (current >= angleEnd && current < angleStart)
                        break;
                }
                else
                {
                    if (current > angleEnd) break;
                }
                newPoints.Add(pointOnCircle(current));
            }
            // 添加结束边沿点
            newPoints.Add(pointOnCircle(angleEnd));
            // PolygonCollider2D 自动闭合路径，因此最后回到中心点闭合形成扇形
        }
        else
        {
            // 移除 angleStart->angleEnd 扇形，保留其余部分
            // 形状将是一个“缺口”圆（即 Pac-Man 形状）
            // 添加圆周上结束边界的外侧点
            newPoints.Add(pointOnCircle(angleEnd));
            // 沿圆弧添加点（绕过被移除部分）
            float stepAngle = 360f / baseCirclePoints.Count;
            float current = angleEnd;
            while (true)
            {
                current += stepAngle;
                if (current >= 360f) current -= 360f;
                // 当即将进入被移除扇形时跳过至另一边
                // 如果angleStart<angleEnd（不跨0点），则当 current 超过 angleStart 跳出循环
                if (!wraps)
                {
                    if (current > angleStart && current < angleEnd)
                        break;
                }
                else
                {
                    // 如果扇形跨越0度，则被移除区间是 angleStart->360 和 0->angleEnd
                    // open区间为 angleEnd->angleStart
                    if (current > angleStart && current < angleEnd)
                    {
                        // 不太可能进入此条件，因为wraps情况open区间经过0
                    }
                    // 若current回到起点附近超出开放区间
                    if ((angleEnd < angleStart && current > angleStart) ||
                        (angleEnd < angleStart && current < angleEnd))
                    {
                        break;
                    }
                }
                newPoints.Add(pointOnCircle(current));
            }
            // 添加圆周上起始边界的外侧点
            newPoints.Add(pointOnCircle(angleStart));
            // 最后，添加中心点闭合缺口
            newPoints.Add(Vector2.zero);
        }
        // 更新PolygonCollider的形状顶点
        collider.SetPath(0, newPoints.ToArray());
    }
}
