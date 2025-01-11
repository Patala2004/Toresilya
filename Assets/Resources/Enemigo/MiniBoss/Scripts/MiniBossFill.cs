using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniBossFill : MonoBehaviour
{
    public MiniBoss miniBoss;
    Image Im;
    // Start is called before the first frame update
    void Start()
    {
        Im = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Im.fillAmount = miniBoss.health / miniBoss.maxHealth;
    }
}
