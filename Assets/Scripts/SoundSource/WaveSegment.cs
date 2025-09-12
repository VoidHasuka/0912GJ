using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class WaveSegment : MonoBehaviour
{
    // ������ɢ�ٶȣ�ÿ��Ŵ�ı�����
    private float expansionSpeed = 5f;
    // �����ʱ�䣨�룩���������Զ�����
    private float lifeTime = 5f;
    // ����Ƿ�Ϊ����������Ĳ���
    [HideInInspector] public bool isReflection = false;
    // �ڲ���ʱ
    private float age = 0f;
    // ��ʾ�Ƿ��Ѿ�������һ�η���
    private bool hasReflected = false;
    // ��ʼPolygonCollider2D��״�Ķ��㼯�ϣ���λԲ�Σ�����ν��ƣ�
    private List<Vector2> baseCirclePoints;
    // PolygonCollider2D���
    private PolygonCollider2D polyCol;

    void Awake()
    {
        polyCol = GetComponent<PolygonCollider2D>();
        // ��ʼ��PolygonCollider2DΪԲ������������Բ�εĶ���Σ�
        InitializeCirclePolygon();
        // ���ø������ԣ�����ʹ��Kinematic���壬����ֻ�����¼�����������Ӱ��
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.simulated = true;
            rb.useFullKinematicContacts = true;
            rb.gravityScale = 0;
        }
        // ����ײ������Ϊ���������Ա㴥��Enter�¼��������������赲
        polyCol.isTrigger = true;
    }

    private void Start()
    {
        //��ʼ�������ٶ�
        expansionSpeed = GameManager.Instance.expendSpeed;
        //��ʼ����������
        lifeTime = GameManager.Instance.LifeTime;
    }

    void Update()
    {
        // ��ʱ�������ƣ��޸�����ʵ����ɢ��
        float scaleDelta = expansionSpeed * Time.deltaTime;
        transform.localScale += new Vector3(scaleDelta, scaleDelta, 0f);

        age += Time.deltaTime;
        if (age >= lifeTime)
        {
            Destroy(gameObject); // �����������ٲ��ζ���
        }
    }

    // ��ʼ���������ײ��Ϊ��λԲ������״
    void InitializeCirclePolygon(int segments = 60)
    {
        baseCirclePoints = new List<Vector2>();
        float angleStep = 2 * Mathf.PI / segments;
        float radius = 0.5f; // Ĭ��Circle����ֱ��Ϊ1��λ����˰뾶0.5
        for (int i = 0; i < segments; i++)
        {
            float theta = i * angleStep;
            baseCirclePoints.Add(new Vector2(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius));
        }
        polyCol.SetPath(0, baseCirclePoints.ToArray());
    }

    // �����ν��봥������ʱ���ã��ϰ�����������
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ����Ƿ������ϰ��ͨ��Tag���������ʶ��
        if (!hasReflected && other.CompareTag("Obstacle"))
        {
            Debug.Log($"���� {name} �����ϰ��� {other.name}");
            // ���㱻�ڵ����ֵĽǶȷ�Χ�����������񲨶�
            BoxCollider2D obstacle = other as BoxCollider2D;
            if (obstacle != null)
            {
                ReflectWave(obstacle);
            }
        }
        // ����������ײ�߼���Receiver�ű�������Receiver�ű������ "Wave" ��ǩ��
    }

    // ���ξ������߼������㱻�ϰ��ڵ��ĽǶȶΣ������µķ��䲨��
    void ReflectWave(BoxCollider2D obstacle)
    {
        hasReflected = true; // ����ѷ��䣬��ֹ�ظ�����

        // ��ȡ�ϰ������ĺ����᷽�򣨳��߷���λ������
        Transform obsTransform = obstacle.transform;
        Vector2 obsCenter = obsTransform.position;
        // ͨ���Ƚ�BoxCollider2D�ߴ�ȷ�����߷��򣨼ٶ�BoxCollider2D�����Ż����������ţ�
        Vector2 mainAxis;
        if (obstacle.size.x >= obstacle.size.y)
            mainAxis = obsTransform.right.normalized;   // ˮƽ����Ϊ����
        else
            mainAxis = obsTransform.up.normalized;      // ��ֱ����Ϊ����

        // 1. ���㲨Դ����ǰ�������ģ�������ϰ�����ڵ��Ƕȷ�Χ
        Vector2 sourcePos = transform.position;
        // ��ȡ�ϰ����ĸ��Ǻ����������ĵ����������
        Vector2 halfSize = obstacle.size * 0.5f;
        // �����ϰ�����ת�����ֲ������ת��������
        Vector2 corner1 = obsCenter + mainAxis * halfSize.x + (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 corner2 = obsCenter + mainAxis * halfSize.x - (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 corner3 = obsCenter - mainAxis * halfSize.x + (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 corner4 = obsCenter - mainAxis * halfSize.x - (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        // �����ĵ㣨�ϰ������������е㣩
        Vector2 upMid = obsCenter + (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 downMid = obsCenter - (Vector2)(Quaternion.Euler(0, 0, 90) * mainAxis) * halfSize.y;
        Vector2 rightMid = obsCenter + mainAxis * halfSize.x;
        Vector2 leftMid = obsCenter - mainAxis * halfSize.x;
        // �ռ���Щ�ؼ�������ڲ�Դ�ķ����
        List<float> angles = new List<float>();
        Vector2[] points = new Vector2[] { corner1, corner2, corner3, corner4, upMid, downMid, leftMid, rightMid };
        foreach (Vector2 pt in points)
        {
            Vector2 dir = pt - (Vector2)sourcePos;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // ת����0~360�ȷ�Χ
            if (ang < 0) ang += 360f;
            angles.Add(ang);
        }
        // Ҳ����Ӳ�Դָ���ϰ������ĵĽǶȣ������᷽��
        Vector2 centerDir = obsCenter - sourcePos;
        float centerAng = Mathf.Atan2(centerDir.y, centerDir.x) * Mathf.Rad2Deg;
        if (centerAng < 0) centerAng += 360f;
        angles.Sort();
        // �ҵ�Χ�� centerAng ���������ڷ���߽磨�ϰ����ڵ������ұ�Ե�ǣ�
        int insertIndex = angles.FindIndex(a => a > centerAng);
        // ���centerAng�����б������нǶȣ�������Ϊ0
        if (insertIndex < 0) insertIndex = angles.Count;
        // ���ڱ߽磺prevAng��nextAng��ע�⴦���б�ѭ����
        float prevAng = angles[(insertIndex - 1 + angles.Count) % angles.Count];
        float nextAng = angles[insertIndex % angles.Count];
        // ȷ�� prevAng ���ڵ�������ʼ�ǣ�nextAng �ǽ����ǣ�ʹ��centerAng����������ڲ���
        // ����ֱ�Ӵ�prevAng��nextAng�����ĽǶ�
        float directDiff = (nextAng >= prevAng) ? (nextAng - prevAng) : (360f - prevAng + nextAng);
        // ���centerAng�Ƿ��ڴ�direct������
        bool centerInDirect = false;
        if (prevAng < nextAng)
            centerInDirect = (centerAng >= prevAng && centerAng <= nextAng);
        else
            centerInDirect = (centerAng >= prevAng || centerAng <= nextAng);
        // ��centerAng����directDiff��Χ�ڣ���ת�߽磨����prevAng��nextAng��
        if (!centerInDirect)
        {
            float temp = prevAng;
            prevAng = nextAng;
            nextAng = temp;
            // ���¼���directDiffΪ��һ����
            directDiff = (nextAng >= prevAng) ? (nextAng - prevAng) : (360f - prevAng + nextAng);
        }
        // ��ʱprevAng��nextAng�����˱��ڵ��ĽǶȷ�Χ������centerAng��
        float blockedStartAng = prevAng;
        float blockedEndAng = nextAng;
        float blockedArcAngle = directDiff;
        // 2. ���ϰ��������ȡ����Գ�λ�úͽǶȷ�Χ
        // ���㲨Դ������ϰ���ľ���㣨����Դλ�ã�
        Vector2 mirrorSourcePos = WaveReflector.ReflectPointAcrossLine(sourcePos, obsCenter, mainAxis);
        // ���㱻�ڵ��Ƕȷ�Χ�ľ���Ƕȣ�������������꣩
        // ȡ�ڵ���Χ�ı߽緽������������
        Vector2 vStart = new Vector2(Mathf.Cos(blockedStartAng * Mathf.Deg2Rad), Mathf.Sin(blockedStartAng * Mathf.Deg2Rad));
        Vector2 vEnd = new Vector2(Mathf.Cos(blockedEndAng * Mathf.Deg2Rad), Mathf.Sin(blockedEndAng * Mathf.Deg2Rad));
        // �����������ֱ����ϰ�����Գ�
        Vector2 vStartRef = WaveReflector.ReflectVectorAcrossLine(vStart, mainAxis);
        Vector2 vEndRef = WaveReflector.ReflectVectorAcrossLine(vEnd, mainAxis);
        float reflectStartAng = Mathf.Atan2(vStartRef.y, vStartRef.x) * Mathf.Rad2Deg;
        float reflectEndAng = Mathf.Atan2(vEndRef.y, vEndRef.x) * Mathf.Rad2Deg;
        if (reflectStartAng < 0) reflectStartAng += 360f;
        if (reflectEndAng < 0) reflectEndAng += 360f;
        // ȷ������Ƕȷ�Χ��ԭ�ڵ����νǶȴ�Сһ��
        float diffRef = 0f;
        if (reflectStartAng <= reflectEndAng)
            diffRef = reflectEndAng - reflectStartAng;
        else
            diffRef = 360f - reflectStartAng + reflectEndAng;
        // �������ˣ��õ���diffRef����ԭblockedArcAngle�����򽻻���ȡ��һ�໡
        if (Mathf.Abs(diffRef - blockedArcAngle) > 1e-2)
        {
            // ����
            float tempAng = reflectStartAng;
            reflectStartAng = reflectEndAng;
            reflectEndAng = tempAng;
            if (reflectStartAng < 0) reflectStartAng += 360f;
            if (reflectEndAng < 0) reflectEndAng += 360f;
            // ���¼����ֵ
            if (reflectStartAng <= reflectEndAng)
                diffRef = reflectEndAng - reflectStartAng;
            else
                diffRef = 360f - reflectStartAng + reflectEndAng;
        }
        // 3. ���µ�ǰ���ε���ײ�壬ʹ�䲻�ٸ��Ǳ��ڵ��������������ɡ�ȱ�ڡ���״��
        CreateSectorShape(polyCol, blockedStartAng, blockedEndAng, removeSector: true);
        // 4. �����µľ����䲨�ζ���
        GameObject newWaveObj = Instantiate(gameObject, mirrorSourcePos, Quaternion.identity);
        // ���þ��񲨶�����
        WaveSegment newWave = newWaveObj.GetComponent<WaveSegment>();
        if (newWave != null)
        {
            newWave.isReflection = true;
            newWave.hasReflected = true; // ���񲨶β��ٽ��ж��η���
            // �����񲨶εĳ�ʼ��ɢ�뾶����Ϊ��ǰ���ΰ뾶���ӶԳ�λ�ü�����ɢ
            newWaveObj.transform.localScale = transform.localScale;
            // ���¾��񲨶ε���ײ��״��������ԭ�ڵ����ζ�Ӧ������
            newWave.CreateSectorShape(newWave.polyCol, reflectStartAng, reflectEndAng, removeSector: false);
        }
    }

    // ������ֹ�Ƕȴ�������Բ�Σ����Σ���ײ��״
    // removeSector=true ��ʾ�Ƴ��ýǶȷ�Χ�����Σ��������ಿ�֣���false��ʾ�����������Σ��Ƴ��������֣�
    public void CreateSectorShape(PolygonCollider2D collider, float angleStart, float angleEnd, bool removeSector)
    {
        // ȷ���Ƕ���0-360��Χ��
        if (angleStart < 0) angleStart += 360f;
        if (angleEnd < 0) angleEnd += 360f;
        angleStart %= 360f;
        angleEnd %= 360f;
        // ��������Բ�ζ��㣬���ڼ���
        if (baseCirclePoints == null || baseCirclePoints.Count == 0)
        {
            Debug.LogWarning("WaveSegment: ����Բ�ζ���δ��ʼ����");
            return;
        }
        List<Vector2> newPoints = new List<Vector2>();
        // ����һ���������ڻ�ȡ�Ƕȶ�Ӧ����
        System.Func<float, Vector2> pointOnCircle = (deg) =>
        {
            float rad = deg * Mathf.Deg2Rad;
            // ע�⣺baseCirclePointsĬ�ϰ뾶0.5����Ӧtransform.scale=1
            // ����õ�λԲ0.5�뾶���ɣ�������transform����
            return new Vector2(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f);
        };
        // �淶���ǶȲ����
        float diff = 0f;
        bool wraps = false;
        if (angleStart <= angleEnd)
            diff = angleEnd - angleStart;
        else
        {
            diff = 360f - angleStart + angleEnd;
            wraps = true;
        }
        // �����Ҫ�Ƴ�ָ�����Σ��������ಿ�֣���֮����ָ������
        bool invert = removeSector;
        // �������ǰ���ʱ��˳�����ɶ���ζ���
        if (!invert)
        {
            // ���� angleStart->angleEnd ֮�������
            // ������ĵ�
            newPoints.Add(Vector2.zero);
            // �����ʼ���ص�
            newPoints.Add(pointOnCircle(angleStart));
            // ��Բ��������ֱ��������
            float stepAngle = 360f / baseCirclePoints.Count;
            float current = angleStart;
            while (true)
            {
                // ǰ������
                current += stepAngle;
                if (wraps)
                {
                    if (current >= 360f)
                        current -= 360f;
                    // ������Խ��angleEndʱ����
                    if (current >= angleEnd && current < angleStart)
                        break;
                }
                else
                {
                    if (current > angleEnd) break;
                }
                newPoints.Add(pointOnCircle(current));
            }
            // ��ӽ������ص�
            newPoints.Add(pointOnCircle(angleEnd));
            // PolygonCollider2D �Զ��պ�·����������ص����ĵ�պ��γ�����
        }
        else
        {
            // �Ƴ� angleStart->angleEnd ���Σ��������ಿ��
            // ��״����һ����ȱ�ڡ�Բ���� Pac-Man ��״��
            // ���Բ���Ͻ����߽������
            newPoints.Add(pointOnCircle(angleEnd));
            // ��Բ����ӵ㣨�ƹ����Ƴ����֣�
            float stepAngle = 360f / baseCirclePoints.Count;
            float current = angleEnd;
            while (true)
            {
                current += stepAngle;
                if (current >= 360f) current -= 360f;
                // ���������뱻�Ƴ�����ʱ��������һ��
                // ���angleStart<angleEnd������0�㣩���� current ���� angleStart ����ѭ��
                if (!wraps)
                {
                    if (current > angleStart && current < angleEnd)
                        break;
                }
                else
                {
                    // ������ο�Խ0�ȣ����Ƴ������� angleStart->360 �� 0->angleEnd
                    // open����Ϊ angleEnd->angleStart
                    if (current > angleStart && current < angleEnd)
                    {
                        // ��̫���ܽ������������Ϊwraps���open���侭��0
                    }
                    // ��current�ص���㸽��������������
                    if ((angleEnd < angleStart && current > angleStart) ||
                        (angleEnd < angleStart && current < angleEnd))
                    {
                        break;
                    }
                }
                newPoints.Add(pointOnCircle(current));
            }
            // ���Բ������ʼ�߽������
            newPoints.Add(pointOnCircle(angleStart));
            // ���������ĵ�պ�ȱ��
            newPoints.Add(Vector2.zero);
        }
        // ����PolygonCollider����״����
        collider.SetPath(0, newPoints.ToArray());
    }
}
