using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // スタートボタンを押したとき
    public void OnStartButton()
    {
        SceneManager.LoadScene("MainScene"); // ゲームシーンへ移動
    }

    // オプションボタンを押したとき
    public void OnOptionsButton()
    {
        Debug.Log("オプション画面を開く（仮）");
        // TODO: 実際のオプション画面を作る場合はここに処理を追加
    }

    // 終了ボタンを押したとき
    public void OnQuitButton()
    {
        Debug.Log("ゲーム終了");
        Application.Quit(); // ゲームを終了（エディタでは無効）
    }
}
