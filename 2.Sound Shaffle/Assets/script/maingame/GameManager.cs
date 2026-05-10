using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SoundCard firstCard;
    private SoundCard secondCard;

    public bool isChecking = false;
    public bool isUIOpen = false;

    public TMP_Text resultText;

    private int judgeCount = 0;

    public void OnCardClicked(SoundCard card)
    {
        if (isUIOpen) return;

        if (isChecking) return;
        if (card == firstCard) return;

        if (firstCard == null)
        {
            firstCard = card;
            firstCard.Open();
        }
        else
        {
            secondCard = card;
            secondCard.Open();
            judgeCount++;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        isChecking = true;

        yield return new WaitForSeconds(1f);

        if (firstCard.categoryID == secondCard.categoryID)
        {
            ShowResult("Ç¢Ç‹ÇÃÇ®Ç∆ÇÕ " + firstCard.categoryName + " Ç≈ÇµÇΩÅI");

            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f); // éöñãï\é¶éûä‘

            resultText.text = "";

            if (GameObject.FindObjectsOfType<SoundCard>().Length == 0)
            {
                PlayerPrefs.SetInt("JudgeCount", judgeCount);
                SceneManager.LoadScene("End Scene");
            }
        }
        else
        {
            firstCard.Close();
            secondCard.Close();
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title Scene");
    }

    void ShowResult(string msg)
    {
        resultText.text = msg;
    }
}