using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public GameObject[] items;

    // Start is called before the first frame update
    void Start()
    {
        items = Resources.LoadAll<GameObject>("Items/Prefabs");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
