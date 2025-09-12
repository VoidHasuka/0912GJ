using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectManager
{
    public void Init()
    {
        //初始化特效管理器
        
        //测试用
        PlayCheckRightEffect(new Vector2(0, 0), Color.green);
    }

    //播放音符正确特效
    public void PlayCheckRightEffect(Vector2 position, UnityEngine.Color color = default)
    {
        var checkRightEffectGo = GameObject.Instantiate(Resources.Load<GameObject>("Effect/CheckRightEffect"));
        checkRightEffectGo.transform.position = position;

        ParticleSystem ps = checkRightEffectGo.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
            ps.Play();
        }

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        Material mat = Object.Instantiate(renderer.material);
        renderer.material = mat;

        mat.DOFade(0f, 1.0f).From(1f).OnComplete(() =>
        {
            Object.Destroy(ps.gameObject, 0.05f);
        });
    }
}
