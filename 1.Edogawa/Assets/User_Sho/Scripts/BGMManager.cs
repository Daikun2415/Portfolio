using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    [Header("通常BGM設定")]
    public AudioClip normalBGM;
    [Header("終盤BGM設定")]
    public AudioClip urgentBGM;
    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("クロスフェード速度")]
    public float fadeSpeed = 1f; // 1秒でフェード完了


    [SerializeField] private AudioSource source1;
    [SerializeField] private AudioSource source2;

    [SerializeField] private AudioMixerGroup bgmGroup;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private bool isUrgent = false;
    private bool isFading = false;

    // 判定開始遅延（プレイ開始直後のいきなり切り替え防止）
    private bool checkEnabled = false;
    private float startDelay = 0.5f;

    void Awake()
    {

        InitAudioSource(source1);
        InitAudioSource(source2);

        currentSource = source1;
        nextSource = source2;

    }

    void InitAudioSource(AudioSource source)
    {
        source.loop = true;
        source.volume = 0;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = bgmGroup;
    }

    void Start()
    {
        if (normalBGM != null)
        {
            currentSource.clip = normalBGM;
            currentSource.volume = volume;
            currentSource.Play();
        }
        else
        {
            Debug.LogWarning("通常BGMが設定されていません！");
        }

    }

    void Update()
    {
        if (!checkEnabled)
        {
            startDelay -= Time.deltaTime;
            if (startDelay <= 0f) checkEnabled = true;
            else return;
        }

        // TimerManagerから残り時間を取得
        float remainTime = TimerManager.RemainingTime;

        // 残り10秒で緊急BGMに切り替え
        if (!isUrgent && remainTime <= 10f)
        {
            StartCrossFade(urgentBGM);
            isUrgent = true;
        }

        // フェード処理
        if (isFading)
        {
            currentSource.volume = Mathf.Max(0f, currentSource.volume - fadeSpeed * Time.deltaTime);
            nextSource.volume = Mathf.Min(volume, nextSource.volume + fadeSpeed * Time.deltaTime);

            if (nextSource.volume >= volume)
            {
                currentSource.volume = 0;
                currentSource.Stop();
                nextSource.volume = volume;

                // AudioSourceを入れ替える
                var temp = currentSource;
                currentSource = nextSource;
                nextSource = temp;

                isFading = false;
            }
        }
    }

    void StartCrossFade(AudioClip newClip)
    {
        if (newClip == null) return;

        nextSource.clip = newClip;
        nextSource.volume = 0;
        nextSource.Play();

        isFading = true;
    }
}
