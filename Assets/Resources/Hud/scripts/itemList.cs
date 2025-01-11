using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{

    public static List<GameObject> itemList = new List<GameObject>();

    public static ItemList instance = null;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject); // Solo puede haber uno >:)
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PrepareItemList(){
        int lineCounter = 0;
        int line = 0;
        int totalLines = itemList.Count / 5 + 1;
        foreach(GameObject item in itemList){
            item.transform.SetParent(this.transform);
            item.transform.GetComponent<RectTransform>().localPosition = new Vector3(lineCounter*200 + 100,-line*200 - 100,0);
            lineCounter++;
            if(lineCounter >= 5){
                lineCounter=0;
                line++;
            }
        }
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0,Mathf.Clamp(totalLines*200,200,1000000));
    }
}
