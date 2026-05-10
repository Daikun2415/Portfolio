using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public TMP_Text countText;
    public TMP_Text messageText;

    int count;
    private string message;
    public GameObject settingPanel;

    void Start()
    {
        settingPanel.SetActive(false);   
        count = PlayerPrefs.GetInt("JudgeCount", 0);

        countText.text = "";
        messageText.text = "";

        if (count <= 10)
        {
            message = "きみはすごい！ききわけおうだ！";
        }
        else if (count <= 16)
        {
            message = "なかなかじょうずだな！";
        }
        else if (count <= 22)
        {
            message = "もうすこしがんばってきいてみよう！";
        }
        else
        {
            message = "つぎはもっといいきろくをめざそう！";
        }

        StartCoroutine(display());
    }


    IEnumerator display()
    {
        yield return new WaitForSeconds(1f);

        countText.text = "カードをとったかいすう：" + count + "かい";

        yield return new WaitForSeconds(1f);

        messageText.text = message;
    }
    
    public void starttitle()
    {
        SceneManager.LoadScene("Title Scene");
    }
}