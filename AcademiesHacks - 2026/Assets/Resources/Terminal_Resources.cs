using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Terminal_Resources : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject terminalCanvas; 
    public GameObject loadingCanvas;   

    [Header("UI Elements")]
    public TextMeshProUGUI terminalText; 

    [Header("Settings")]
    public float typeSpeed = 0.04f;
    public string gameSceneName = "The King"; 
    
    [TextArea(10, 15)]
    public string storyText = ": WINDOWS POWERSHELL [VERSION 10.0.19045.3803]\n(C) MICROSOFT CORPORATION. ALL RIGHTS RESERVED.\n\n> LOADING CORE_DRIVE... DONE.\n> 03:15 AM: CONTAINMENT UNIT \"GOLEM\" BREACHED.\n> WARNING: SYNTHETIC MINERAL ENTITY IS HOSTILE.\n\n> INITIALIZING COMBAT PROTOCOLS...\n> CRITICAL ERROR: SUBJECT ESCAPED.";

    void Start()
    {
        terminalText.text = "";
        
       
        if (loadingCanvas != null)
            loadingCanvas.SetActive(false); 

        StartCoroutine(RunCutscene());
    }

    IEnumerator RunCutscene()
    {
        yield return new WaitForSeconds(1f);
        foreach (char letter in storyText.ToCharArray())
        {
            terminalText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

     
        yield return new WaitForSeconds(3f);

       
        if (terminalCanvas != null)
            terminalCanvas.SetActive(false); 

        yield return new WaitForSeconds(4f); 

       
        if (loadingCanvas != null)
            loadingCanvas.SetActive(true); 

        yield return new WaitForSeconds(2f); 

        
        SceneManager.LoadScene(gameSceneName); 
    }
}