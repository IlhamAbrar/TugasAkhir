using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SimpleJSON;

public class DeepAITextureLoader : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiKey = "YOUR_DEEPAI_API_KEY";
    [SerializeField] private string apiUrl = "https://api.deepai.org/api/pixel-world-generator";

    // --- PUBLIC method ---
    // userInput: input user untuk cache key
    // apiPrompt: prompt yg dikirim ke API DeepAI
    // textureType: folder cache misalnya "Walls" atau "Floors"
    // callback: callback texture setelah didapatkan
    public IEnumerator FetchTexture(string userInput, string apiPrompt, string textureType, System.Action<Texture2D> callback)
    {
        string cacheSubfolder = $"Textures/{textureType}";
        string cacheKey = GenerateCacheKey(userInput);

        // cek cache dulu
        if (TryLoadFromCache(cacheSubfolder, cacheKey, out Texture2D cachedTexture))
        {
            Debug.Log($"Loaded from cache: {cacheKey} in {cacheSubfolder}");
            callback?.Invoke(cachedTexture);
            yield break;
        }

        // kalau tidak ada di cache, request API
        yield return StartCoroutine(FetchFromAPI(apiPrompt, cacheSubfolder, cacheKey, callback));
    }

    // --- PRIVATE methods ---

    private IEnumerator FetchFromAPI(string apiPrompt, string cacheSubfolder, string cacheKey, System.Action<Texture2D> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("text", apiPrompt);
        form.AddField("tileable", "true");
        form.AddField("symmetry", "mirror");
        form.AddField("grid_size", "32");

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            request.SetRequestHeader("api-key", apiKey);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error: {request.error}\nResponse: {request.downloadHandler.text}");
                callback?.Invoke(null);
                yield break;
            }

            string imageUrl = JSON.Parse(request.downloadHandler.text)["output_url"];
            if (string.IsNullOrEmpty(imageUrl))
            {
                Debug.LogError("No image URL in API response");
                callback?.Invoke(null);
                yield break;
            }

            yield return StartCoroutine(DownloadAndCacheTexture(imageUrl, cacheSubfolder, cacheKey, callback));
        }
    }

    private IEnumerator DownloadAndCacheTexture(string imageUrl, string cacheSubfolder, string cacheKey, System.Action<Texture2D> callback)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                SaveToCache(texture, cacheSubfolder, cacheKey);
                callback?.Invoke(texture);
            }
            else
            {
                Debug.LogError($"Download Failed: {request.error}");
                callback?.Invoke(null);
            }
        }
    }

    private bool TryLoadFromCache(string cacheSubfolder, string cacheKey, out Texture2D texture)
    {
        string filePath = Path.Combine(Application.persistentDataPath, cacheSubfolder, cacheKey);

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return true;
        }

        texture = null;
        return false;
    }

    private void SaveToCache(Texture2D texture, string cacheSubfolder, string cacheKey)
    {
        string directory = Path.Combine(Application.persistentDataPath, cacheSubfolder);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(Path.Combine(directory, cacheKey), texture.EncodeToPNG());
        Debug.Log($"Texture cached: {cacheKey} at {cacheSubfolder}");
    }

    private string GenerateCacheKey(string userInput)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(userInput));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString() + ".png";
        }
    }
}
