using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public static string SFX_BUTTON = "SFX_button";

    public static string BGM_BOSS = "BGM_boss";
    public static string BGM_TITLE = "BGM_title";

    public static string SFX_FALLING_OBJECT = "SFX_falling_object";
    public static string SFX_WRONG_MOVE = "SFX_wrong_move";

    public static string SFX_DESTROY = "SFX_destroy";
    public static string SFX_BOMB = "SFX_bomb";
    public static string SFX_LIGHTNING = "SFX_lightning";
    public static string SFX_CLOCK = "SFX_clock";
    public static string SFX_RAINBOW= "SFX_rainbow";
    public static string SFX_HAMMER = "SFX_hammer";

    public static string SFX_BLOCK = "SFX_block";
    public static string SFX_ICE = "SFX_ice";
    public static string SFX_CHAIN = "SFX_chain";

    public static string SFX_ENDGAME = "SFX_endgame";
    public static string SFX_WIN = "SFX_win";
    public static string SFX_LOSE = "SFX_lose";


    [SerializeField] private List<Sound> m_Sounds = new List<Sound>();
    private List<KeyValuePair<Sound, AudioSource>> m_AudioSources = new List<KeyValuePair<Sound, AudioSource>>();

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

    void Start()
    {
    }

    public void ToggleBGMVolume(bool isOn)
    {
        foreach (var source in m_AudioSources)
        {
            if (source.Key.audioType == AudioType.bgm)
            {
                source.Value.mute = !isOn;
            }
        }
    }

    public void ToggleSFXVolume(bool isOn)
    {
        foreach (var source in m_AudioSources)
        {
            if (source.Key.audioType == AudioType.sfx)
            {
                source.Value.mute = !isOn;
            }
        }
    }

    public void PlayBGM(string name)
    {
        foreach (var source in m_AudioSources)
        {
            if (source.Value.isPlaying && source.Key.audioType == AudioType.bgm)
            {
                if (name == source.Key.clipName)
                    break;
                else
                    StartCoroutine(FadeOut(source.Value, 1.5f));
            }    
            if (name == source.Key.clipName && source.Key.audioType == AudioType.bgm)
            {
                StartCoroutine(FadeIn(source.Value, 1.5f));
            }
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        audioSource.volume = 0f;
        audioSource.Play();

        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = startVolume;
    }

    public void StopBGM(float fadeTime)
    {
        foreach (var source in m_AudioSources)
        {
            if (source.Value.isPlaying && source.Key.audioType == AudioType.bgm)
            {
                StartCoroutine(FadeOut(source.Value, fadeTime));
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
