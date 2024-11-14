using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public bool randomized = true; 
    public Enemy[] enemies;
    int rng = 0;
    // Start is called before the first frame update
    void Start()
    {
        rng = randomized ? (int)Random.Range(0,enemies.Length) : 0;
        Enemy newEnemy = Instantiate(enemies[rng]);
        newEnemy.transform.position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
