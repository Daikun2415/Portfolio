using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class volumecontrol : MonoBehaviour
{
    [SerializeField] private AudioMixer audiomixer;
    [SerializeField] private Slider masterslider;
    [SerializeField] private Slider bgmslider;
    [SerializeField] private Slider seslider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (audiomixer.GetFloat("Master-Volume", out float masterDb))
        {
            masterslider.value = DbToLinear(masterDb);
        }
        if (audiomixer.GetFloat("BGM-Volume", out float bgmDb))
        {
            bgmslider.value = DbToLinear(bgmDb);
        }
        if (audiomixer.GetFloat("SE-Volume", out float seDb))
        {
            seslider.value = DbToLinear(seDb);
        }

        if (audiomixer != null)
        {
            masterslider.onValueChanged.AddListener((value) =>
            {
                value = Mathf.Clamp01(value);

                float decibel = 20f * Mathf.Log10(value);
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                audiomixer.SetFloat("Master-Volume", decibel);
            });

            bgmslider.onValueChanged.AddListener((value) =>
            {
                value = Mathf.Clamp01(value);

                float decibel = 20f*Mathf.Log10(value);
                decibel = Mathf.Clamp(decibel,-80f,0f);
                audiomixer.SetFloat("BGM-Volume", decibel);
            });

            seslider.onValueChanged.AddListener((value) =>
            {
                value = Mathf.Clamp01(value);

                float decibel = 20f * Mathf.Log10(value);
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                audiomixer.SetFloat("SE-Volume", decibel);
            });
        }
    }

    private float DbToLinear(float db)
    {
        if (db <= -80f) return 0f;
        return Mathf.Pow(10f, db / 20f);
    }

}
