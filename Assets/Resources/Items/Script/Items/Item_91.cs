using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Item_91 : Item
{
    //Declaramos stats o cosas que modificara el item
    public bool intentadoItem91 = false;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        p.attackMechanics.Add(metodoItem91Beta);
        p.criticalAttackMechanics.Add(metodoItem91Beta);
        p.parryMechanics.Add(metodoItem91Alpha);
        p.perfectParryMechanics.Add(metodoItem91Alpha);
    }

    public void metodoItem91Alpha(Enemy e)
    {
        if (e.debilTime > 0 && !intentadoItem91 && e.caosTime==0){
            intentadoItem91=true;
            int random = Random.Range(1, 101);
            if(random <= 5)
            {
                e.caosTime = 1;
                e.isCaos = true;
                e.debilTime = 0;
            }
        }
        else
        {
            intentadoItem91 = false;
        }
    }
    public void metodoItem91Beta(Enemy[] eList)
    {
        foreach (Enemy e in eList)
        {
            if (e.debilTime > 0 && !intentadoItem91 && e.caosTime == 0)
            {
                intentadoItem91 = true;
                int random = Random.Range(1, 101);
                if (random <=5)
                {
                    Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                    e.caosTime = 1;
                    e.isCaos = true;    
                    e.debilTime = 0;
                }
            }
            else
            {
                intentadoItem91 = false;
            }
        }
    }
}
