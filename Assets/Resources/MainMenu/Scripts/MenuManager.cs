using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private AsyncOperation preloadOperation;

    // Start is called before the first frame update
    void Start()
    {
        PreloadScene(); // Llamar para que se empieze a cargar el juego
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PreloadScene(){
        if (preloadOperation == null)
        {
            preloadOperation = SceneManager.LoadSceneAsync("SampleScene");
            preloadOperation.allowSceneActivation = false; // Don't activate immediately
        }
    }

        // Load the preloaded scene
    public void LoadPreloadedScene()
    {
        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = true; // Activate the scene

        }
        else
        {
            Debug.LogWarning("No scene has been preloaded!");
        }
    }


    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
