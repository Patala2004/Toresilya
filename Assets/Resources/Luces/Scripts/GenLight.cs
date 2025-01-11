using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GenLight : MonoBehaviour
{
    public GameObject luz;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GenerateLight(Vector2 pos)
    {
        Instantiate(luz, pos, Quaternion.identity);
    }
}
