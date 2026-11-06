using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI参照")]
    public TextMeshProUGUI guideText;
    public GameObject manualButton;

    private int step = 0;

    private void Start()
    {
        ShowStepMessage();
    }

    private void ShowStepMessage()
    {
        switch (step)
        {
            case 0:
                guideText.text = "【基本操作 1】\n下部のボタンで「チョーク」を選び、見た目でわかる異常（大きなひび割れなど）をクリックして囲みましょう。";
                break;
            case 1:
                guideText.text = "【基本操作 2】\n次に「ハンマー」を選び、壁をクリックしてみましょう。見た目には見えない内部空洞（鈍い音）を探します。";
                break;
            case 2:
                guideText.text = "【マニュアルの活用】\n画面上の「マニュアル」ボタンを開いてみましょう。現場で迷ったら、マニュアルのイラストと見比べながら異常を判断します！";
                if (manualButton != null) manualButton.SetActive(true);
                break;
            case 3:
                guideText.text = "【チュートリアル完了】\n基本操作はマスターしました！「タイトルへ戻る」ボタンを押して本編に挑戦しましょう。";
                break;
        }
    }

    /// 
    /// 各操作を行った際に次のステップへ進めるメソッド
    /// 
    public void AdvanceStep(int currentCompletedStep)
    {
        if (step == currentCompletedStep)
        {
            step++;
            ShowStepMessage();
        }
    }
}