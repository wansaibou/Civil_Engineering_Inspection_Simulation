using UnityEngine;
using TMPro;

public class JudgmentPopup : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float destroyTime = 1.5f;

    public void Setup(string message, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = message;
            textMesh.color = color;
        }
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 浮き上がりながら少しづつ消えるアニメーション
        transform.position += Vector3.up * 0.5f * Time.deltaTime;
    }
}