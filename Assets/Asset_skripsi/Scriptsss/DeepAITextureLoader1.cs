/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using SimpleJSON;

public class DeepAITextureLoader : MonoBehaviour
{
    [Header("API Settings")]
    public string apiKey = "YOUR_API_KEY";
    public string apiUrl = "https://api.deepai.org/api/pixel-world-generator";

    public IEnumerator FetchTexture(string userInput, string apiPrompt, string cacheSubfolder, System.Action<Texture2D> callback)
    {
        string cacheKey = TextProcessing.GenerateCacheKey(userInput);
        string cachePath = Path.Combine(Application.persistentDataPath, cacheSubfolder);
        
        // Try load from cache
        if(TryLoadFromCache(cachePath, cacheKey, out Texture2D cachedTexture))
        {
            callback(cachedTexture);
            yield break;
        }

        // Download new texture
        yield return StartCoroutine(DownloadTexture(apiPrompt, cachePath, cacheKey, callback));
    }

    private IEnumerator DownloadTexture(string prompt, string cachePath, string cacheKey, System.Action<Texture2D> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("text", prompt);
        
        using(UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            request.SetRequestHeader("api-key", apiKey);
            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error: {request.error}");
                callback(null);
                yield break;
            }

            string imageUrl = JSON.Parse(request.downloadHandler.text)["output_url"];
            yield return StartCoroutine(DownloadImage(imageUrl, cachePath, cacheKey, callback));
        }
    }

    private IEnumerator DownloadImage(string url, string cachePath, string cacheKey, System.Action<Texture2D> callback)
    {
        using(UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = request.SendWebRequest();
            
            while(!asyncOp.isDone)
            {
                UIManager.Instance.UpdateLoadingProgress(asyncOp.progress);
                yield return null;
            }

            if(request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                SaveToCache(texture, cachePath, cacheKey);
                callback(texture);
            }
            else
            {
                callback(null);
            }
        }
    }

    private bool TryLoadFromCache(string cachePath, string cacheKey, out Texture2D texture)
    {
        string fullPath = Path.Combine(cachePath, cacheKey);
        if(File.Exists(fullPath))
        {
            byte[] data = File.ReadAllBytes(fullPath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(data);
            return true;
        }
        texture = null;
        return false;
    }

    private void SaveToCache(Texture2D texture, string cachePath, string cacheKey)
    {
        string fullPath = Path.Combine(cachePath, cacheKey);
        File.WriteAllBytes(fullPath, texture.EncodeToPNG());
    }
}
*/