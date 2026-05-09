using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HowtoPlayScript : MonoBehaviour
{
    public List<GameObject> slides; // 遊び方パネル
    public Button NextButton;
    public Button BackButton;
    public Button CloseButton;

    private int currentIndex = 0;

    void Awake()
    {
        //ボタンイベント登録
        NextButton.onClick.AddListener(NextSlide);
        BackButton.onClick.AddListener(BackSlide);
        CloseButton.onClick.AddListener(ClosePanel);
    }

    private void OnEnable()
    {
        if (PauseManager.Instance != null)
            PauseManager.Instance.RequestPause();
    }

    private void OnDisable()
    {
        if (PauseManager.Instance != null)
            PauseManager.Instance.ReleasePause();
    }

    void ShowSlide(int index)
    {
        foreach (var slide in slides)
        {
            slide.SetActive(false);
        }

        //indexのスライドだけ表示
        slides[index].SetActive(true);

        //ボタンの有効/無効の切り替え
        BackButton.gameObject.SetActive(index > 0);
        NextButton.gameObject.SetActive(index < slides.Count - 1);
    }

    public void NextSlide()
    {
        if (currentIndex < slides.Count - 1)
        {
            currentIndex++;
            ShowSlide(currentIndex);
        }
    }

    public void BackSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowSlide(currentIndex);
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false); // 自動的に OnDisable が呼ばれ、Pause解除
    }

    public void ResetAndShowFirstSlide()
    {
        currentIndex = 0;
        ShowSlide(currentIndex);
        gameObject.SetActive(true); // OnEnable で Pause 要求
    }
}
