using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject[] itemSlots;
    [SerializeField] private GameObject[] consumableSlots;

    private ItemGenerator itemGen;

    List<string> savedItems = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        itemGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();

        Player player = GameObject.Find("player").GetComponent<Player>();

        foreach(GameObject item_slot in itemSlots){
            // Get new Item
            StoreTile slotScript = item_slot.GetComponent<StoreTile>();
            slotScript.player = player;
            GameObject newItem = itemGen.getItem();
            int try_limit = 75;
            while(savedItems.Contains(newItem.GetComponent<Item>().name) && try_limit>0){
                newItem = itemGen.getItem();
                try_limit--;
            }
            slotScript.SetItem(newItem);
            savedItems.Add(newItem.GetComponent<Item>().name);
        }
        savedItems.Clear();

        foreach(GameObject cons_slot in consumableSlots){
            // Get new Item
            StoreTile slotScript = cons_slot.GetComponent<StoreTile>();
            slotScript.player = player;
            slotScript.SetItem(itemGen.getConsumable());
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
