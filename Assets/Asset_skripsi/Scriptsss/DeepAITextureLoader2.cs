/*using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class DeepAITextureLoader2 : MonoBehaviour
{
    private const string apiUrl = "https://api.deepai.org/api/pixel-world-generator";
    [SerializeField] private string apiKey = "5a425f9e-0ddc-4b32-8f90-9083b497cb33"; // Put your real DeepAI API key here or set in Inspector

    /// <summary>
    /// Fetches a texture from DeepAI API based on the text prompt.
    /// </summary>
    /// <param name="prompt">Text prompt for image generation</param>
    /// <param name="textureType">"Wall" or "Floor" - passed back in callback for identification</param>
    /// <param name="callback">Callback called with the downloaded Texture2D and textureType</param>
    /// <returns></returns>
    public IEnumerator FetchTexture(string prompt, string textureType, Action<Texture2D, string> callback)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("DeepAITextureLoader2: API key is not set.");
            callback(null, textureType);
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("text", prompt);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            www.SetRequestHeader("api-key", apiKey);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"DeepAITextureLoader2: Failed to fetch texture: {www.error}");
                callback(null, textureType);
                yield break;
            }

            // Parse JSON response to extract image URL
            string json = www.downloadHandler.text;
            string imageUrl = ExtractImageUrlFromJson(json);

            if (string.IsNullOrEmpty(imageUrl))
            {
                Debug.LogError("DeepAITextureLoader2: No image URL found in API response.");
                callback(null, textureType);
                yield break;
            }

            // Download the actual texture image
            yield return DownloadImage(imageUrl, textureType, callback);
        }
    }

    private IEnumerator DownloadImage(string url, string textureType, Action<Texture2D, string> callback)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"DeepAITextureLoader2: Failed to download texture image: {www.error}");
                callback(null, textureType);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                callback(texture, textureType);
            }
        }
    }

    /// <summary>
    /// Simple JSON parsing to get the image URL from DeepAI's response.
    /// This assumes the JSON contains a key "output_url" with the image link.
    /// </summary>
    private string ExtractImageUrlFromJson(string json)
    {
        // Example response:
        // {"id":"someid","output_url":"https://someurl.com/image.png"}

        const string key = "\"output_url\":\"";
        int startIndex = json.IndexOf(key);
        if (startIndex == -1) return null;

        startIndex += key.Length;
        int endIndex = json.IndexOf("\"", startIndex);
        if (endIndex == -1) return null;

        string url = json.Substring(startIndex, endIndex - startIndex);
        return url;
    }
}
*/