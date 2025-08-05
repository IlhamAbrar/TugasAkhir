/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using LLMUnity;

public class UIManager2 : MonoBehaviour
{
    public TMP_InputField wallInputField;
    public TMP_InputField floorInputField;
    public Button okWallButton;
    public Button okFloorButton;
    public Button okButton2;
    public TextMeshProUGUI typewriterText;
    public TextMeshProUGUI typewriterText2;
    public TextMeshProUGUI loadingText;
    public GameObject gameUI;
    public GameObject loadingScreen;
    public LLMCharacter llmCharacter;

    private string wallUserInput;
    private string floorUserInput;
    private string generatedWallPrompt;
    private string generatedFloorPrompt;
    private bool wallPromptReady = false;
    private bool floorPromptReady = false;

    void Start()
    {
        Debug.Log("UIManager2: Initializing UI...");

        if (wallInputField == null) Debug.LogError("wallInputField is NULL!");
        if (floorInputField == null) Debug.LogError("floorInputField is NULL!");
        if (okWallButton == null) Debug.LogError("okWallButton is NULL!");
        if (okFloorButton == null) Debug.LogError("okFloorButton is NULL!");
        if (okButton2 == null) Debug.LogError("okButton2 is NULL!");
        if (typewriterText == null) Debug.LogError("typewriterText is NULL!");
        if (typewriterText2 == null) Debug.LogError("typewriterText2 is NULL!");
        if (loadingText == null) Debug.LogError("loadingText is NULL!");
        if (gameUI == null) Debug.LogError("gameUI is NULL!");
        if (llmCharacter == null) Debug.LogError("llmCharacter is NULL!");

        gameUI.SetActive(true);
        typewriterText.text = "";
        typewriterText2.text = "";
        okButton2.gameObject.SetActive(false);
        okButton2.interactable = false;
        loadingText.gameObject.SetActive(false);

        wallPromptReady = false;
        floorPromptReady = false;

        okWallButton.onClick.AddListener(OnWallOkPressed);
        okFloorButton.onClick.AddListener(OnFloorOkPressed);
        okButton2.onClick.AddListener(OnSecondOkPressed);
    }

    void OnWallOkPressed()
    {
        wallUserInput = wallInputField.text;

        if (string.IsNullOrEmpty(wallUserInput))
        {
            Debug.LogWarning("UIManager2: Wall input is empty.");
            return;
        }

        wallInputField.interactable = false;
        typewriterText.text = "Generating wall prompt...";
        llmCharacter.Chat(wallUserInput, OnWallLLMReplyReceived);
    }

    void OnFloorOkPressed()
    {
        floorUserInput = floorInputField.text;

        if (string.IsNullOrEmpty(floorUserInput))
        {
            Debug.LogWarning("UIManager2: Floor input is empty.");
            return;
        }

        floorInputField.interactable = false;
        typewriterText2.text = "Generating floor prompt...";
        llmCharacter.Chat(floorUserInput, OnFloorLLMReplyReceived);
    }

    void OnWallLLMReplyReceived(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            Debug.LogError("Received empty response for wall.");
            return;
        }

        generatedWallPrompt = response;
        wallPromptReady = true;
        Debug.Log($"Wall LLM Response: {generatedWallPrompt}");

        StopAllCoroutines();
        StartCoroutine(TypewriterEffect(typewriterText, generatedWallPrompt));
    }

    void OnFloorLLMReplyReceived(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            Debug.LogError("Received empty response for floor.");
            return;
        }

        generatedFloorPrompt = response;
        floorPromptReady = true;
        Debug.Log($"Floor LLM Response: {generatedFloorPrompt}");

        StopAllCoroutines();
        StartCoroutine(TypewriterEffect(typewriterText2, generatedFloorPrompt));
    }

    IEnumerator TypewriterEffect(TextMeshProUGUI targetText, string text)
    {
        targetText.text = "";
        foreach (char c in text)
        {
            targetText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        if (wallPromptReady && floorPromptReady)
        {
            okButton2.gameObject.SetActive(true);
            okButton2.interactable = true;
        }
    }

    void OnSecondOkPressed()
    {
        if (!wallPromptReady || !floorPromptReady)
        {
            Debug.LogError("Prompts not ready.");
            return;
        }

        okButton2.interactable = false;

        loadingText.gameObject.SetActive(true);
        loadingText.text = "Loading...";

        StartCoroutine(FetchTextures());
    }

    IEnumerator FetchTextures()
    {
        TextureManager2 textureManager = FindFirstObjectByType<TextureManager2>();

        if (textureManager == null)
        {
            Debug.LogError("TextureManager2 not found.");
            yield break;
        }

        loadingScreen.SetActive(true);
        loadingText.text = "Loading...";

        yield return StartCoroutine(textureManager.SetPromptAndFetchTexture(wallUserInput, generatedWallPrompt, true));
        yield return StartCoroutine(textureManager.SetPromptAndFetchTexture(floorUserInput, generatedFloorPrompt, false));

        yield return new WaitForEndOfFrame();

        loadingScreen.SetActive(false);
        loadingText.gameObject.SetActive(false);
    }
}
*/