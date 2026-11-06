using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    [Header("切替対象の視点オブジェクトリスト")]
    public GameObject[] inspectionViews; // 各画像＋コライダーをまとめた親オブジェクト

    private int currentIndex = 0;

    void Start()
    {
        UpdateViewDisplay();
    }

    /// 
    /// 次の画像（視点）へ
    /// 
    public void NextView()
    {
        if (inspectionViews.Length == 0) return;

        currentIndex = (currentIndex + 1) % inspectionViews.Length;
        UpdateViewDisplay();
    }

    /// 
    /// 前の画像（視点）へ
    /// 
    public void PrevView()
    {
        if (inspectionViews.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = inspectionViews.Length - 1;
        }
        UpdateViewDisplay();
    }

    /// 
    /// 現在の視点だけを表示し、他を非表示にする
    /// 
    private void UpdateViewDisplay()
    {
        for (int i = 0; i < inspectionViews.Length; i++)
        {
            if (inspectionViews[i] != null)
            {
                inspectionViews[i].SetActive(i == currentIndex);
            }
        }
    }
}