/*using UnityEngine;
using LLMUnity;
using System.Linq;
using TMPro;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;

public class LLMTextureInteraction : MonoBehaviour
{
    // References (HAPUS PLAYER TEXT & AI TEXT)
    public LLMCharacter llmCharacter;
    public TextureManager textureManager;

    [TextArea] public string baseTemplate = 
        "[SYSTEM_PROMPT]\n\n" +
        "Generate texture description for theme: {theme}\n" +
        "Strictly follow this format:\n" +
        "---\n" +
        "Seamless 2D pixel art texture of a {theme} dungeon wall.\n" +
        "Features: [DETAILS].\n" +
        "Technical: [BIT_STYLE]-bit, [TILING_TYPE].\n" +
        "Colors: [PRIMARY] + [SECONDARY] + [ACCENT].\n" +
        "---";

    void Start()
    {
        // HAPUS BAGIAN INI (TIDAK BUTUH PLAYER TEXT LAGI)
        // playerText.onSubmit.AddListener(OnInputFieldSubmit);
        // playerText.Select();
        
        if (llmCharacter != null)
            baseTemplate = baseTemplate.Replace("[SYSTEM_PROMPT]", llmCharacter.prompt);
    }

    // HAPUS METHOD INI (SUDAH DITANGANI OLEH UIMANAGER)
    // void OnInputFieldSubmit(string message) { ... }

    private string SemanticNormalize(string input)
    {
        string[] connectors = { "and", "with", "featuring", "in", "of" };
        string[] words = input.ToLower()
            .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(part => part.Split(' '))
            .Where(word => !connectors.Contains(word))
            .ToArray();
        
        Array.Sort(words);
        return string.Join(" ", words).Trim();
    }

    private string BuildLLMPrompt(string theme)
    {
        return baseTemplate
            .Replace("{theme}", theme)
            .Replace("[SYSTEM_PROMPT]", llmCharacter.prompt);
    }

    // HAPUS PARAMETER YANG TIDAK DIGUNAKAN
    private IEnumerator ProcessTextureRequest(string normalizedInput, string prompt)
    {
        bool responseReceived = false;
        string llmResponse = "";
        
        llmCharacter.Chat(prompt, response => {
            if (ValidateResponse(response))
            {
                llmResponse = PostProcessResponse(response);
                responseReceived = true;
            }
        });

        yield return new WaitUntil(() => responseReceived);
        
        // HAPUS REFERENSI KE AI TEXT
        // AIText.text = llmResponse;
        
        StartCoroutine(textureManager.SetPromptAndFetchTexture(
            normalizedInput, 
            llmResponse,
            "Wall" // Default type
        ));
        
        // HAPUS ONLLMREPLYCOMPLETE() JIKA TIDAK DIBUTUHKAN
    }

    public IEnumerator GetLLMPrompt(string theme, string type, System.Action<string> callback)
    {
        string normalizedTheme = SemanticNormalize(theme);
        string prompt = BuildLLMPrompt(normalizedTheme).Replace("wall", type.ToLower());
        
        bool responseReceived = false;
        string llmResponse = "";
        
        llmCharacter.Chat(prompt, response => {
            if (ValidateResponse(response))
            {
                llmResponse = PostProcessResponse(response);
                responseReceived = true;
            }
        });

        yield return new WaitUntil(() => responseReceived);
        callback?.Invoke(llmResponse);
    }

    private bool ValidateResponse(string response)
    {
        string[] requiredKeywords = { "Seamless 2D", "Features:", "Technical:", "Colors:" };
        return requiredKeywords.All(keyword => response.Contains(keyword));
    }

    private string PostProcessResponse(string response)
    {
        return response.Replace("---", "").Replace("**", "").Trim();
    }

    // HAPUS METHOD INI (SUDAH DITANGANI OLEH UIMANAGER)
    // public void OnLLMReplyComplete() { ... }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
        // HAPUS REFERENSI KE ONLLMREPLYCOMPLETE()
    }

    public void ExitGame()
    {
        Debug.Log("Game exited by user");
        Application.Quit();
    }
}
*/