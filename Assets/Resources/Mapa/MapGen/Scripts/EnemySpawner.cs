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

    public void SpawnEnemy(int x, int y){
        GameObject newEnemy = Instantiate(enemies[0]);
        newEnemy.transform.position = new Vector3(x,y,0);
    }
}
