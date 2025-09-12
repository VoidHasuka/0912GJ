using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载到声源物体上
/// </summary>
public class SoundSource : MonoBehaviour
{
    //注册到SoundSourceManager中
    private void Start()
    {
        if(GameManager.Instance.soundSourceManager.soundSources!=null)
        {
            GameManager.Instance.soundSourceManager.soundSources.Add(this);
        }
    }
}
