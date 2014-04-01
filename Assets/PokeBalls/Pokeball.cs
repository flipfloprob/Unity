using UnityEngine;
using System.Collections;

public class Pokeball : MonoBehaviour {
	public GameObject trainer = null;
	public Pokemon pokemon = null;
	public bool active = true;
	bool fired = false;

	float lifetime = 1;

	void Update(){
		if (lifetime<2.9f)	collider.enabled = true;

		if (pokemon!=null){
			lifetime-=Time.deltaTime;
			if (lifetime<0 && !fired){
				Transform particles = transform.FindChild("Particles");
				if (particles){
					particles.parent = null;
					particles.GetComponent<ParticleSystem>().Play();
					Destroy(particles.gameObject, 1);
				}
				Destroy(gameObject);
				fired = true;

				if (pokemon!=null){
					GameObject pokeObj = (GameObject)Instantiate(Resources.Load("Pokemon/"+Pokemon.GetName(pokemon.number)));
					pokeObj.transform.position = transform.position;
					pokeObj.transform.rotation = Quaternion.Euler(0,Random.value*360,0);
					pokeObj.GetComponent<PokemonObj>().pokemon = pokemon;
					pokeObj.name = pokemon.name;

					//assuming direct control
					if (trainer==Player.This.gameObject){
						pokeObj.AddComponent<PokemonPlayer>();
						Player.pokemonObj = pokeObj;
						Player.pokemonActive = true;
					}
				}
			}
		}
	}

	public static void ReleasePokemon(Pokemon pokemon, GameObject trainer){
		if (trainer==Player.This.gameObject){
			if (Player.pokemonActive)	return;
			Player.pokemonActive = true;
			Player.click = true;
		}

		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));

		ball.transform.position = GameObject.Find("_PokeballHolder").transform.position;
		ball.rigidbody.AddForce
			( Camera.main.transform.forward*500+ Camera.main.transform.up*300 );
		ball.GetComponent<Pokeball>().pokemon = pokemon;
		ball.GetComponent<Pokeball>().trainer = trainer;
	}

	public static void ThrowPokeBall(GameObject trainer){
		//find the nearest pokemon to capture, withing the correct direction I guess
		float dist = 1000000;
		GameObject pokemonOb = null;
		foreach(GameObject poke in GameObject.FindGameObjectsWithTag("pokemon")){
			Vector3 direct = trainer.transform.position-poke.transform.position;
			if (direct.sqrMagnitude<dist){
				dist = direct.sqrMagnitude;
				pokemonOb = poke;
			}
		}

		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));
		ball.transform.position = GameObject.Find("_PokeballHolder").transform.position;

		if (pokemonOb!=null){
			Vector3 direct = pokemonOb.transform.position - ball.transform.position;
			ball.rigidbody.AddForce( direct.normalized*500+ Vector3.up * direct.magnitude/50);
			ball.GetComponent<Pokeball>().trainer = trainer;
		}
	}
}