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

    public void ShowItemText(string itemName, string itemDescription)
    {
         itemText.text = itemName;
         itemProyectado = true;
         timer = 0f; // Reinicia el temporizador
    }

}
