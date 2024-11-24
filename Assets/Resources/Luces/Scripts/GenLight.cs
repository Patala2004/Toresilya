using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GenLight : MonoBehaviour
{
    public GameObject luz;
    GameObject clon;
    public float intensity;
    // Start is called before the first frame update
    void Start()
    {
        clon = Instantiate(luz,new(800,800), Quaternion.identity);
        intensity = clon.GetComponent<Light2D>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if(clon.GetComponent<Light2D>().intensity < 0)
        {
            clon.transform.position = new(800,800);
        }
    }
    public void GenerateLight(Vector2 pos)
    {
        clon.transform.position = pos;
        clon.GetComponent<Light2D>().intensity = intensity;
    }
}
