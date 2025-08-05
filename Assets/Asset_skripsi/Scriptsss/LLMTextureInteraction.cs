using UnityEngine;
using LLMUnity;
using TMPro;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections;
using static TextureType; // Agar bisa pakai Wall / Floor langsung

public class LLMTextureInteraction : MonoBehaviour
{
    // references
    public LLMCharacter llmCharacter;
    public TMP_InputField playerTextWall;
    public TMP_InputField playerTextFloor;
    public TMP_Text AITextWall;
    public TMP_Text AITextFloor;
    public TextureManager textureManager;

    // Internal buffer untuk typewriter agar smooth
    private string currentWallText = "";
    private string currentFloorText = "";

    private bool isGeneratingWall = false;
    private bool isGeneratingFloor = false;

    private Coroutine wallTypingCoroutine;
    private Coroutine floorTypingCoroutine;


    private const string baseTemplate = 
    "Generate texture for {type} based on theme: {theme}. " + 
    "Follow EXACTLY the format in your instructions.";

    private string GenerateStructuredPrompt(string theme, string type)
    {
        return baseTemplate
            .Replace("{theme}", theme)
            .Replace("{type}", type);
    }

    void Start()
    {
        playerTextWall.onSubmit.AddListener(OnInputWallSubmit);
        playerTextFloor.onSubmit.AddListener(OnInputFloorSubmit);
        playerTextWall.Select();
    }

    void OnInputWallSubmit(string message)
    {
        Debug.Log("OnInputWallSubmit triggered with message: " + message);
        if (string.IsNullOrEmpty(message) || isGeneratingWall || isGeneratingFloor) return;

        isGeneratingWall = true;
        playerTextWall.interactable = false;
        AITextWall.text = "";

        currentWallText = "";

        string normalizedInput = NormalizeInput(message);
        string structuredPrompt = GenerateStructuredPrompt(normalizedInput, "wall");

        StartCoroutine(textureManager.SetPromptAndFetchTexture(
            normalizedInput,
            structuredPrompt,
            TextureType.Wall
        ));

        _ = llmCharacter.Chat(structuredPrompt, AppendAITextWall, OnLLMReplyCompleteWall);
    }

        void AppendAITextWall(string newText)
    {
        string toAppend = newText.Substring(currentWallText.Length);
        currentWallText += toAppend;

        if (wallTypingCoroutine != null)
            StopCoroutine(wallTypingCoroutine);
        
        wallTypingCoroutine = StartCoroutine(TypewriterEffect(AITextWall, currentWallText));
    }


    IEnumerator TypewriterEffect(TMP_Text textUI, string fullText)
    {
        textUI.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            textUI.text += fullText[i];
            yield return new WaitForSeconds(0.03f); // kecepatan typewriter, sesuaikan
        }
    }

    void OnLLMReplyCompleteWall()
    {
        isGeneratingWall = false;
        playerTextWall.interactable = true;
        playerTextWall.text = "";
        playerTextWall.Select();

        // Setelah wall selesai, mulai floor jika ada input floor
        if (!string.IsNullOrEmpty(playerTextFloor.text))
        {
            OnInputFloorSubmit(playerTextFloor.text);
        }
    }

    void OnInputFloorSubmit(string message)
    {
        Debug.Log("OnInputFloorSubmit triggered with message: " + message);
        if (string.IsNullOrEmpty(message) || isGeneratingFloor || isGeneratingWall) return;

        isGeneratingFloor = true;
        playerTextFloor.interactable = false;
        AITextFloor.text = "";

        currentFloorText = "";

        string normalizedInput = NormalizeInput(message);
        string structuredPrompt = GenerateStructuredPrompt(normalizedInput, "floor");

        StartCoroutine(textureManager.SetPromptAndFetchTexture(
            normalizedInput,
            structuredPrompt,
            TextureType.Floor
        ));

        _ = llmCharacter.Chat(structuredPrompt, AppendAITextFloor, OnLLMReplyCompleteFloor);
    }

        void AppendAITextFloor(string newText)
    {
        string toAppend = newText.Substring(currentFloorText.Length);
        currentFloorText += toAppend;

        if (floorTypingCoroutine != null)
            StopCoroutine(floorTypingCoroutine);
        
        floorTypingCoroutine = StartCoroutine(TypewriterEffect(AITextFloor, currentFloorText));
    }


    void OnLLMReplyCompleteFloor()
    {
        isGeneratingFloor = false;
        playerTextFloor.interactable = true;
        playerTextFloor.text = "";
        playerTextFloor.Select();
    }

    // --- helper method tetap sama ---

    private string NormalizeInput(string input)
    {
        string[] words = input.ToLower()
            .Split(new char[] { ',', ' ', '|', '-' }, StringSplitOptions.RemoveEmptyEntries);
        Array.Sort(words);
        return string.Join(" ", words).Trim();
    }

        /*private string GenerateStructuredPrompt(string theme, string type)
    {
        Debug.Log($"GenerateStructuredPrompt called with theme: {theme}, type: {type}");

        string details = type == "wall"
            ? "rugged, naturally formed stone bricks"
            : "worn, cracked stone floor tiles";

        string colors = "colors that best represent the theme";

        return baseTemplate
            .Replace("{theme}", theme)
            .Replace("{type}", type)
            .Replace("{details}", details)
            .Replace("{colors}", colors);
    }
    */


    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
        OnLLMReplyCompleteWall();
        OnLLMReplyCompleteFloor();
    }

    public void ExitGame()
    {
        Debug.Log("Game exited by user");
        Application.Quit();
    }
}
