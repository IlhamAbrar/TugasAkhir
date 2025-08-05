/*using UnityEngine;
using System.Collections;
using System.IO;

public class TextureManager : MonoBehaviour
{
    public DeepAITextureLoader textureLoader;

    void Awake()
    {
        CreateTextureFolders();
    }

    public IEnumerator SetPromptAndFetchTexture(string normalizedInput, string llmResponse, string type = "Wall")
    {
        yield return StartCoroutine(ProcessTexture(llmResponse, type));
    }

    public IEnumerator ProcessAllTextures(string wallPrompt, string floorPrompt)
    {
        // Process Wall
        yield return StartCoroutine(ProcessTexture(wallPrompt, "Wall"));

        // Process Floor
        yield return StartCoroutine(ProcessTexture(floorPrompt, "Floor"));

        UIManager.Instance.UpdateStatus("All textures applied!");
    }

    IEnumerator ProcessTexture(string prompt, string type)
    {
        string normalizedInput = TextProcessing.NormalizeInput(prompt);
        string cacheSubfolder = $"Textures/{type}s";
        
        Texture2D texture = null;
        yield return StartCoroutine(textureLoader.FetchTexture(
            normalizedInput,
            prompt,
            cacheSubfolder,
            result => texture = result
        ));

        if(texture != null) ApplyTexture(texture, type);
    }

    void ApplyTexture(Texture2D texture, string type)
    {
        var targets = GameObject.FindGameObjectsWithTag(type);
        foreach(var target in targets)
        {
            var renderer = target.GetComponent<Renderer>();
            if(renderer != null) renderer.material.mainTexture = texture;
        }
    }

    void CreateTextureFolders()
    {
        string[] folders = {
            Path.Combine(Application.persistentDataPath, "Textures/Walls"),
            Path.Combine(Application.persistentDataPath, "Textures/Floors")
        };

        foreach(var folder in folders)
        {
            if(!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }
    }
}
*/