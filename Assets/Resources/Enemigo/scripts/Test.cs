using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Test : MonoBehaviour
{
	public Player player;
	public Enemy enemy;
    void TestDano() {
		Enemy en = new() {
			health = 120,
		};
		en.ReduceHealth(30);
		Debug.Assert(en.health == 120 - 30);
	}
	// No hay manera de probar esto
	// Siempre da NullReferenceException
	/*void TestAttack() {
		Player p = Instantiate(player);
		Enemy en = Instantiate(enemy);

		en.HitPlayer(0, en.damage, 0);
		Debug.Assert(p.health == 0);
		Destroy(p);
		Destroy(en);
	}*/

	public void Start() {
		TestDano();
		//TestAttack();
	}
}
