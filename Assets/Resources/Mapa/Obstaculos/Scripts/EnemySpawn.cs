using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public bool randomized = true; 
    public GameObject[] enemies;
    int rng = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
        {
            if (transform.parent.gameObject.transform.parent != null)
            {
                GameObject room = transform.parent.gameObject.transform.parent.gameObject;
                Debug.Log("El enemigo pertenece a la sala: " + room.name);
                room.GetComponent<Room>().aliveEnemies++;
                rng = randomized ? (int)Random.Range(0, enemies.Length) : 0;
                GameObject newEnemy = Instantiate(enemies[rng]);
                newEnemy.transform.position = this.transform.position;
                newEnemy.transform.parent = transform.parent.gameObject.transform.parent.transform;
                newEnemy.GetComponent<Enemy>().room = room.GetComponent<Room>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
