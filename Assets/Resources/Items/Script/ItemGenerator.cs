using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public GameObject[] items;
    //public GameObject[] itemspocoComunes;
    //public GameObject[] itemsEpicos;
    //public GameObject[] itemsLegendarios;
    //public GameObject[] itemspocoComunDebil;
    //public GameObject[] itemsEpicoDebil;
    //public GameObject[] consumables;
    //public GameObject[] itemsRayoEpico;
    //public GameObject[] itemsRayoRaro;

    public List<GameObject> itemspocoComunes;
    public List<GameObject> itemsEpicos;
    public List<GameObject> itemsLegendarios;
    public List<GameObject> itemspocoComunDebil;
    public List<GameObject> itemsEpicoDebil;
    public List<GameObject> consumables;
    public List<GameObject> itemsRayoEpico;
    public List<GameObject> itemsRayoRaro;

    public static ItemGenerator instance = null;
    public static bool hasUsedDebil= false;
    public static bool hasUsedRayos = false;
    // Start is called before the first frame update
    void Start()
    {
        items = Resources.LoadAll<GameObject>("Items/Prefabs");
        itemspocoComunes = Resources.LoadAll<GameObject>("Items/Prefabs/Items_pocoComun").ToList();
        itemsEpicos = Resources.LoadAll<GameObject>("Items/Prefabs/Items_epico").ToList();
        itemsLegendarios = Resources.LoadAll<GameObject>("Items/Prefabs/Items_legendarios").ToList();
        itemsEpicoDebil = Resources.LoadAll<GameObject>("Items/Prefabs/Items_epico_DEBIL").ToList();
        itemspocoComunDebil = Resources.LoadAll<GameObject>("Items/Prefabs/Items_pocoComunRaro_DEBIL").ToList();
        consumables = Resources.LoadAll<GameObject>("Items/Consumibles/Prefabs").ToList();
        itemsRayoEpico = Resources.LoadAll<GameObject>("Items/Prefabs/Items_Rayo_Epico").ToList();
        itemsRayoRaro = Resources.LoadAll<GameObject>("Items/Prefabas/Items_Rayo_raro").ToList();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getItem(){
        int rand = Random.Range(0, 100);
        if(rand>=0 && rand < 80)
        {
            return Instantiate(itemspocoComunes[UnityEngine.Random.Range(0, itemspocoComunes.Count)]);
        }
        else if (rand >= 80 && rand < 95)
        {
            return Instantiate(itemsEpicos[UnityEngine.Random.Range(0, itemsEpicos.Count)]);
        }
        else
        {
            return Instantiate(itemsLegendarios[UnityEngine.Random.Range(0, itemsLegendarios.Count)]);
        }
        
    }

    public GameObject getItemForChests()
    {
        int rand = Random.Range(0, 100);
        if (rand >= 0 && rand < 70)
        {
            return Instantiate(itemspocoComunes[UnityEngine.Random.Range(0, itemspocoComunes.Count)]);
        }
        else if (rand >= 70 && rand < 90)
        {
            return Instantiate(itemsEpicos[UnityEngine.Random.Range(0, itemsEpicos.Count)]);
        }
        else
        {
            return Instantiate(itemsLegendarios[UnityEngine.Random.Range(0, itemsLegendarios.Count)]);
        }

    }

    public GameObject getConsumable(){
        return Instantiate(consumables[UnityEngine.Random.Range(0,consumables.Count)]);
    }

    public static void activarDebil()
    {
        if (hasUsedDebil)
        {
            return;
        }
        Debug.Log("AHHHHHHHHHHHHHHHH SEXO");
        instance.itemsEpicos.AddRange(instance.itemsEpicoDebil);
        instance.itemspocoComunes.AddRange(instance.itemspocoComunDebil);
        hasUsedDebil = true;
    }
    public void activarRayos()
    {
        if (hasUsedRayos)
        {
            return;
        }
        hasUsedRayos = true;
        anadirRayos();
    }
    public static void anadirRayos()
    {
        if (hasUsedRayos)
        {
            return;
        }
        instance.itemsEpicos.AddRange(instance.itemsRayoEpico);
        instance.itemspocoComunes.AddRange(instance.itemsRayoRaro);
        hasUsedRayos = true;
    }


}
