using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParticulaTexto : MonoBehaviour
{
    TextMeshPro mProTexto;
    public Vector3 vel;
    public float damage;
    public float dismissTime;
    // Start is called before the first frame update
    void Start()
    {
        mProTexto = GetComponent<TextMeshPro>();
        mProTexto.text = "-" + (Mathf.Round(damage * 10f) / 10f).ToString();
        Destroy(gameObject, dismissTime);
    }

    // Update is called once per frame
    void Update()
    {
        vel.y -= 9.8f * Time.deltaTime;
        transform.position += vel * Time.deltaTime;

    }
}
