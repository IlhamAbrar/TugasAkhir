using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

public enum TextureType { Wall, Floor }

public class TextureManager : MonoBehaviour
{
    public DeepAITextureLoader textureLoader;
    public Texture2D fallbackWallTexture;
    public Texture2D fallbackFloorTexture;
    private List<Renderer> wallRenderers = new List<Renderer>();
    private List<Renderer> floorRenderers = new List<Renderer>();
    private Texture2D cachedWallTexture;
    private Texture2D cachedFloorTexture;

    void Awake()
    {
        Debug.Log($"Texture Cache Path Walls: {Application.persistentDataPath}/Textures/Walls");
        Debug.Log($"Texture Cache Path Floors: {Application.persistentDataPath}/Textures/Floors");

        CacheWallRenderers();
        CacheFloorRenderers();
    }

    private void CacheWallRenderers()
    {
        wallRenderers.Clear();
        foreach (Renderer renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (renderer.CompareTag("Wall"))
                wallRenderers.Add(renderer);
        }
    }

    private void CacheFloorRenderers()
    {
        floorRenderers.Clear();
        foreach (Renderer renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (renderer.CompareTag("Floor"))
                floorRenderers.Add(renderer);
        }
    }

    public void AddWallRenderer(Renderer renderer)
    {
        if (!wallRenderers.Contains(renderer))
        {
            wallRenderers.Add(renderer);
            if (cachedWallTexture != null)
                renderer.material.mainTexture = cachedWallTexture;
        }
    }

    public void AddFloorRenderer(Renderer renderer)
    {
        if (!floorRenderers.Contains(renderer))
        {
            floorRenderers.Add(renderer);
            if (cachedFloorTexture != null)
                renderer.material.mainTexture = cachedFloorTexture;
        }
    }

    public IEnumerator SetPromptAndFetchTexture(string userInput, string llmPrompt, TextureType type)
    {
        string cacheKey = GenerateCacheKey(userInput);

        Debug.Log($"Input: '{userInput}' → Cache Key: {cacheKey}");

        if (TryLoadCachedTexture(cacheKey, out Texture2D cachedTex, type))
        {
            ApplyTexture(cachedTex, type);
            yield break;
        }

        string textureTypeFolder = type == TextureType.Wall ? "Walls" : "Floors";

        Texture2D downloadedTexture = null;

        yield return StartCoroutine(
            textureLoader.FetchTexture(
                userInput,
                llmPrompt,
                textureTypeFolder,
                (Texture2D tex) => downloadedTexture = tex
            )
        );

        if (downloadedTexture != null)
        {
            SaveTextureToCache(downloadedTexture, cacheKey, type);
            ApplyTexture(downloadedTexture, type);
        }
        else
        {
            ApplyFallbackTexture(type);
        }
    }

    private string GenerateCacheKey(string userInput)
    {
        string[] words = userInput.ToLower()
            .Split(new char[] { ',', ' ', '|' }, System.StringSplitOptions.RemoveEmptyEntries);

        System.Array.Sort(words);
        string normalizedInput = string.Join(" ", words);

        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(normalizedInput));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString() + ".png";
        }
    }

    private bool TryLoadCachedTexture(string cacheKey, out Texture2D texture, TextureType type)
    {
        string folder = type == TextureType.Wall ? "Walls" : "Floors";
        string filePath = Path.Combine(Application.persistentDataPath, "Textures", folder, cacheKey);

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            Debug.Log($"Loaded cached texture from {filePath}");
            return true;
        }

        texture = null;
        return false;
    }

    private void SaveTextureToCache(Texture2D texture, string cacheKey, TextureType type)
    {
        string folder = type == TextureType.Wall ? "Walls" : "Floors";
        string directory = Path.Combine(Application.persistentDataPath, "Textures", folder);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(Path.Combine(directory, cacheKey), texture.EncodeToPNG());
    }

    private void ApplyTexture(Texture2D texture, TextureType type)
    {
        if (texture == null)
        {
            Debug.LogWarning("Null texture provided, applying fallback texture");
            ApplyFallbackTexture(type);
            return;
        }

        if (type == TextureType.Wall)
            cachedWallTexture = texture;
        else
            cachedFloorTexture = texture;

        List<Renderer> targetRenderers = type == TextureType.Wall ? wallRenderers : floorRenderers;

        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer != null)
                renderer.material.mainTexture = texture;
        }
    }

        private void ApplyFallbackTexture(TextureType type)
    {
        List<Renderer> targetRenderers = type == TextureType.Wall ? wallRenderers : floorRenderers;
        Texture2D fallbackTexture = type == TextureType.Wall ? fallbackWallTexture : fallbackFloorTexture;

        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer != null)
                renderer.material.mainTexture = fallbackTexture != null ? fallbackTexture : Texture2D.grayTexture;
        }
    }

        public void ApplyTextureToSingleRenderer(Renderer renderer, TextureType type)
    {
        Texture2D textureToApply = type == TextureType.Wall ? cachedWallTexture : cachedFloorTexture;
        Texture2D fallbackTexture = type == TextureType.Wall ? fallbackWallTexture : fallbackFloorTexture;
        
        if (textureToApply != null)
        {
            renderer.material.mainTexture = textureToApply;
        }
        else
        {
            renderer.material.mainTexture = fallbackTexture != null ? fallbackTexture : Texture2D.grayTexture;
            //Debug.LogWarning($"No cached {type} texture available, using fallback");
        }
    }


    public void RefreshAllRenderers()
    {
        CacheWallRenderers();
        CacheFloorRenderers();
        
        if (cachedWallTexture != null)
            ApplyTexture(cachedWallTexture, TextureType.Wall);
        
        if (cachedFloorTexture != null)
            ApplyTexture(cachedFloorTexture, TextureType.Floor);
    }
}