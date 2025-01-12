using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueAutomatico : MonoBehaviour
{
	public Player player;
	bool autoataque = false;
    // Start is called before the first frame update
    void Start()
    {
		
    }

	public void OnValueChanged(bool val) {
		player.autoataque = !player.autoataque;
		Debug.Log("Autoataque vale " + player.autoataque);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
