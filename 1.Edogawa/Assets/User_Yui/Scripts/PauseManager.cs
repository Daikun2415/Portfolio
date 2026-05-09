using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private int pauseRequestCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RequestPause()
    {
        pauseRequestCount++;
        UpdatePauseState();
    }

    public void ReleasePause()
    {
        pauseRequestCount = Mathf.Max(0, pauseRequestCount - 1);
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        if (pauseRequestCount > 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
