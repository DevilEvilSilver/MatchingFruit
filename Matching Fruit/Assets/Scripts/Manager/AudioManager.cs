using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public static string BGM_TITLE = "BGM_title";
    public static string SFX_DESTROY = "SFX_destroy";
    public static string SFX_BOMB = "SFX_bomb";
    public static string SFX_LIGHTNING = "SFX_lightning";
    public static string SFX_CLOCK = "SFX_clock";

    public static string SFX_HAMMER = "SFX_hammer";

    [SerializeField] private List<Sound> m_Sounds = new List<Sound>();
    private List<KeyValuePair<Sound, AudioSource>> m_AudioSources = new List<KeyValuePair<Sound, AudioSource>>();
    private bool m_IsBGM = true;
    private bool m_IsSFX = true;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        foreach (Sound sound in m_Sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.loop = sound.isLoop;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            m_AudioSources.Add(new KeyValuePair<Sound, AudioSource>(sound, source));
        }
    }

    public void ToggleBGMVolume()
    {
        if (m_IsBGM)
        {
            m_IsBGM = false;
            foreach (var source in m_AudioSources)
            {
                if (source.Key.audioType == AudioType.bgm)
                {
                    source.Value.volume = 0f;
                }
            }
        }
        else
        {
            m_IsBGM = true;
            foreach (var source in m_AudioSources)
            {
                if (source.Key.audioType == AudioType.bgm)
                {
                    source.Value.volume = source.Key.volume;
                }
            }
        }
    }

    public void ToggleSFXVolume()
    {
        if (m_IsSFX)
        {
            m_IsSFX = false;
            foreach (var source in m_AudioSources)
            {
                if (source.Key.audioType == AudioType.sfx)
                {
                    source.Value.volume = 0f;
                }
            }
        }
        else
        {
            m_IsSFX = true;
            foreach (var source in m_AudioSources)
            {
                if (source.Key.audioType == AudioType.sfx)
                {
                    source.Value.volume = source.Key.volume;
                }
            }
        }
    }

    public void PlayBGM(string name)
    {
        foreach (var source in m_AudioSources)
        {
            if (name == source.Key.clipName && source.Key.audioType == AudioType.bgm)
            {
                source.Value.Play();
                break;
            }
        }
    }

    public void StopBGM()
    {
        foreach (var source in m_AudioSources)
        {
            if (source.Key.audioType == AudioType.bgm)
            {
                source.Value.Stop();
                break;
            }
        }
    }

    public void PlaySFX(string name)
    {
        foreach (var source in m_AudioSources)
        {
            if (name == source.Key.clipName && source.Key.audioType == AudioType.sfx)
            {
                source.Value.Play();
                break;
            }
        }
    }
}
