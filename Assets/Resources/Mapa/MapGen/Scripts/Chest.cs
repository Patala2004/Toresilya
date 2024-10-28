using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    [SerializeField] private GameObject BoxSprite;

    private ItemGenerator itemGen;
    // Start is called before the first frame update
    void Start()
    {
        itemGen = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(){
        GameObject newItem = itemGen.getItem();
        newItem.transform.SetParent(this.transform, false);
        newItem.transform.localPosition = new Vector2(0,0);
        Destroy(BoxSprite);
    }
}
