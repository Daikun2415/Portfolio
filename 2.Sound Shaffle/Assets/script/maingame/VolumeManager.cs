using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider bgmSlider;
    public Slider seSlider;

    public GameObject SettingPanal;
    public GameManager gameManager;

    [Header("SE設定")]
    public AudioSource audioSource;
    public AudioClip buttonSE;

    void Start()
    {
        // 保存値読み込み
        float bgm = PlayerPrefs.GetFloat("BGM", 0.75f);
        float se = PlayerPrefs.GetFloat("SE", 0.75f);

        bgmSlider.value = bgm;
        seSlider.value = se;

        SetBGM(bgm);
        SetSE(se);

        SettingPanal.SetActive(false);
    }
    void PlaySE()
    {
        if (audioSource != null && buttonSE != null)
        {
            audioSource.PlayOneShot(buttonSE);
        }
    }

    public void SetBGM(float value)
    {
        mixer.SetFloat("BGMVolume", value);
        PlayerPrefs.SetFloat("BGM", value);
    }

    public void SetSE(float value)
    {
        mixer.SetFloat("SEVolume", value);
        PlayerPrefs.SetFloat("SE", value);
    }

    public void OpenVolumePanel()
    {
        PlaySE();
        SettingPanal.SetActive(true);
        gameManager.isUIOpen = true;
        Time.timeScale = 0;
    }

    public void CloseVolumePanal()
    {
        PlaySE();
        SettingPanal.SetActive(false);
        gameManager.isUIOpen = false;
        Time.timeScale = 1;
    }
}