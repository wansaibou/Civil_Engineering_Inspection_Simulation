using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManualController : MonoBehaviour
{
    [Header("ページ設定")]
    public GameObject[] pages;           // 各ページのPanelオブジェクト
    public TextMeshProUGUI pageNumText;  // 「1 / 3」などのページ表示用テキスト

    private int currentPage = 0;

    private void OnEnable()
    {
        // マニュアルを開いたときは必ず1ページ目から表示
        currentPage = 0;
        UpdatePageVisibility();
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            UpdatePageVisibility();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageVisibility();
        }
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == currentPage);
            }
        }

        if (pageNumText != null)
        {
            pageNumText.text = $"{currentPage + 1} / {pages.Length}";
        }
    }
}