using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundGroup
{
    public string name;              // グループネーム
    public List<AudioClip> clips;    // 同じグループの音を登録
}