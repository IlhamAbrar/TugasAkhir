/*using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

public class TextureManager2 : MonoBehaviour
{
    public DeepAITextureLoader2 textureLoader;
    private Dictionary<string, List<Renderer>> renderersByType = new Dictionary<string, List<Renderer>>();

    void Awake()
    {
        Debug.Log("Cache Path: " + Application.persistentDataPath);

        renderersByType["Wall"] = new List<Renderer>();
        renderersByType["Floor"] = new List<Renderer>();

        foreach (Renderer renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (renderer.gameObject.CompareTag("Wall"))
                renderersByType["Wall"].Add(renderer);
            else if (renderer.gameObject.CompareTag("Floor"))
                renderersByType["Floor"].Add(renderer);
        }
    }

    public IEnumerator SetPromptAndFetchTexture(string userInput, string aiPrompt, bool isWall)
    {
        string textureType = isWall ? "wall" : "floor"; // Determine texture type
        Debug.Log($"TextureManager2: Fetching AI texture for {textureType} with user input: {userInput}");

        // Generate unique hash from user input only
        string hashedInput = HashInput(userInput);
        string filePath = Path.Combine(Application.persistentDataPath, $"{textureType}_{hashedInput}.png");

        // Check if texture is already cached
        if (File.Exists(filePath))
        {
            Debug.Log($"TextureManager2: Cached {textureType} texture found. Loading from cache.");
            yield return StartCoroutine(LoadTextureFromCache(filePath, isWall));
            yield break;
        }

        // If not cached, fetch from DeepAI using AI-generated prompt
        yield return StartCoroutine(DownloadTextureFromDeepAI(aiPrompt, filePath, isWall));
    }


    private string HashInput(string userInput)
    {
        // Normalize input: lowercase and alphabetically sorted words
        string[] words = userInput.ToLower()
            .Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        System.Array.Sort(words);
        string normalizedInput = string.Join("|", words);

        // Generate MD5 hash
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(normalizedInput));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }


    private IEnumerator LoadTextureFromCache(string filePath, bool isWall)
    {
        string textureType = isWall ? "Wall" : "Floor";
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);

        if (ImageConversion.LoadImage(texture, fileData))
        {
            ApplyTexture(texture, textureType);
        }
        else
        {
            Debug.LogError($"Failed to load cached {textureType} texture.");
        }
        yield break;
    }

    private IEnumerator DownloadTextureFromDeepAI(string prompt, string filePath, bool isWall)
    {
        string textureType = isWall ? "Wall" : "Floor";
        Debug.Log($"TextureManager2: Requesting new {textureType} texture from DeepAI...");

        yield return StartCoroutine(textureLoader.FetchTexture(prompt, textureType, (Texture2D texture, string returnedType) =>
    {
        if (texture != null)
        {
            File.WriteAllBytes(filePath, texture.EncodeToPNG());
            ApplyTexture(texture, returnedType); // Use returnedType to ensure consistency
        }
        else
        {
            Debug.LogError($"TextureManager2: Failed to fetch {returnedType} texture from DeepAI.");
        }
    }));

    }

    private void ApplyTexture(Texture2D texture, string type)
    {
        if (texture == null)
        {
            Debug.LogError($"{type} texture loading failed. Applying fallback texture.");
            ApplyFallbackTexture(type);
            return;
        }

        foreach (Renderer renderer in renderersByType[type])
        {
            renderer.material.mainTexture = texture;
        }
    }

    private void ApplyFallbackTexture(string type)
    {
        foreach (Renderer renderer in renderersByType[type])
        {
            renderer.material.mainTexture = Texture2D.grayTexture;
        }
    }
}
*/
