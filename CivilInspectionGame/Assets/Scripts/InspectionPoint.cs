using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InspectionPoint : MonoBehaviour
{
    [Header("異常データ (正常箇所のときは None のまま)")]
    public AnomalyData anomalyData;

    [Header("状態")]
    public bool isDiscovered = false; // すでにチェックされたか

    // 異常データがセットされていない場所 ＝ 正常な場所
    public bool IsNormal => anomalyData == null;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    /// 
    /// 異常データのスプライトがあれば自動適用
    /// 
    private void UpdateVisuals()
    {
        if (spriteRenderer != null && anomalyData != null && anomalyData.anomalySprite != null)
        {
            spriteRenderer.sprite = anomalyData.anomalySprite;
        }
    }

    /// 
    /// 検査済みに変更
    /// 
    public void MarkAsDiscovered()
    {
        isDiscovered = true;
    }
}