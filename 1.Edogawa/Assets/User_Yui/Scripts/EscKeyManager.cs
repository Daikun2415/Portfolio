using UnityEngine;

public class EscKeyManager : MonoBehaviour
{
    public HowtoPlayScript howToPlayScript;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!howToPlayScript.gameObject.activeSelf) // ‚Ü‚¾ŠJ‚¢‚Ä‚È‚¢‚Æ‚«‚¾‚¯
            {
                howToPlayScript.ResetAndShowFirstSlide();
            }
            else
            {
                howToPlayScript.ClosePanel();
            }
        }
    }

    public void Close()
    {
        Time.timeScale = 1.0f;
    }
}
