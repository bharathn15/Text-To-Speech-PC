using System.Collections;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TextToSpeechConverter : MonoBehaviour
{
    [SerializeField] RectTransform canvas;
    [SerializeField] TMP_InputField textInput;
    [SerializeField] RectTransform textInputRectTransform;
    [SerializeField] Button textToSpeechButton;
    [SerializeField] float voiceOutPutDelay = 0.5f;

    private string pythonExePath;
    private string textToSpeechPythonScript;

    void Start()
    {
        ResizeUi(); // Resize UI elements

        // Set paths for Python executable and script
        pythonExePath = Application.streamingAssetsPath + "/python/text2speech_env/Scripts/python.exe";
        
        textToSpeechPythonScript = Application.streamingAssetsPath + "/python/text2speech_env/main.py";
        //textToSpeechPythonScript = Path.Combine(Application.persistentDataPath, "text2speech_env", "main.py");

        Debug.Log($"Python exe path: {pythonExePath}");
        Debug.Log($"Python script path: {textToSpeechPythonScript}");

        Debug.Log($"Python exe file exists {File.Exists(pythonExePath)}");
        Debug.Log($"Python script exists {File.Exists(textToSpeechPythonScript)}");

        // Start the process to copy files from StreamingAssets to persistentDataPath
        // StartCoroutine(CopyFilesToPersistentDataPath());

        // Add button listener
        textToSpeechButton.onClick.AddListener(TextToSpeechOnButtonClick);
    }

    private IEnumerator CopyFilesToPersistentDataPath()
    {
        // string sourcePath = Path.Combine(Application.streamingAssetsPath, "text2speech_env");
        string sourcePath = (Application.streamingAssetsPath+ "/python/text2speech_env");
        string destinationPath = (Application.persistentDataPath + "/python/text2speech_env");

        // Create destination directory if it doesn't exist
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        // Copy Python executable
        /*string sourceExePath = Path.Combine(sourcePath, "python.exe");
        string destinationExePath = Path.Combine(destinationPath, "python.exe");*/
        
        string sourceExePath = (sourcePath + "/python/text2speech_env/Scripts/python.exe");
        string destinationExePath = (destinationPath + "/python/text2speech_env/Scripts/python.exe");
        yield return StartCoroutine(CopyFile(sourceExePath, destinationExePath));

        // Copy Python script
        /*string sourceScriptPath = Path.Combine(sourcePath, "main.py");
        string destinationScriptPath = Path.Combine(destinationPath, "main.py");*/
        
        string sourceScriptPath = (sourcePath + "/python/text2speech_env/main.py");
        string destinationScriptPath = (sourcePath + "/python/text2speech_env/main.py");
        yield return StartCoroutine(CopyFile(sourceScriptPath, destinationScriptPath));
    }

    private IEnumerator CopyFile(string sourcePath, string destinationPath)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(sourcePath))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error copying file: {www.error}");
            }
            else
            {
                File.WriteAllBytes(destinationPath, www.downloadHandler.data);
                Debug.Log($"Copied file to: {destinationPath}");
            }
        }
    }

    public void TextToSpeechOnButtonClick()
    {
        if (!File.Exists(pythonExePath) || !File.Exists(textToSpeechPythonScript))
        {
            Debug.Log($"<color=red>Failed to load the Python scripts.......!</color>");
        }
        else
        {
            StartCoroutine(PythonCoroutine());
        }
    }

    private IEnumerator PythonCoroutine()
    {
        yield return new WaitForSeconds(voiceOutPutDelay);
        RunPythonScript(textInput.text);
    }

    void RunPythonScript(string textInput)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = pythonExePath,
            Arguments = string.Format("\"{0}\" \"{1}\"", textToSpeechPythonScript, textInput),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(processStartInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Debug.Log(result);
            }

            using (StreamReader reader = process.StandardError)
            {
                string error = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError(error);
                }
            }
        }

        Debug.Log($"<color=green>{nameof(RunPythonScript)} executed successfully.....</color>");
    }

    void ResizeUi()
    {
        textInputRectTransform.sizeDelta = new Vector2(canvas.sizeDelta.x - 40f, 400);
        Debug.Log($"{nameof(ResizeUi)} is invoked...");
    }
}
