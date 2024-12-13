using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
	Player p;
    // Start is called before the first frame update
    void Start()
    {
        p = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.tag.Equals("enemy")) {
			p.enrango++;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if(collider.tag.Equals("enemy")) {
			p.enrango--;
		}
	}
}
