using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声源管理器
/// </summary>
public class SoundSourceManager
{
    //所有声源
    public List<SoundSource> soundSources = new List<SoundSource>();

    //初始化列表
    public void Init()
    {

    }

    //声源发射
    public void EmitAll()
    {
        foreach(var source in soundSources)
        {
            if(source!=null)
            {
                source.Emit();
            }
        }
    }
}
