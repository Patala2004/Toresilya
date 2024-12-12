using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public GameObject[] items;
    public GameObject[] consumables;

    // Start is called before the first frame update
    void Start()
    {
        items = Resources.LoadAll<GameObject>("Items/Prefabs");
        consumables = Resources.LoadAll<GameObject>("Items/Consumibles/Prefabs");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getItem(){
        return Instantiate(items[UnityEngine.Random.Range(0,items.Length)]);
    }

    public GameObject getConsumable(){
        return Instantiate(consumables[UnityEngine.Random.Range(0,consumables.Length)]);
    }
}
