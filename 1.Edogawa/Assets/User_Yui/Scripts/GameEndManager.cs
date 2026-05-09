using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    //ゲーム終了:ボタンから呼び出す
    public void EndGame()
    {
#if UNITY_EDITOR
        //ゲームプレイ終了
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //ゲームのプレイ終了
        Application.Quit();
#endif
    }
}
