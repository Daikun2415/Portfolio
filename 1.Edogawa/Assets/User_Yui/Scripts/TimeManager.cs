using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    
    public float gamelimit = 60f;
    public string resultSceneName = "YuiCreate";

    private float elapsedTime = 0f;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI PlayerscoreText;

    public static float RemainingTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsedTime = 0;
        PlayerscoreText = GameObject.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        RemainingTime = gamelimit - elapsedTime;
        timeText.text = $"のこり{((int)(RemainingTime))}秒";

        

        if (elapsedTime >= gamelimit)
        {
            var results = new List<ResultUIManager.PlayerResult>();

            // ゲーム中に存在する全ての ICharacter を取得
            var characters = GameObject.FindObjectsOfType<MonoBehaviour>(true)
                .OfType<ICharacter>();

            foreach (var c in characters)
            {
                results.Add(new ResultUIManager.PlayerResult
                {
                    playerName = c.Name,
                    CharacterIcon = c.Icon,
                    TotalScore = c.Score
                });
            }

            // 結果を共有クラスに保存
            GameResultData.Results = results;

            // リザルト画面へ遷移
            SceneManager.LoadScene(resultSceneName);
        }
    }
}
