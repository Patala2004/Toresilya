using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opciones : MonoBehaviour
{
    AudioSource aS;
    // Start is called before the first frame update
    void Start()
    {
        aS = GetComponent<AudioSource>();
        aS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Reiniciar()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
