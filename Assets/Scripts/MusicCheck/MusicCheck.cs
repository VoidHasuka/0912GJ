using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCheck : MonoBehaviour
{
    private int currentIndex = 0;
    public int maxIndex = 8;
    
    public float faultToleranceTime = 0.2f; // 容错时间
    private float faultToleranceTimer = 0f;
}
