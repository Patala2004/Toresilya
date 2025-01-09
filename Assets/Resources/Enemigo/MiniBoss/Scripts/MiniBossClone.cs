using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossClone : MonoBehaviour
{
    // Start is called before the first frame update
    float x = 1;
    void Start()
    {
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        x -= Time.deltaTime * 5;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, x);
    }
}
