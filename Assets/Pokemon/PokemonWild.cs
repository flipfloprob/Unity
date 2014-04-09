using UnityEngine;
using System.Collections;

public class PokemonWild : MonoBehaviour {
	Vector3 target = Vector3.zero;
	PokemonObj pokemonObj;

	void Start(){
		target = transform.position;
		pokemonObj = GetComponent<PokemonObj>();
	}

	void Update(){
		if (pokemonObj.enemy==null){
			if (Random.value<0.01f)	target = transform.position + Quaternion.Euler(0,Random.value*360,0)*Vector3.right*10;
			Vector3 direct = target-transform.position;
			direct.y = 0;
			if (direct.sqrMagnitude>1){
				transform.rotation = Quaternion.LookRotation(direct);
				pokemonObj.SetVelocity(direct.normalized * pokemonObj.speed/3);
			}
		}else{
			Vector3 direct = pokemonObj.enemy.transform.position-transform.position;
			if (direct.sqrMagnitude>25*25){
				pokemonObj.enemy = null;
				return;
			}

			direct.y = 0;
			transform.rotation = Quaternion.LookRotation(direct);

			if (Random.value<0.1f)	//use random moves whenever
				pokemonObj.UseMove(direct.normalized, pokemonObj.pokemon.moves[Random.Range(0,pokemonObj.pokemon.moves.Count)]);

			if (direct.sqrMagnitude>1){
				pokemonObj.SetVelocity(direct.normalized*pokemonObj.speed);
			}
		}
	}
}