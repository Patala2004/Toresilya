using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTransparente : MonoBehaviour
{
    Image image;

    public GameObject botonMenu;

    public GameObject botonReinicio;

    public GameObject textoMuerte;

    float alpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(alpha > 1) // se ejecuta al final
        {
            botonReinicio.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            botonMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -266, 0);
            gameObject.transform.position = new(0, 1000, 0);
            textoMuerte.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 350, 0);
        }
        else
        {
            alpha += Time.deltaTime / 2;
            image.color = new Color(0, 0, 0, alpha);
        }
    }
}
