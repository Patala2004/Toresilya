using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject[] itemSlots;
    [SerializeField] private GameObject[] consumableSlots;

    private ItemGenerator itemGen;
    // Start is called before the first frame update
    void Start()
    {
        itemGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();

        Player player = GameObject.Find("player").GetComponent<Player>();

        foreach(GameObject item_slot in itemSlots){
            // Get new Item
            StoreTile slotScript = item_slot.GetComponent<StoreTile>();
            slotScript.player = player;
            slotScript.SetItem(itemGen.getItem());
        }

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
