using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float period = 2.0f;             // 每隔多少秒触发一次
    public string shockProperty = "_ShockTime";
    // Start is called before the first frame update
    void Start()
    {
        
    }
    SpriteRenderer sr;
    MaterialPropertyBlock mpb;
    float nextTime;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        // 立刻触发一次
        TriggerShock();
        nextTime = Time.time + period;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTime)
        {
            TriggerShock();
            nextTime = Time.time + period;
        }

        // 手动测试：按空格立即触发
        if (Input.GetKeyDown(KeyCode.Space)) TriggerShock();

    }
    public void TriggerShock()
    {
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(shockProperty, Time.time);   // 写入 _ShockTime
        sr.SetPropertyBlock(mpb);
    }
}
