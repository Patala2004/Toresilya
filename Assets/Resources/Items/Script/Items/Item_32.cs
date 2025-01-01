using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_32 :Item
{
    //Declaramos stats o cosas que modificara el item
    public float cooldown = 60f;
    public bool enCooldown = false;
    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "456";
        descripcionRecoger = "";
        nombre = "";
        unique = false;
        rarity = "";
        precio = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        p.stunMechanics.Add(eliminateStun);

    }

    public void eliminateStun()
    {
        if (player.stuned && !enCooldown)
        {
            player.stuned = false;
            player.stunedTime = 0;
            enCooldown = true;
            StartCoroutine(putOnCooldown(cooldown));
        }
    }


    public virtual IEnumerator putOnCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enCooldown = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto con el que colisionamos es el jugador
        if (other.gameObject.CompareTag("player"))
        {
            grabItem(player);
            Debug.Log("TUS MUERTOS");
            transform.position = new Vector3(10000, 100000, transform.position.z);// Destruir el objeto después de recogerlo
        }
    }

}
