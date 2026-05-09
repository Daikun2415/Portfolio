using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class titleresultBGMManager : MonoBehaviour
{
    [Header("通常BGM")]
    public AudioClip normalBGM;

    [Range(0f, 1f)]
    public float volume = 0.5f;    // 音量（0～1）

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // AudioSourceの設定
        audioSource.clip = normalBGM;
        audioSource.loop = true;       // ループ再生する場合
        audioSource.volume = volume;
        audioSource.playOnAwake = false; // Awakeで自動再生するのでここはオフでもOK
    }

    void Start()
    {
        if (normalBGM != null)
        { audioSource.Play(); }
        else
        { Debug.LogWarning("BGMクリップが設定されていません！"); }

    }

}
