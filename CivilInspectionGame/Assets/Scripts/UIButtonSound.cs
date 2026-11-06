using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;        // 鳴らしたいSE
    public AudioSource audioSource;     // 音声を再生するAudioSource（空なら自動指定）

    private void Start()
    {
        // AudioSource が指定されていない場合、シーン内のメインのものを自動検索
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
        }

        // ボタンの OnClick イベントに SE 再生処理を自動追加
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}