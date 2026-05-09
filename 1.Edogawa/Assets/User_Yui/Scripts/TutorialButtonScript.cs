using UnityEngine;

public class TutorialButtonScript : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public void ToggleHowToPlay()
    {
        //ï\é¶/îÒï\é¶êÿÇËë÷Ç¶
        /*
        bool isActive = howToPlayPanel.activeSelf;
        howToPlayPanel.SetActive(!isActive);
        */

        Time.timeScale = 0;
    }
}
