using UnityEngine;

public class FadeOutRoom : MonoBehaviour
{
    [Header("Referensi")]
    public SpriteRenderer blackOverlay; // Hubungkan ke objek "Square"

    [Header("Pengaturan")]
    public float fadeSpeed = 2f;

    private bool hasFaded = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasFaded)
        {
            hasFaded = true;
            Debug.Log("Fade dimulai!");
        }
    }

    void Update()
    {
        if (hasFaded && blackOverlay.color.a > 0)
        {
            Color newColor = blackOverlay.color;
            newColor.a -= fadeSpeed * Time.deltaTime;
            blackOverlay.color = newColor;

            if (newColor.a <= 0)
            {
                blackOverlay.gameObject.SetActive(false); // 🔥 Turn off the entire overlay object
                enabled = false; // Nonaktifkan script ini
            }
        }
    }
}
