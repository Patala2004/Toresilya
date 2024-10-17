using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara : MonoBehaviour
{
    public Player player;
    public Enemy enemigo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            Instantiate(enemigo.gameObject, new Vector2(player.transform.position.x +5, player.transform.position.y),Quaternion.identity);
        }
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y,-20);
    }
}
