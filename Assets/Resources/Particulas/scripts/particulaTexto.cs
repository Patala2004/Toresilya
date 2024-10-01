using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class particulaTexto : MonoBehaviour
{
    TextMeshPro mProTexto;
    public Vector3 vel;
    public int damage;
    public float dismissTime;
    // Start is called before the first frame update
    void Start()
    {
        mProTexto = GetComponent<TextMeshPro>();
        mProTexto.text = "-" + damage.ToString();
        Destroy(gameObject, dismissTime);
    }

    // Update is called once per frame
    void Update()
    {
        vel.y -= 9.8f * Time.deltaTime;
        transform.position += vel * Time.deltaTime;

    }
}
