using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WavePropagation2D : MonoBehaviour
{

    public WaveType itsWaveType = WaveType.A; // 波类型

    [Header("Propagation")]
    public float startScale = 0f;       // 初始统一缩放
    public float expansionSpeed = 6f;   // 匀速扩张（每秒缩放增量）
    public float maxScale = 50f;        // >= 上限即销毁；<=0 表示不限
    public float lifeTime = 8f;         // >= 到时销毁；<=0 表示不限

    [Header("Collision Filter")]
    public string obstacleTag = "Obstacle";
    public LayerMask obstacleLayer = ~0;    // 需要检测的 Layer

    [Header("Dispatch Targets (独立脚本实例)")]
    public TriangleMaskPlacer triangleMaskPlacer; // 主波命中障碍 → 放三角遮罩
    public EchoSpawner echoSpawner;               // 回声命中障碍 → 生成回声
    public Transform source;                      // 声源（给两个脚本使用）

    [Header("当前波产生的遮罩")]
    private List<SpriteMask> _createdMasks;

    // 仅触发一次（每个障碍）
    private readonly HashSet<Collider2D> _hitOnce = new();
    private float _age;

    void Start()
    {
        // 匀速扩张从 startScale 开始
        transform.localScale = Vector3.one * startScale;

        // 触发器基本要求：有 CircleCollider2D 且 isTrigger，且至少一方有 Rigidbody2D
        //（本物体上挂 Kinematic Rigidbody2D 最稳）:contentReference[oaicite:1]{index=1}
        var col = GetComponent<PolygonCollider2D>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.gravityScale = 0f;

        _createdMasks = new List<SpriteMask>();
    }

    void Update()
    {
        // 匀速扩张（统一缩放 x=y）
        float ds = expansionSpeed * Time.deltaTime;
        transform.localScale += new Vector3(ds, ds, 0f);

        // 生命周期/尺寸上限
        _age += Time.deltaTime;
        if ((lifeTime > 0f && _age >= lifeTime) ||
            (maxScale > 0f && transform.localScale.x >= maxScale))
        {

            DestroySelfAndMyMasks();
        }
    }

    private void DestroySelfAndMyMasks()
    {
        // 先清理自己产生的遮罩
        for (int i = 0; i < _createdMasks.Count; i++)
        {
            var sm = _createdMasks[i];
            if (sm != null) Destroy(sm.gameObject);
        }
        _createdMasks.Clear();

        // 再销毁 wave 自己
        Destroy(gameObject);

        // 不要再调用全局清空了（会误删其他 wave 的）
        // GameManager.Instance.uiManager.ClearTriangleMasks();  // ← 删除这句
    }

    // 防止外部直接 Destroy(this.gameObject) 漏清遮罩
    void OnDestroy()
    {
        for (int i = 0; i < _createdMasks.Count; i++)
        {
            var sm = _createdMasks[i];
            if (sm != null) Destroy(sm.gameObject);
        }
        _createdMasks.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Layer 与 Tag 过滤
        if (((1 << other.gameObject.layer) & obstacleLayer) == 0) return;
        if (!string.IsNullOrEmpty(obstacleTag) && !other.CompareTag(obstacleTag)) return;

        if (_hitOnce.Contains(other)) return; // 对同一障碍只处理一次
        _hitOnce.Add(other);

        // 仅处理 BoxCollider2D 障碍
        var box = other as BoxCollider2D ?? other.GetComponent<BoxCollider2D>();
        if (box == null) return;

       

        // 自身是“主波”还是“回声”，由 Tag 决定
        if (other.CompareTag("Obstacle"))
        {
            if (echoSpawner != null)
            {
                Debug.Log(111);
                if (echoSpawner.source == null && source != null)
                    echoSpawner.source = source;
                echoSpawner.SpawnEchoWithObstacle(box);
            }
            if (triangleMaskPlacer != null)
            {
                if (triangleMaskPlacer.source == null && source != null)
                    triangleMaskPlacer.source = source;

                // 传入当前物体的 transform → 同步对齐到“对边”的位置与朝向
                var mask = triangleMaskPlacer.PlaceMaskWithObstacle(box);
                if(mask)_createdMasks.Add(mask);
            }
        }
    }
}
