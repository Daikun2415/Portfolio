using UnityEngine;

public class SettingsPanelScript : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("SettingsPanel Enabled - RequestPause");
        if (PauseManager.Instance != null)
            PauseManager.Instance.RequestPause();
    }

    private void OnDisable()
    {
        Debug.Log("SettingsPanel Disabled - ReleasePause");
        if (PauseManager.Instance != null)
            PauseManager.Instance.ReleasePause();
    }
}
