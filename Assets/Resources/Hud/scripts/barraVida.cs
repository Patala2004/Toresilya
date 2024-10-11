using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class barraVida : MonoBehaviour
{
    // -883 -578
    public jugador jugador;
    RectTransform rT;
    public float sizeVar = 1;
	private RawImage img;
	private float origOffset;

    // Start is called before the first frame update
    void Start()
    {
        rT = GetComponent<RectTransform>();
		origOffset = rT.offsetMax.x;
    }

    // Update is called once per frame
    void Update()
    {
        sizeVar = (float)jugador.health/jugador.maxHealth;
        rT.offsetMax = new Vector2(sizeVar*origOffset, rT.offsetMax.y);
    }
}
