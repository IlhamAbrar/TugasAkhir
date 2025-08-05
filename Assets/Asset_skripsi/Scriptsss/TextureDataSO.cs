using UnityEngine;

[CreateAssetMenu(fileName = "TextureData", menuName = "Game/TextureData")]
public class TextureDataSO : ScriptableObject
{
    [Header("Wall Data")]
    public string wallTheme;
    public string wallCacheKey;
    public Texture2D wallTexture;

    [Header("Floor Data")]
    public string floorTheme;
    public string floorCacheKey;
    public Texture2D floorTexture;

    public void ResetData()
    {
        wallTheme = string.Empty;
        wallCacheKey = string.Empty;
        wallTexture = null;
        
        floorTheme = string.Empty;
        floorCacheKey = string.Empty;
        floorTexture = null;
    }
}