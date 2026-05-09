using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

//ゲーム内でアイテムを取得した時のスコア管理プログラム
public class ScoreManager : MonoBehaviour 
{
    public static ScoreManager instance { get; private set; }

    public List<ResultUIManager.PlayerResult> results { get; private set; } = new List<ResultUIManager.PlayerResult>();

    //ゲームデータの保存（終了してもプレイデータは保存される）
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ゲーム中にアイテム取得時などに呼び出してスコアを加算する
    //public void AddItem(string playerName, string itemType, Sprite icon = null)
    //{
    //    var player = results.Find(p => p.playerName == playerName);
    //    if(player == null)
    //    {
    //        player = new ResultUIManager.PlayerResult
    //        {
    //            playerName = playerName,
    //            CharacterIcon = icon,
    //        };
    //        results.Add(player);
    //    }

    //    //アイテムごとの加算
    //    switch (itemType)
    //    {
    //        case "komatsuna": player.komatsunaCount++; break;
    //        case "financier": player.financierCount++; break;
    //        case "madeleine": player.madeleineCount++; break;
    //        case "windChime": player.windChimeCount++; break;
    //        case "glass": player.glassCount++; break;
    //        case "umbrella": player.umbrellaCount++; break;
    //    }
    //}

    // PlayerScriptインスタンスから情報を取り込むメソッド
    public void CollectScores(Sprite[] icons = null)
    {
        results.Clear();

        var players = GameObject.FindObjectsOfType<PlayerScript_Sho>();

        ResultUIManager.PlayerResult playerResult = null;
        ResultUIManager.PlayerResult npc1 = null;
        ResultUIManager.PlayerResult npc2 = null;
        ResultUIManager.PlayerResult npc3 = null;

        foreach (var p in players)
        {
            var result = new ResultUIManager.PlayerResult
            {
                playerName = (p.playerID == 0) ? "Player" : $"NPC{p.playerID}",
                CharacterIcon = (icons != null && p.playerID < icons.Length) ? icons[p.playerID] : null,
            };

            // PlayerScript の score を TotalScore に代入
            result.TotalScore = p.score;

            // 固定順で変数に代入
            switch (p.playerID)
            {
                case 0: playerResult = result; break;
                case 1: npc1 = result; break;
                case 2: npc2 = result; break;
                case 3: npc3 = result; break;
            }
        }

        // 固定順でリストに追加
        results.Add(playerResult);
        results.Add(npc1);
        results.Add(npc2);
        results.Add(npc3);
    }

    // ゲーム開始時にスコアリセット
    public void ClearScore()
    {
        results.Clear();
    }
}
