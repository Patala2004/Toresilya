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

    // Start is called before the first frame update
    void Start()
    {
        healthOrigOffset = healthBar.GetComponent<RectTransform>().sizeDelta.x;
        resistanceOrigOffset = resistanceBar.GetComponent<RectTransform>().sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthOrigOffset*((float)player.health/player.healthMax), healthBar.GetComponent<RectTransform>().sizeDelta.y);
        resistanceBar.GetComponent<RectTransform>().sizeDelta = new Vector2(resistanceOrigOffset * ((float)player.resistance / player.resistanceMax), resistanceBar.GetComponent<RectTransform>().sizeDelta.y);
        contadorMonedas.text = player.monedas.ToString();
    }
}
