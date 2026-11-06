using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [Header("UIパネル参照")]
    public GameObject titlePanel;        // タイトル画面パネル
    public GameObject stageSelectPanel;  // ステージ選択画面パネル

    [Header("音響（任意）")]
    public AudioSource audioSource;
    public AudioClip buttonSound;

    // どのステージが選ばれたかを保持する静的変数（必要な場合に使用）
    public static int SelectedStageID { get; private set; } = 1;

    private void Start()
    {
        // 初期状態：タイトル画面を表示、ステージ選択画面は非表示
        if (titlePanel != null) titlePanel.SetActive(true);
        if (stageSelectPanel != null) stageSelectPanel.SetActive(false);
    }

    /// 
    /// 「ゲーム開始」ボタンを押したとき：ステージ選択画面を開く
    /// 
    public void OnClickStartButton()
    {
        PlaySE();
        if (titlePanel != null) titlePanel.SetActive(false);
        if (stageSelectPanel != null) stageSelectPanel.SetActive(true);
    }

    /// 
    /// 「戻る」ボタンを押したとき：タイトル画面に戻る
    /// 
    public void OnClickBackButton()
    {
        PlaySE();
        if (stageSelectPanel != null) stageSelectPanel.SetActive(false);
        if (titlePanel != null) titlePanel.SetActive(true);
    }

    /// 
    /// ステージボタンを押したとき（シーン名で読み込み）
    /// 例: ボタンの引数に "Stage1", "Stage2" などを渡します
    /// 
    public void LoadStageScene(string sceneName)
    {
        PlaySE();
        SceneManager.LoadScene(sceneName);
    }

    /// 
    /// ステージボタンを押したとき（ID指定で共通ゲームシーンを読み込む場合）
    /// 
    public void SelectStageIDAndStart(int stageID)
    {
        PlaySE();
        SelectedStageID = stageID;
        SceneManager.LoadScene("MainGameScene"); // 共通のゲームシーン名
    }

    private void PlaySE()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }
}