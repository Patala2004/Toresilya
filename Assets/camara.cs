using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara : MonoBehaviour
{
    public Player player;
    public Enemy enemigo;
    GenLight gN;
    // Start is called before the first frame update
    void Start()
    {
        gN = gameObject.AddComponent<GenLight>();
        gN.luz = Resources.Load<GameObject>("Luces/LuzBossSwordEpico");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            Instantiate(enemigo.gameObject, new Vector2(player.transform.position.x +5, player.transform.position.y),Quaternion.identity);
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            Instantiate(Resources.Load<GameObject>("Enemigo/MiniBoss/MiniBoss"), new Vector2(player.transform.position.x + 8, player.transform.position.y), Quaternion.identity);
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            gN.GenerateLight(new Vector2(player.transform.position.x + 8, player.transform.position.y));
        }
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y,-20);
        if (Input.GetKeyUp(KeyCode.O))
        {
            Instantiate(Resources.Load<GameObject>("Proyectiles/arrow"), new Vector2(player.transform.position.x + 5, player.transform.position.y), Quaternion.identity);
        }
    }
}
