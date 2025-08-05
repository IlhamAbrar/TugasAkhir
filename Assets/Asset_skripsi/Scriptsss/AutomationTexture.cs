using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AutomationTexture : MonoBehaviour
{
    public TextureType textureType = TextureType.Wall;

    private void Start()
    {
        ApplyTexture();
    }

    public void ApplyTexture()
    {
        Renderer renderer = GetComponent<Renderer>();
        TextureManager textureManager = FindObjectOfType<TextureManager>();

        if (textureManager != null)
        {
            textureManager.ApplyTextureToSingleRenderer(renderer, textureType);
        }
        else
        {
            Debug.LogWarning("TextureManager not found, cannot apply textures");
        }

        // Auto-tag the object for system compatibility
        gameObject.tag = textureType.ToString();
    }
}
