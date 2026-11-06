using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InspectionController : MonoBehaviour
{
    public enum ToolMode
    {
        Normal,  // 通常（報告・チョーク）モード
        Hammer,  // ハンマーモード
        Manual   // マニュアル閲覧モード
    }

    [Header("現在のモード")]
    public ToolMode currentTool = ToolMode.Normal;

    [Header("長押し設定（通常モード）")]
    public float requiredHoldTime = 3.0f;
    private float currentHoldTime = 0f;
    private bool isHolding = false;
    public Image holdGaugeUI;

    [Header("ハンマー設定・クールタイム")]
    public float hammerCooldown = 10.0f;
    private float currentHammerCooldown = 0f;
    public Image hammerCooldownGauge;
    public TextMeshProUGUI hammerCooldownText;

    [Header("ツールバーUI参照（透明度制御用）")]
    public Image[] toolButtonImages; // [0]: 通常, [1]: ハンマー, [2]: マニュアル

    [Header("コンポーネント・効果音設定")]
    public AudioSource audioSource;
    public AudioClip defaultHammerSound;  // 通常打音（カンカン）
    public AudioClip chalkSound;           // チョーク書き込み音（シュッ）
    public AudioClip toolChangeSound;      // モード切り替え音
    public AudioClip errorSound;           // クールタイム中のエラー音

    public GameObject chalkMarkPrefab;
    public LayerMask inspectionLayer;

    [Header("UI参照")]
    public GameObject manualUIPanel;
    public GameObject judgmentPopupPrefab;

    private void Start()
    {
        UpdateToolButtonVisuals();
    }

    private void Update()
    {

        if ((GameManager.Instance != null && GameManager.Instance.IsIntroActive) ||
        (manualUIPanel != null && manualUIPanel.activeSelf))
        {
            return;
        }

        // ハンマーのクールタイム進行
        if (currentHammerCooldown > 0f)
        {
            currentHammerCooldown -= Time.deltaTime;
            if (currentHammerCooldown < 0f) currentHammerCooldown = 0f;
            UpdateHammerCooldownUI();
        }

        // マニュアル表示中・UI操作中は現場操作をスキップ
        if (manualUIPanel != null && manualUIPanel.activeSelf) return;

        switch (currentTool)
        {
            case ToolMode.Hammer:
                HandleHammerInput();
                break;

            case ToolMode.Normal:
                HandleNormalHoldInput();
                break;

            case ToolMode.Manual:
                break;
        }
    }

    private void HandleHammerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            PerformHammerInspection();
        }
    }

    private void PerformHammerInspection()
    {
        if (currentTool != ToolMode.Hammer) return;

        if (currentHammerCooldown > 0f)
        {
            if (audioSource != null && errorSound != null) audioSource.PlayOneShot(errorSound);
            Debug.Log($"ハンマー冷却中... 残り {currentHammerCooldown:F1} 秒");
            return;
        }

        InspectionPoint point = GetTargetPoint();

        if (point != null && !point.IsNormal && point.anomalyData != null && point.anomalyData.requiredTool == ToolType.Hammer)
        {
            AudioClip clip = point.anomalyData.inspectionSound != null
                ? point.anomalyData.inspectionSound
                : defaultHammerSound;

            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        }
        else
        {
            if (audioSource != null && defaultHammerSound != null) audioSource.PlayOneShot(defaultHammerSound);
        }

        currentHammerCooldown = hammerCooldown;
        UpdateHammerCooldownUI();
    }

    private void HandleNormalHoldInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            isHolding = true;
            currentHoldTime = 0f;
            if (holdGaugeUI != null) holdGaugeUI.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0) && isHolding)
        {
            currentHoldTime += Time.deltaTime;

            if (holdGaugeUI != null)
            {
                holdGaugeUI.fillAmount = currentHoldTime / requiredHoldTime;
                holdGaugeUI.transform.position = Input.mousePosition;
            }

            if (currentHoldTime >= requiredHoldTime)
            {
                PerformChalkReport();
                ResetHoldState();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetHoldState();
        }
    }

    /// 
    /// チョーク報告の実行
    /// 
    private void PerformChalkReport()
    {
        if (currentTool != ToolMode.Normal) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero, Mathf.Infinity, inspectionLayer);

        InspectionPoint point = null;
        Transform parentTransform = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            // 非表示（非アクティブ）状態の画面のコライダーは完全に無視する
            if (!hit.collider.gameObject.activeInHierarchy) continue;

            // 異変（InspectionPoint）があれば優先確定
            InspectionPoint p = hit.collider.GetComponent<InspectionPoint>();
            if (p != null)
            {
                point = p;
                parentTransform = hit.collider.transform;
                break;
            }

            // 表示中の背景コライダーを親の候補として保持
            if (parentTransform == null)
            {
                parentTransform = hit.collider.transform;
            }
        }

        // すでに報告済みの異変ならスキップ
        if (point != null && point.isDiscovered) return;

        // まだ親が見つかっていない場合、画面上に見えている（activeInHierarchy）コライダーをサーチ
        if (parentTransform == null)
        {
            Collider2D[] overlapColliders = Physics2D.OverlapPointAll(mouseWorldPos);
            foreach (var col in overlapColliders)
            {
                if (col != null && col.gameObject.activeInHierarchy)
                {
                    parentTransform = col.transform;
                    break;
                }
            }
        }

        // チョーク音の再生
        if (chalkSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chalkSound);
        }

        // 印の生成と描画順（sortingOrder）の強制補正
        if (chalkMarkPrefab != null)
        {
            GameObject markObj = Instantiate(chalkMarkPrefab, mouseWorldPos, Quaternion.identity, parentTransform);
            markObj.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, -0.1f); // 少し手前(Z=-0.1)に配置

            // 親のスケール（拡大率）の影響で潰れないようリセット
            markObj.transform.localScale = Vector3.one;

            // 背景画像の後ろに隠れないよう SpriteRenderer の描画順を背景より手前（例: 10）にする
            SpriteRenderer sr = markObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
            }
        }
        else
        {
            Debug.LogWarning("【警告】Chalk Mark Prefab が Inspector にアタッチされていません！");
        }

        if (point != null)
        {
            point.MarkAsDiscovered();
        }

        // 判定ポップアップの表示
        ShowJudgmentPopup(point, mouseWorldPos, parentTransform);

        // GameManagerへ通知
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChalkUsed(point);
        }
    }

    /// 
    /// 判定ポップアップの表示
    /// 
    private void ShowJudgmentPopup(InspectionPoint point, Vector3 position, Transform parent)
    {
        if (judgmentPopupPrefab == null) return;

        GameObject popupObj = Instantiate(judgmentPopupPrefab, position, Quaternion.identity, parent);
        popupObj.transform.position = new Vector3(position.x, position.y, 0f); // Z軸補正

        JudgmentPopup popup = popupObj.GetComponent<JudgmentPopup>();

        if (popup != null)
        {
            if (point == null || point.IsNormal)
            {
                popup.Setup("X 誤診", Color.red);
            }
            else
            {
                popup.Setup("O 正解！", Color.green);
            }
        }
    }

    /// 
    /// クリック箇所の InspectionPoint を優先取得
    /// 
    private InspectionPoint GetTargetPoint()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity, inspectionLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                InspectionPoint point = hit.collider.GetComponent<InspectionPoint>();
                if (point != null) return point;
            }
        }
        return null;
    }

    private void ResetHoldState()
    {
        isHolding = false;
        currentHoldTime = 0f;
        if (holdGaugeUI != null)
        {
            holdGaugeUI.fillAmount = 0f;
            holdGaugeUI.gameObject.SetActive(false);
        }
    }

    public void SetToolMode(int modeIndex)
    {
        if ((int)currentTool != modeIndex)
        {
            if (audioSource != null && toolChangeSound != null)
            {
                audioSource.PlayOneShot(toolChangeSound);
            }
        }

        currentTool = (ToolMode)modeIndex;
        ResetHoldState();

        if (manualUIPanel != null)
        {
            manualUIPanel.SetActive(currentTool == ToolMode.Manual);
        }

        UpdateToolButtonVisuals();
    }

    private void UpdateToolButtonVisuals()
    {
        if (toolButtonImages == null || toolButtonImages.Length == 0) return;

        for (int i = 0; i < toolButtonImages.Length; i++)
        {
            if (toolButtonImages[i] == null) continue;

            Color color = toolButtonImages[i].color;
            color.a = (i == (int)currentTool) ? 1.0f : 0.5f;
            toolButtonImages[i].color = color;
        }
    }

    private void UpdateHammerCooldownUI()
    {
        if (hammerCooldownGauge != null)
        {
            hammerCooldownGauge.fillAmount = currentHammerCooldown / hammerCooldown;
        }

        if (hammerCooldownText != null)
        {
            if (currentHammerCooldown > 0f)
            {
                hammerCooldownText.gameObject.SetActive(true);
                hammerCooldownText.text = $"{Mathf.CeilToInt(currentHammerCooldown)}s";
            }
            else
            {
                hammerCooldownText.gameObject.SetActive(false);
            }
        }
    }
}