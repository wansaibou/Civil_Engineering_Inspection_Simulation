using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRouter : MonoBehaviour
{
    /// 
    /// タイトル画面へ遷移
    /// 
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// 
    /// チュートリアルへ遷移
    /// 
    public void LoadTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    /// 
    /// 本編ゲーム画面へ遷移
    /// 
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// 
    /// 現在のシーンをリトライ
    /// 
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}