using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    [Header("呼吸（縦方向の拡大縮小）の設定")]
    public bool enableScaleBreathing = true;
    public float scaleSpeed = 2.0f;       // 呼吸の速さ（1往復にかかる時間）
    public float scaleAmount = 0.02f;     // 拡大縮小の大きさ（例: 0.02 = 2%伸び縮み）

    [Header("上下揺れ（上下移動）の設定")]
    public bool enablePositionBobbing = true;
    public float positionSpeed = 2.0f;    // 上下移動の速さ
    public float positionAmount = 3.0f;   // 上下移動の幅（ピクセル単位）

    private Vector3 initialScale;
    private Vector3 initialPosition;

    private void Start()
    {
        // 開始時点のスケールと位置を記憶
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // 時間経過で -1 ～ 1 の間を滑らかに往復する値を計算
        float sinValue = Mathf.Sin(Time.time * scaleSpeed);

        // 1. 縦方向にわずかに伸ばして「胸が上下に膨らむ」呼吸感を表現
        if (enableScaleBreathing)
        {
            float scaleFactorY = 1f + (sinValue * scaleAmount);
            transform.localScale = new Vector3(initialScale.x, initialScale.y * scaleFactorY, initialScale.z);
        }

        // 2. わずかに上下移動させて「体が揺れている」感を表現
        if (enablePositionBobbing)
        {
            float yOffset = Mathf.Sin(Time.time * positionSpeed) * positionAmount;
            transform.localPosition = initialPosition + new Vector3(0, yOffset, 0);
        }
    }
}