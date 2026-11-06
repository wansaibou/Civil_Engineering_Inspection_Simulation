using UnityEngine;

// 検査に必要なツールの定義
public enum ToolType
{
    Chalk,  // チョーク（見た目でわかる異常用）
    Hammer  // ハンマー（内部の異常・打音用）
}

[CreateAssetMenu(fileName = "NewAnomalyData", menuName = "CivilInspection/AnomalyData")]
public class AnomalyData : ScriptableObject
{
    [Header("基本情報")]
    public string anomalyName;          // 異常の名前（例: ひび割れ、内部空洞）
    [TextArea(2, 5)]
    public string description;          // マニュアルに掲載する説明文

    [Header("検査設定")]
    public ToolType requiredTool;       // 発見に必要なツール（Chalk または Hammer）

    [Header("演出・効果音")]
    public AudioClip inspectionSound;   // 打音（音の鳴り分け用）や発見時のSE
    public Sprite anomalySprite;        // 表面に見える異常画像（内部欠落の場合はなしでOK）
}