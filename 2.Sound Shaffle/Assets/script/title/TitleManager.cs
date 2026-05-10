using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject titlePanel;
    public GameObject stageSelectPanel;

    [Header("SE設定")]
    public AudioSource audioSource;
    public AudioClip buttonSE;

    void Start()
    {
        titlePanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }

    void PlaySE()
    {
        if (audioSource != null && buttonSE != null)
        {
            audioSource.PlayOneShot(buttonSE);
        }
    }

    // スタート押した時
    public void OpenStageSelect()
    {
        PlaySE();
        titlePanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }

    // 戻るボタン
    public void BackTitle()
    {
        PlaySE();
        titlePanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }

    // ステージ開始
    public void StartStage1()
    {
        PlaySE();
        SceneManager.LoadScene("Main Scene1");
    }

    public void StartStage2()
    {
        PlaySE();
        SceneManager.LoadScene("Main Scene2");
    }

    public void StartStage3()
    {
        PlaySE();
        SceneManager.LoadScene("Main Scene3");
    }
}