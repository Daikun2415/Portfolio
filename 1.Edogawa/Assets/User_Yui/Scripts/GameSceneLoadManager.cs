using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoadManager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;//60fps
    }
    public string gameSceneName = "ShoScene";
    // Update is called once per frame
    void Update()
    {
        // Enterキーが押されたらシーンをロード
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
