using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PokeCenter : MonoBehaviour {
	public static void HealPokemon() {
		foreach (var pokemon in Pokemon.party) {
			//pokemon.hp = pokemon.health;
			pokemon.hp = 1;
		}
	}
}