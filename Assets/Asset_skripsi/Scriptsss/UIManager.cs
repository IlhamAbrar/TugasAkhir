using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using LLMUnity;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Input Fields
    public TMP_InputField wallInputField;
    public TMP_InputField floorInputField;

    // Buttons
    public Button okButton1; // Untuk generate prompt
    public Button okButton2; // Untuk generate texture API

    // Text UI untuk menampilkan prompt hasil LLM
    public TextMeshProUGUI promptWallText;
    public TextMeshProUGUI promptFloorText;

    // Loading & status UI
    public TextMeshProUGUI loadingText;
    public GameObject gameUI;
    public GameObject fullscreenPanel;
    public TextMeshProUGUI statusText;

    // Reference ke LLMCharacter
    public LLMCharacter llmCharacter;

    private CanvasGroup fullscreenCanvasGroup;

    // Menyimpan prompt yang dihasilkan LLM
    private string generatedPromptWall;
    private string generatedPromptFloor;

    // State untuk memulai UI setelah LLM siap
    private bool isReadyToStart = false;

    private void Awake()
    {
       //if (Instance != null && Instance != this)
        //{
            //Destroy(gameObject);
            //return;
        //}
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameUI.SetActive(true);
    }

    /*private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject); // Hapus instance lama
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
    StartTextureGenerationUI();
}
*/

    private void Start()
    {
        //ResetUIReferences();
        // Null checks
        if (wallInputField == null) Debug.LogError("wallInputField is NULL!");
        if (floorInputField == null) Debug.LogError("floorInputField is NULL!");
        if (okButton1 == null) Debug.LogError("okButton1 is NULL!");
        if (okButton2 == null) Debug.LogError("okButton2 is NULL!");
        if (promptWallText == null) Debug.LogError("promptWallText is NULL!");
        if (promptFloorText == null) Debug.LogError("promptFloorText is NULL!");
        if (loadingText == null) Debug.LogError("loadingText is NULL!");
        if (gameUI == null) Debug.LogError("gameUI is NULL!");
        if (fullscreenPanel == null) Debug.LogError("fullscreenPanel is NULL!");
        if (statusText == null) Debug.LogError("statusText is NULL!");
        if (llmCharacter == null) Debug.LogError("llmCharacter is NULL!");

        fullscreenCanvasGroup = fullscreenPanel.GetComponent<CanvasGroup>();
        if (fullscreenCanvasGroup == null)
            Debug.LogError("CanvasGroup is missing from fullscreenPanel!");

        // Initial UI state
        //gameUI.SetActive(false);
        okButton2.gameObject.SetActive(false);
        okButton2.interactable = false;
        loadingText.gameObject.SetActive(false);
        promptWallText.text = "";
        promptFloorText.text = "";

        // Set button listeners
        okButton1.onClick.AddListener(OnFirstOkPressed);
        okButton2.onClick.AddListener(OnSecondOkPressed);

            // Panggil coroutine overlay hanya kalau game sedang texture generation
        if (GameManager.Instance.gameState == GameState.textureGeneration)
        {
            StartCoroutine(StartOverlaySequence());
        }
        else
        {
            Debug.Log("Starting game at Level 1 without texture generation.");
            // Bisa aktifkan UI game normal di sini jika perlu
            gameUI.SetActive(false); // Aktifkan UI untuk gameplay normal
        }
    }

    private IEnumerator StartOverlaySequence()
    {
        gameUI.SetActive(true);
        fullscreenPanel.SetActive(true);
        fullscreenCanvasGroup.alpha = 1f;

        statusText.text = "UI is initializing...";
        yield return new WaitForSecondsRealtime(2f);

        statusText.text = "UI initialized...";
        yield return new WaitForSecondsRealtime(1f);

        statusText.text = "LLM is baking...";
        yield return new WaitForSecondsRealtime(1f);

        statusText.text = "LLM is baking...";
        yield return new WaitUntil(() => LLM.isLLMReady);

        isReadyToStart = true;
        statusText.text = "<b>LLM is baked</b>\n<color=#CCCCCC>Click spacebar to start...</color>";

        // Pastikan game paused
        GameManager.Instance.gameState = GameState.textureGeneration;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (isReadyToStart && Input.GetKeyDown(KeyCode.Space))
        {
            isReadyToStart = false;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            fullscreenCanvasGroup.alpha = 1f - (elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        fullscreenCanvasGroup.alpha = 0f;
        fullscreenPanel.SetActive(false);
    }

    // =================== BUTTON HANDLERS ===================

    // Tombol OK1: generate prompt wall dan floor secara bergantian
    private void OnFirstOkPressed()
    {
        if (string.IsNullOrEmpty(wallInputField.text) || string.IsNullOrEmpty(floorInputField.text))
        {
            Debug.LogWarning("Wall or Floor input is empty!");
            return;
        }

        // Disable input dan tombol agar user tidak ubah selama proses
        wallInputField.interactable = false;
        floorInputField.interactable = false;
        okButton1.interactable = false;

        // Bersihkan prompt sebelumnya
        promptWallText.text = "";
        promptFloorText.text = "";

        // Jalankan coroutine generate prompt bergantian
        StartCoroutine(GeneratePromptsSequence());
    }

    private IEnumerator GeneratePromptsSequence()
    {
        // Generate prompt Wall (tambahkan "wall" secara eksplisit)
        string wallTheme = $"{NormalizeInput(wallInputField.text)} wall"; // <-- tambahkan "wall"
        bool wallDone = false;
        llmCharacter.Chat(wallTheme, (responseWall) => // <-- gunakan wallTheme yang sudah ada kata "wall"
        {
            if (string.IsNullOrWhiteSpace(responseWall))
            {
                Debug.LogError("Empty LLM response for Wall!");
                wallDone = true;
                return;
            }

            generatedPromptWall = responseWall;
            promptWallText.text = responseWall;
            wallDone = true;
            Debug.Log($"[Wall AI Placement] Generated prompt: {responseWall}");
        });

        yield return new WaitUntil(() => wallDone);

        // Generate prompt Floor (tambahkan "floor" secara eksplisit)
        string floorTheme = $"{NormalizeInput(floorInputField.text)} floor"; // <-- tambahkan "floor"
        bool floorDone = false;
        llmCharacter.Chat(floorTheme, (responseFloor) => // <-- gunakan floorTheme yang sudah ada kata "floor"
        {
            if (string.IsNullOrWhiteSpace(responseFloor))
            {
                Debug.LogError("Empty LLM response for Floor!");
                floorDone = true;
                return;
            }

            generatedPromptFloor = responseFloor;
            promptFloorText.text = responseFloor;
            floorDone = true;
            Debug.Log($"[Floor AI Placement] Generated prompt: {responseFloor}");
        });
        yield return new WaitUntil(() => floorDone);
        
        okButton2.gameObject.SetActive(true);
        okButton2.interactable = true;
    }

    // Tombol OK2: panggil API untuk generate tekstur berdasarkan prompt wall & floor
    private void OnSecondOkPressed()
    {
        StartCoroutine(ProcessTextureRequest());
    }

    private IEnumerator ProcessTextureRequest()
    {
        TextureManager textureManager = FindFirstObjectByType<TextureManager>();
        if (textureManager == null)
        {
            Debug.LogError("TextureManager missing!");
            loadingText.gameObject.SetActive(false);
            yield break;
        }

        loadingText.gameObject.SetActive(true);

        // Kirim request generate tekstur wall
        yield return textureManager.SetPromptAndFetchTexture(
            NormalizeInput(wallInputField.text),
            generatedPromptWall,
            TextureType.Wall
        );

        // Kirim request generate tekstur floor
        yield return textureManager.SetPromptAndFetchTexture(
            NormalizeInput(floorInputField.text),
            generatedPromptFloor,
            TextureType.Floor
        );

        // SETELAH TEXTURE SIAP:
        GameManager.Instance.StartGameAfterTextureGeneration();

        loadingText.gameObject.SetActive(false);
        gameUI.SetActive(false);
    }

        public void StartTextureGenerationUI()
    {
        // Debug awal pemanggilan
        Debug.Log("StartTextureGenerationUI called");

        // Cek terlebih dahulu semua referensi penting
        if (wallInputField == null)
        {
            Debug.LogError("StartTextureGenerationUI: wallInputField is null");
            return;
        }

        if (floorInputField == null)
        {
            Debug.LogError("StartTextureGenerationUI: floorInputField is null");
            return;
        }

        if (promptWallText == null)
        {
            Debug.LogError("StartTextureGenerationUI: promptWallText is null");
            return;
        }

        if (promptFloorText == null)
        {
            Debug.LogError("StartTextureGenerationUI: promptFloorText is null");
            return;
        }

        if (gameUI == null)
        {
            Debug.LogError("StartTextureGenerationUI: gameUI is null");
            return;
        }

        if (fullscreenPanel == null)
        {
            Debug.LogError("StartTextureGenerationUI: fullscreenPanel is null");
            return;
        }

        if (fullscreenCanvasGroup == null)
        {
            Debug.LogError("StartTextureGenerationUI: fullscreenCanvasGroup is null");
            return;
        }

        if (okButton1 == null)
        {
            Debug.LogError("StartTextureGenerationUI: okButton1 is null");
            return;
        }

        if (okButton2 == null)
        {
            Debug.LogError("StartTextureGenerationUI: okButton2 is null");
            return;
        }

        // Inisialisasi UI setelah semua valid
        promptWallText.text = "";
        promptFloorText.text = "";
        wallInputField.text = "";
        floorInputField.text = "";
        wallInputField.interactable = true;
        floorInputField.interactable = true;

        okButton1.interactable = true;
        okButton2.gameObject.SetActive(false);
        okButton2.interactable = false;

        gameUI.SetActive(true);
        fullscreenPanel.SetActive(true);
        fullscreenCanvasGroup.alpha = 1f;

        Debug.Log("UI initialization completed.");

        StartCoroutine(StartOverlaySequence());
    }

     /*public void RefreshUIReferences()
    {
        // Example refresh logic; adjust according to your exact UI setup
        if (gameUI == null)
            gameUI = GameObject.Find("Canvas");

        if (fullscreenPanel == null)
            fullscreenPanel = GameObject.Find("FullscreenPanel");

        if (fullscreenCanvasGroup == null && fullscreenPanel != null)
            fullscreenCanvasGroup = fullscreenPanel.GetComponent<CanvasGroup>();

        if (wallInputField == null)
            wallInputField = GameObject.Find("WallInputField")?.GetComponent<InputField>();

        if (floorInputField == null)
            floorInputField = GameObject.Find("FloorInputField")?.GetComponent<InputField>();

        if (promptWallText == null)
            promptWallText = GameObject.Find("PromptWallText")?.GetComponent<Text>();

        if (promptFloorText == null)
            promptFloorText = GameObject.Find("PromptFloorText")?.GetComponent<Text>();

        if (statusText == null)
            statusText = GameObject.Find("StatusText")?.GetComponent<Text>();

        if (okButton1 == null)
            okButton1 = GameObject.Find("OkButton1")?.GetComponent<Button>();

        if (okButton2 == null)
            okButton2 = GameObject.Find("OkButton2")?.GetComponent<Button>();
    }
    */

    // =================== HELPER ===================

    private string NormalizeInput(string input)
    {
        string[] words = input.ToLower()
            .Split(new char[] { ',', ' ', '|', '-' }, System.StringSplitOptions.RemoveEmptyEntries);

        System.Array.Sort(words);
        return string.Join(" ", words).Trim();
    }

    // =================== EXIT ===================

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
