using UnityEngine;


public class SoundWave : MonoBehaviour
{

    [Header("声源所属关卡索引")]
    public int itsLevelIndex = 0; // 该声源所属的关卡索引
    public WaveType itsWaveType = WaveType.A; // 该声源的波类型


    [Header("Prefabs & Optional Wiring")]
    [Tooltip("主波的Prefab，里边应带有 SpriteRenderer + CircleCollider2D(isTrigger) + Rigidbody2D(Kinematic) + WavePropagation2D")]
    public GameObject wavePrefab;

    [Header("Wave Defaults (applied to WavePropagation2D)")]
    public float startScale = 0f;
    public float expansionSpeed = 6f;
    public float maxScale = 50f;     // <=0 不限制
    public float lifeTime = 8f;      // <=0 不限制

    [Header("Collision Filter (applied to WavePropagation2D)")]
    public string obstacleTag = "Obstacle";
    public LayerMask obstacleLayer = ~0;

    [Header("Spawn Offset (optional)")]
    public Vector3 localSpawnOffset = Vector3.zero; // 如需略微前移可设这里

    private void Start()
    {
        GameManager.Instance.soundSourceManager.soundSources.Add(this);
    }

    /// <summary>
    /// 在“声源”当前位置（含可选偏移）生成一枚主波
    /// </summary>
    public void EmitWave()
    {
        if (wavePrefab == null)
        {
            Debug.LogError("[WaveSourceEmitter] 未指定 wavePrefab");
            return;
        }

        Vector3 spawnPos = transform.position + localSpawnOffset;
        Quaternion rot = Quaternion.identity;

        // 生成主波实例
        GameObject wave = Instantiate(wavePrefab, spawnPos, rot); // 运行时实例化 Prefab 的标准用法
        // 给 WavePropagation2D 赋默认参数（如果该组件存在）
        var prop = wave.GetComponent<WavePropagation2D>();
        if (prop != null)
        {
            prop.startScale = startScale;
            prop.expansionSpeed = expansionSpeed;
            prop.maxScale = maxScale;
            prop.lifeTime = lifeTime;

            prop.obstacleTag = obstacleTag;
            prop.obstacleLayer = obstacleLayer;

            prop.itsWaveType = itsWaveType;
        }
    }
}
