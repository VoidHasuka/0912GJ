using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PasswordSO", menuName = "MusicCheck/PasswordSO")]
public class PasswordSO : ScriptableObject
{
    [Header("密码序列")]
    public List<PassWord> passWords = new List<PassWord>();
}