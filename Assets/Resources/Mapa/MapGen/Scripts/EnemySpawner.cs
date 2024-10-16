using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnEnemy(int x, int y){
        GameObject newEnemy = Instantiate(enemies[0]);
        newEnemy.transform.position = new (x,y,0);
        return newEnemy;
    }
}
