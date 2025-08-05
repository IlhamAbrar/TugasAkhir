/*using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TMP_InputField wallThemeInput;
    public TMP_InputField floorThemeInput;
    public Button okButton;
    public Button proceedButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI loadingProgress;
    public TextMeshProUGUI wallPromptText;
    public TextMeshProUGUI floorPromptText;
    public float typewriterSpeed = 0.03f;

    [Header("References")]
    public LLMTextureInteraction llmInteraction;
    public TextureManager textureManager;

    private string wallPrompt;
    private string floorPrompt;
    private bool isWallTyping = false;
    private bool isFloorTyping = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        proceedButton.interactable = false;
        okButton.onClick.AddListener(OnOkPressed);
        proceedButton.onClick.AddListener(OnProceedPressed);
        UpdateStatus("Enter themes for Wall and Floor");
    }

    void OnOkPressed()
    {
        if (string.IsNullOrEmpty(wallThemeInput.text)) return;
        if (string.IsNullOrEmpty(floorThemeInput.text)) return;

        StartCoroutine(ProcessThemesSequence());
    }

    IEnumerator ProcessThemesSequence()
    {
        proceedButton.interactable = false;

        // Proses Wall
        UpdateStatus("Generating Wall Prompt...");
        yield return StartCoroutine(llmInteraction.GetLLMPrompt(
            wallThemeInput.text,
            "Wall",
            prompt => {
                wallPrompt = prompt;
                StartCoroutine(TypewriterEffect(wallPrompt, wallPromptText));
                wallThemeInput.interactable = false;
            }
        ));

        // Proses Floor
        UpdateStatus("Generating Floor Prompt...");
        yield return StartCoroutine(llmInteraction.GetLLMPrompt(
            floorThemeInput.text,
            "Floor",
            prompt => {
                floorPrompt = prompt;
                StartCoroutine(TypewriterEffect(floorPrompt, floorPromptText));
                floorThemeInput.interactable = false;
            }
        ));

        // Tunggu sampai kedua typewriter selesai
        yield return new WaitUntil(() => !isWallTyping && !isFloorTyping);

        // Setelah selesai semua
        proceedButton.interactable = true;
        wallThemeInput.interactable = true;
        floorThemeInput.interactable = true;

        Debug.Log("LLM selesai, Typewriter selesai, ProceedButton aktif");
    }

    IEnumerator TypewriterEffect(string text, TextMeshProUGUI targetText)
    {
        if (targetText == wallPromptText) isWallTyping = true;
        else if (targetText == floorPromptText) isFloorTyping = true;

        targetText.text = "";
        foreach (char c in text)
        {
            targetText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        if (targetText == wallPromptText) isWallTyping = false;
        else if (targetText == floorPromptText) isFloorTyping = false;
    }

    void OnProceedPressed()
    {
        StartCoroutine(textureManager.ProcessAllTextures(wallPrompt, floorPrompt));
    }

    public void UpdateStatus(string message)
    {
        statusText.text = message;
    }

    public void UpdateLoadingProgress(float progress)
    {
        loadingProgress.text = $"Loading: {progress * 100:0}%";
    }
}
*/