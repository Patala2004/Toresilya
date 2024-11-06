using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;

    public bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(!isPaused){
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                isPaused = true;
            }
            else{
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                isPaused = false;
            }            
        }
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void unPause(){
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;
    }
}
