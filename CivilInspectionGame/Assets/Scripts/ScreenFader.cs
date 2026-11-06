using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;          // 全画面を覆う黒いImage
    public float fadeDuration = 2.5f; // フェードインにかかる時間（秒）

    private void Start()
    {
        if (fadeImage != null)
        {
            // 開始時は真っ黒（Alpha = 1.0）にする
            Color color = fadeImage.color;
            color.a = 1.0f;
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(true);

            StartCoroutine(FadeInRoutine());
        }
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // 徐々に透明（Alpha = 0.0）にしていく
            color.a = Mathf.Lerp(1.0f, 0.0f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false); // 完全に消えたら非表示にして操作を阻害しないようにする
    }
}