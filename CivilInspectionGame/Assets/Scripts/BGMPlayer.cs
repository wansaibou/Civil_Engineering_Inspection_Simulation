using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [Header("BGM設定")]
    public AudioClip bgmClip;          // 再生したいBGM音源 (.wav / .mp3)
    [Range(0f, 1f)]
    public float volume = 0.5f;        // BGMの音量（0.0 ～ 1.0）
    public bool loop = true;           // ループ再生するかどうか

    private AudioSource audioSource;

    private void Start()
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("BGMPlayer: bgmClip が設定されていません。");
            return;
        }

        // 自身またはオブジェクトから AudioSource を取得（無ければ自動アタッチ）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // BGMパラメータの設定
        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2Dサウンドとして再生（画面全体で均一に響く）

        // BGMの再生開始
        audioSource.Play();
    }

    /// 
    /// スクリプトから音量を変更したい場合（オプション）
    /// 
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}