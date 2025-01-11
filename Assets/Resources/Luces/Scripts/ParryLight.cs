using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParryLight : MonoBehaviour
{
    // Start is called before the first frame update
    Light2D L2D;
    public float waitseconds = 0.7f;
    void Start()
    {
        L2D = GetComponent<Light2D>();
    }
    private void FixedUpdate()
    {
        L2D.intensity -= Time.deltaTime / (waitseconds);
        if (L2D.intensity < 0 ) { Destroy(gameObject); }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
