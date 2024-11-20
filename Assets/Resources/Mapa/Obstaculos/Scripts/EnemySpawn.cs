using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public bool randomized = true; 
    public GameObject[] enemies;
    Room room;
    int rng = 0;
    bool crearEnemigos = false;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
        {
            if (transform.parent.gameObject.transform.parent != null)
            { 
                room = transform.parent.gameObject.transform.parent.gameObject.GetComponent<Room>();
                rng = randomized ? (int)Random.Range(0, enemies.Length) : 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (room != null) if (room.spawned && !crearEnemigos) actualizarRoom(Instantiate(enemies[rng]));
    }
    private void actualizarRoom(GameObject newEnemy)
    {
        if (newEnemy != null)
        {
            room.aliveEnemies++;
            newEnemy.transform.position = this.transform.position;
            newEnemy.transform.parent = transform.parent.gameObject.transform.parent.transform;
            newEnemy.GetComponent<Enemy>().room = room.GetComponent<Room>();
            crearEnemigos = true;
        }
    }
}
