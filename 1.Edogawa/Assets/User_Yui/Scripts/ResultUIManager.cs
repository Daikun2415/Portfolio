using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultUIManager : MonoBehaviour
{
    public GameObject playerPanelPrefab;
    public Transform playerPanelParent;

    //プレイヤーとNPCのアイコンの設定↓
    public Sprite icon1;
    public Sprite icon2;
    public Sprite icon3;
    public Sprite icon4;

    public string ResultSceneName = "YuiCreate";
    public string TitleSceneName = "TitleScene";
    public string gameSceneName = "ShoScene";

    public Button RetryButton;                // Retryボタン
    public Button ExitButton;                 // Exitボタン

    [System.Serializable]
    public class PlayerResult
    {
        public string playerName;  //プレイヤー名
        public Sprite CharacterIcon;//アイコン
        public int rank;            //ランク

        //合計
        public int TotalScore {  get; set; }
    }
    // シーン遷移前にスコア結果を渡す用
    private List<PlayerResult> currentResults;
    //外部からスコアデータを受け取る
    public void Initialize(List<PlayerResult> results)
    {
        currentResults = results;
    }

    void Start()
    {
        //テストデータ②↓
        //var dummyResults = new List<PlayerResult>
        //{
        //    new PlayerResult {
        //        playerName = "Player", CharacterIcon = icon1,
        //        TotalScore = 10  // Player のスコア
        //    },
        //    new PlayerResult {
        //        playerName = "NPC1", CharacterIcon = icon2,
        //        TotalScore = 8   // NPC1 のスコア
        //    },
        //    new PlayerResult {
        //        playerName = "NPC2", CharacterIcon = icon3,
        //        TotalScore = 12  // NPC2 のスコア
        //    },
        //    new PlayerResult {
        //        playerName = "NPC3", CharacterIcon = icon4,
        //        TotalScore = 6   // NPC3 のスコア
        //    },
        //};

        //↓テストデータ実行
        //ShowResults(dummyResults);

        //↓本番用
        if (GameResultData.Results != null)
        {
            ShowResults(GameResultData.Results);
        }
        /*
        if (currentResults !=  null)
        {
            ShowResults(currentResults);
        }
        */
        // ボタンのクリックイベント登録
        RetryButton.onClick.AddListener(OnRetryClicked);
        ExitButton.onClick.AddListener(OnExitClicked);
    }

    private void Update()
    {
        
    }
    public void ShowResults(List<PlayerResult> results)
        {
        // スコア順に降順ソート
        var ranked = results.OrderByDescending(r => r.TotalScore).ToList();

        int currentRank = 1;       // 現在の順位カウンタ
        int displayRank = 1;       // 実際に割り当てる順位
        int previousScore = -1;    // 前のスコア

        for (int i = 0; i < ranked.Count; i++)
        {
            if (ranked[i].TotalScore != previousScore)
            {
                displayRank = currentRank; // スコアが変わったら順位更新
            }
            ranked[i].rank = displayRank;
            previousScore = ranked[i].TotalScore;
            currentRank++;
        }

        int maxScore = ranked.First().TotalScore;

        // 既存パネル削除
        foreach (Transform child in playerPanelParent)
        {
            Destroy(child.gameObject);
        }

        // 固定順: Player → NPC1 → NPC2 → NPC3
        PlayerResult player = results.Find(r => r.playerName == "Player");
        PlayerResult npc1 = results.Find(r => r.playerName == "NPC1");
        PlayerResult npc2 = results.Find(r => r.playerName == "NPC2");
        PlayerResult npc3 = results.Find(r => r.playerName == "NPC3");

        PlayerResult[] displayOrder = new PlayerResult[] { player, npc1, npc2, npc3 };

        // パネル生成
        foreach (var result in displayOrder)
        {
            var panel = Instantiate(playerPanelPrefab, playerPanelParent);

            panel.transform.Find("CharacterImage").GetComponent<Image>().sprite = result.CharacterIcon;
            panel.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = result.playerName;

            var rankText = panel.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            rankText.text = result.TotalScore == maxScore
                ? $"Winner!\nRank: {result.rank}"
                : $"Rank: {result.rank}";
            rankText.color = result.TotalScore == maxScore ? Color.yellow : Color.white;

            panel.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = $"Score: {result.TotalScore}";
        }
    }
    //もう一度同じゲームをプレイする(シーン移動はまだ未実装)
    private void OnRetryClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    //ゲームを終了してタイトルに戻る
    private void OnExitClicked()
    {
        SceneManager.LoadScene(TitleSceneName);
    }
}
