using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Golpeable
{

    [SerializeField] private GameObject BoxSprite;
    [SerializeField] private GameObject itemHolder;

    private BoxCollider2D col;
    bool hasBeenOpened = false;

    private ItemGenerator itemGen;
    // Start is called before the first frame update
    void Start()
    {
        itemGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(float damage)
    {
        OpenChest();
    }

    public void OpenChest(){
        
        GameObject newItem = itemGen.getItem();
        newItem.transform.SetParent(this.transform, false);
        newItem.transform.localPosition = new Vector2(0,0);
        Destroy(BoxSprite);
        hasBeenOpened = true;
        itemHolder.SetActive(true);
        itemHolder.GetComponent<ChestTile>().SetText(newItem.GetComponent<Item>().descriptionItem);
        itemHolder.GetComponent<ChestTile>().item = newItem.GetComponent<Item>();
        col.enabled = false;

        this.enabled = false;
        
    }
}
