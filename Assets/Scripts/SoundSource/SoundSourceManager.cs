using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声源管理器
/// </summary>
public class SoundSourceManager
{
    public List<SoundSource> soundSources = new List<SoundSource>();

    public void Init()
    {
        soundSources = new List<SoundSource>();
    }
}
