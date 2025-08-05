/*using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using System.Collections;

public class LLMTextureInteraction2 : MonoBehaviour
{
    public LLMCharacter llmCharacter;

    public TMPro.TMP_InputField wallInputField;
    public TMPro.TMP_InputField floorInputField;

    public TMPro.TMP_Text wallAIText;
    public TMPro.TMP_Text floorAIText;

    public TextureManager2 textureManager;

    private void Start()
    {
        wallInputField.onSubmit.AddListener(OnWallInputSubmit);
        floorInputField.onSubmit.AddListener(OnFloorInputSubmit);
    }

    void OnWallInputSubmit(string message)
    {
        wallInputField.interactable = false;
        wallAIText.text = "Generating dungeon wall texture prompt...";

        string structuredPrompt = GenerateWallPrompt(message);
        _ = llmCharacter.Chat(structuredPrompt, (response) => SetAIText(response, true, message), OnLLMReplyComplete);
    }

    void OnFloorInputSubmit(string message)
    {
        floorInputField.interactable = false;
        floorAIText.text = "Generating dungeon floor texture prompt...";

        string structuredPrompt = GenerateFloorPrompt(message);
        _ = llmCharacter.Chat(structuredPrompt, (response) => SetAIText(response, false, message), OnLLMReplyComplete);
    }

    string GenerateWallPrompt(string playerTheme)
    {
        return $"Generate a **seamless 2D pixel art texture** of a **stone wall in a dungeon**. " +
               $"The wall is made of **rugged, naturally formed stone bricks** with **cracks, moss, and slight color variations**. " +
               $"It should feel **aged, medieval, and fit the '{playerTheme}' theme**. " +
               $"Use a **dark, muted fantasy color palette**. " +
               $"Make it **tileable** so it can be used in a game environment.";
    }

    string GenerateFloorPrompt(string playerTheme)
    {
        return $"Generate a **seamless 2D pixel art texture** of a **dungeon floor**. " +
               $"The floor consists of **worn, cracked stone tiles** with **dirt, moss, and broken edges**. " +
               $"It should feel **ancient, weathered, and fit the '{playerTheme}' theme**. " +
               $"Use a **dark, earthy fantasy color palette**. " +
               $"Make it **tileable** so it can be used in a game environment.";
    }

    public void SetAIText(string aiPrompt, bool isWall, string userInput)
    {
        // Start the typewriter animation on the correct text component
        if (isWall)
        {
            StartCoroutine(TypewriterEffect(wallAIText, aiPrompt));
        }
        else
        {
            StartCoroutine(TypewriterEffect(floorAIText, aiPrompt));
        }

        // Trigger texture manager logic
        StartCoroutine(textureManager.SetPromptAndFetchTexture(userInput, aiPrompt, isWall));
    }

    IEnumerator TypewriterEffect(TMPro.TMP_Text textComponent, string fullText)
    {
        textComponent.text = "";
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(0.01f); // typing speed (adjustable)
        }
    }

    public void OnLLMReplyComplete()
    {
        wallInputField.interactable = true;
        floorInputField.interactable = true;

        wallInputField.text = "";
        floorInputField.text = "";
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
        OnLLMReplyComplete();
    }
}
*/