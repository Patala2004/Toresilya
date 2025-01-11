using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Barra : MonoBehaviour
{
    public Player player;
    public GameObject resistanceBar;
    public GameObject healthBar;
    public TextMeshProUGUI contadorMonedas;
	private float healthOrigOffset;
    private float resistanceOrigOffset;

    //Para los items
    public TextMeshProUGUI itemText;
    private float duracionMax = 3f; // Tiempo en segundos que el texto se mostrará
    private float timer = 0f;    // Temporizador para controlar el tiempo del mensaje
    private bool itemProyectado = false;

    // Start is called before the first frame update
    void Start()
    {
        healthOrigOffset = healthBar.GetComponent<RectTransform>().sizeDelta.x;
        resistanceOrigOffset = resistanceBar.GetComponent<RectTransform>().sizeDelta.x;
        itemText.text = "";

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthOrigOffset*((float)player.health/player.healthMax), healthBar.GetComponent<RectTransform>().sizeDelta.y);
        resistanceBar.GetComponent<RectTransform>().sizeDelta = new Vector2(resistanceOrigOffset * ((float)player.resistance / player.resistanceMax), resistanceBar.GetComponent<RectTransform>().sizeDelta.y);
        contadorMonedas.text = player.monedas.ToString();

        if (itemProyectado)
        {
            timer += Time.deltaTime;
            if (timer > duracionMax)
            {
                itemText.text = "";  // Reset del texto
                timer = 0f;
                itemProyectado = false;
            }
        }
    }

    public void ShowItemText(string itemName, string rareza)
    {
         itemText.text = itemName;
         itemText.color = colorSegunRareza(rareza);
         
         itemProyectado = true;
         timer = 0f; // Reinicia el temporizador
    }

    public Color32 colorSegunRareza(string Rareza)
    {
        switch (Rareza)
        {
            case "comun": return new Color32(165, 165, 165, 255); //Gris oscuro -> comun
            case "pocoComun": return new Color32(0, 148, 9, 255); //verde oscuro -> pocoComun
            case "raro": return new Color32(15, 178, 227, 255); //azul -> raro
            case "epico": return new Color32(142, 32, 239, 255); //morado -> epico
            case "legendario": return new Color32(250, 250, 63, 255); //amarillo -> lengendario
        }
        return new Color32(100, 100, 100, 255); //El mismo que el del precio, imposible que llegue
    }

}
