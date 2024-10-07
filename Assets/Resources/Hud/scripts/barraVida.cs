using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barraVida : MonoBehaviour
{
    // -883 -578
    public jugador jugador;
    RectTransform rT;
    public float sizeVar = 1;
    // Start is called before the first frame update
    void Start()
    {
        rT = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        sizeVar = (float)jugador.health/jugador.maxHealth;
        rT.localScale = new Vector2(sizeVar,rT.localScale.y);
        rT.localPosition = new Vector2(-883 + 305 * sizeVar,rT.localPosition.y);
    }
}
