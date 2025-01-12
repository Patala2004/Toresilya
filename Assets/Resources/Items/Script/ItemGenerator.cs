using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public GameObject[] items;
    public GameObject[] itemspocoComunes;
    public GameObject[] itemsEpicos;
    public GameObject[] itemsLegendarios;
    public GameObject[] consumables;

    // Start is called before the first frame update
    void Start()
    {
        items = Resources.LoadAll<GameObject>("Items/Prefabs");
        itemspocoComunes = Resources.LoadAll<GameObject>("Items/Prefabs/Items_pocoComun");
        itemsEpicos = Resources.LoadAll<GameObject>("Items/Prefabs/Items_epico");
        itemsLegendarios = Resources.LoadAll<GameObject>("Items/Prefabs/Items_legendarios");
        consumables = Resources.LoadAll<GameObject>("Items/Consumibles/Prefabs");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getItem(){
        int rand = Random.Range(0, 100);
        if(rand>=0 && rand < 80)
        {
            return Instantiate(itemspocoComunes[UnityEngine.Random.Range(0, itemspocoComunes.Length)]);
        }
        else if (rand >= 80 && rand < 95)
        {
            return Instantiate(itemsEpicos[UnityEngine.Random.Range(0, itemsEpicos.Length)]);
        }
        else
        {
            return Instantiate(itemsLegendarios[UnityEngine.Random.Range(0, itemsLegendarios.Length)]);
        }
        
    }

    public GameObject getItemForChests()
    {
        int rand = Random.Range(0, 100);
        if (rand >= 0 && rand < 70)
        {
            return Instantiate(itemspocoComunes[UnityEngine.Random.Range(0, itemspocoComunes.Length)]);
        }
        else if (rand >= 70 && rand < 90)
        {
            return Instantiate(itemsEpicos[UnityEngine.Random.Range(0, itemsEpicos.Length)]);
        }
        else
        {
            return Instantiate(itemsLegendarios[UnityEngine.Random.Range(0, itemsLegendarios.Length)]);
        }

    }

    public GameObject getConsumable(){
        return Instantiate(consumables[UnityEngine.Random.Range(0,consumables.Length)]);
    }
}
