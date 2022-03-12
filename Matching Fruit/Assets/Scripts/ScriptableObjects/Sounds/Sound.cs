using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    bgm, sfx
}

[CreateAssetMenu(menuName = "Sound")]
public class Sound : ScriptableObject
{
    public string clipName;

    public AudioClip clip;

    public AudioType audioType = AudioType.sfx;

    public bool isLoop = false;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Range(.1f, 3f)]
    public float pitch = 1f;
}
