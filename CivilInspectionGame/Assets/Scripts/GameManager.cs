using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("時間設定")]
    public float totalTime = 120f;
    private float currentTime;
    private bool isGameActive = true;

    [Header("導入説明UI")]
    public GameObject introPanel;            // ★導入説明用パネル（暗い背景）
    public TextMeshProUGUI introText;        // ★説明文テキスト
    private bool isIntroActive = true;       // ★説明表示中フラグ

    public bool IsIntroActive => isIntroActive; // 外部参照用プロパティ

    [Header("スコア設定")]
    public int currentScore = 0;
    public int scorePerSecond = 100;
    public int correctBonus = 1000;
    public int mistakePenalty = 500;
    private float scoreTimer = 0f;

    [Header("判定条件設定")]
    public int maxMistakes = 3;
    public int allowedRemainingAnomalies = 1;

    [Header("UI参照")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private int mistakeCount = 0;
    private int totalAnomalies = 0;
    private int foundAnomalies = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentTime = totalTime;

        // 非アクティブ（非表示）の視点も含めて全ての異変を取得
        InspectionPoint[] allObjects = FindObjectsByType<InspectionPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        totalAnomalies = 0;
        foreach (InspectionPoint point in allObjects)
        {
            if (!point.IsNormal)
            {
                totalAnomalies++;
            }
        }

        Debug.Log($"【ゲーム開始】全異常数: {totalAnomalies}個");
        if (resultPanel != null) resultPanel.SetActive(false);

        // ★導入説明パネルを表示状態にする
        if (introPanel != null)
        {
            introPanel.SetActive(true);
            isIntroActive = true;
        }
        else
        {
            isIntroActive = false;
        }

        UpdateScoreUI();
        UpdateWorkTimeUI();
    }

    private void Update()
    {
        // ★ 1. 導入説明表示中の処理（クリックされるまでタイマーを進めない）
        if (isIntroActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isIntroActive = false;
                if (introPanel != null) introPanel.SetActive(false);
            }
            return; // ★説明表示中はこれ以降のタイマー・スコア処理を実行しない
        }

        // 2. リザルト表示中のユーザーアクション
        if (!isGameActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
            }
            return;
        }

        // 3. 通常のタイムカウントダウン
        currentTime -= Time.deltaTime;
        UpdateWorkTimeUI();

        // 1秒ごとにスコア加算
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= 1.0f)
        {
            currentScore += scorePerSecond;
            scoreTimer -= 1.0f;
            UpdateScoreUI();
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            EvaluateShiftEnd();
        }
    }

    private void UpdateWorkTimeUI()
    {
        if (timerText == null) return;

        float progress = 1.0f - (currentTime / totalTime);
        float startMinutes = 8f * 60f + 30f;
        float totalWorkMinutes = 8.5f * 60f;

        float currentWorkMinutes = startMinutes + (progress * totalWorkMinutes);

        int hours = Mathf.FloorToInt(currentWorkMinutes / 60f);
        int minutes = Mathf.FloorToInt(currentWorkMinutes % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    public void OnChalkUsed(InspectionPoint point)
    {
        if (!isGameActive || isIntroActive) return;

        bool isNormal = (point == null) || point.IsNormal;

        if (isNormal)
        {
            mistakeCount++;
            currentScore = Mathf.Max(0, currentScore - mistakePenalty);
            UpdateScoreUI();

            Debug.Log($"【誤診】誤った箇所を選択しました（{mistakeCount}/{maxMistakes}） -{mistakePenalty}pt");

            if (mistakeCount >= maxMistakes)
            {
                TriggerGameOver("間違い多発による現場混乱のため、点検が打ち切られました。");
            }
        }
        else
        {
            foundAnomalies++;
            currentScore += correctBonus;
            UpdateScoreUI();

            int remaining = totalAnomalies - foundAnomalies;
            Debug.Log($"【異常発見！】残りの異常: {remaining}個 +{correctBonus}pt");

            if (remaining <= 0)
            {
                TriggerClear("すべての異常を発見しました！現場の安全が確認されました。");
            }
        }
    }

    private void EvaluateShiftEnd()
    {
        int remainingAnomalies = totalAnomalies - foundAnomalies;

        if (remainingAnomalies >= 2)
        {
            TriggerGameOver($"3年後、見落とした異常({remainingAnomalies}箇所)が原因で崩落事故が発生しました...");
        }
        else
        {
            TriggerClear("定時（17:00）になりました。見落としも少なく、安全基準を満たしています。");
        }
    }

    private void TriggerGameOver(string reason)
    {
        isGameActive = false;
        Debug.Log($"【GAME OVER】{reason}");
        ShowResult("【 GAME OVER 】", reason);
    }

    private void TriggerClear(string reason)
    {
        isGameActive = false;
        Debug.Log($"【CLEAR】{reason}");
        ShowResult("【 INSPECTION CLEAR 】", reason);
    }

    private void ShowResult(string title, string message)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            if (resultText != null)
            {
                resultText.text = $"{title}\n\n{message}\n\n最終スコア: {currentScore:N0} pt\n\n画面クリックでタイトルへ戻る";
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {currentScore:N0}";
        }
    }
}