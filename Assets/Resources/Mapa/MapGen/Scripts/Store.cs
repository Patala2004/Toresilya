using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject[] itemSlots;

    private ItemGenerator itemGen;
    // Start is called before the first frame update
    void Start()
    {
        itemGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();

        foreach(GameObject item_slot in itemSlots){
            // Get new Item
            GameObject newItem = itemGen.getItem();
            newItem.transform.SetParent(item_slot.transform, false);
            newItem.transform.localPosition = new Vector2(0,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
