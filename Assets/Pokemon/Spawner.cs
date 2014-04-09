using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
	public float radius;
	public GameObject[] pokemon;
	public int level;
	public int number;

	public List<GameObject> spawned = new List<GameObject>();

	void Update(){
		if (spawned.Count<number){
			GameObject prefab = (GameObject)pokemon[Random.Range(0,pokemon.Length)];
			GameObject newPokemon = (GameObject)Instantiate(prefab);
			newPokemon.transform.position = transform.position+Vector3.up*100
				+Quaternion.Euler(0,Random.value*360,0)*Vector3.right *radius * Mathf.Sqrt(Random.value);
			RaycastHit hit;
			if (Physics.Raycast(newPokemon.transform.position, Vector3.down, out hit)){
				newPokemon.transform.position = hit.point;
			}
			newPokemon.transform.rotation = Quaternion.Euler(0,Random.value*360,0);
			newPokemon.AddComponent<PokemonWild>();
			newPokemon.name = prefab.name;
			int thisLevel = level + Random.Range(-2,3);
			newPokemon.GetComponent<PokemonObj>().pokemon = new Pokemon(Pokemon.GetNumber(newPokemon.name), false, thisLevel);
			spawned.Add(newPokemon);
			newPokemon.transform.parent = transform;
		}
	}
}