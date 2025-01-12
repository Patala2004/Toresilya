using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    bool options = false;

    public GameObject menuPrincipal;
    public GameObject menuOpciones;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreloadScene(){
        SceneManager.LoadScene("SampleScene");
    }

        // Load the preloaded scene
    public void Opciones()
    {
        if (!options)
        {
            menuOpciones.SetActive(true);
            menuPrincipal.SetActive(false);
            options = true;
        }
        else
        {
            menuOpciones.SetActive(false);
            menuPrincipal.SetActive(true);
            options = false;
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
