using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    // 効果音全体の音量（0.0f ～ 1.0f）
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    private Dictionary<AudioClip, AudioSource> seSources = new Dictionary<AudioClip, AudioSource>();

    [SerializeField] private AudioMixerGroup seGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;

        if (seSources.TryGetValue(clip, out AudioSource source))
        {
            if (source.isPlaying)
                source.Stop();

            source.volume = masterVolume; // ボリューム適用
            source.Play();
        }
        else
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.spatialBlend = 0f;
            newSource.playOnAwake = false;
            newSource.clip = clip;
            newSource.volume = masterVolume; // ボリューム適用
            newSource.Play();

            if (seGroup != null)
                newSource.outputAudioMixerGroup = seGroup;

            seSources[clip] = newSource;
        }
    }

    /// <summary>
    /// 全体の効果音ボリュームを設定
    /// </summary>
    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);

        // 既に存在する全AudioSourceにも反映
        foreach (var kvp in seSources)
        {
            if (kvp.Value != null)
            {
                kvp.Value.volume = masterVolume;
            }
        }
    }
}
