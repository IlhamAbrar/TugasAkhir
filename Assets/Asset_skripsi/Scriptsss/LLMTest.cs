/*using UnityEngine;
using LLMUnity; // Keep this if the namespace exists

public class LLMTest : MonoBehaviour
{
    private LLM llm; // Use the correct class name

    void Start()
    {
        llm = FindObjectOfType<LLM>(); // Find the LLM component in the scene

        if (llm == null)
        {
            Debug.LogError("LLM instance not found in the scene.");
            return;
        }

        TestLLM();
    }

    void TestLLM()
    {
        string prompt = "Hello, how are you?";
        string response = llm.GenerateResponse(prompt); // Call LLM to generate a response
        Debug.Log("LLM Response: " + response);
    }
}
*/