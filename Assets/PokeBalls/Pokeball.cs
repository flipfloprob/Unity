using UnityEngine;
using System.Collections;

public class Pokeball : MonoBehaviour {
	public Trainer trainer = null;
	public Pokemon pokemon = null;

	public bool active = true;
	bool fired = false;
	static GameGUI gamegui = new GameGUI();

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
					pokemon.obj = pokeObj.GetComponent<PokemonObj>();
					PokemonDomesticated pokeDom = pokeObj.AddComponent<PokemonDomesticated>();
					pokeDom.trainer = trainer;

					//assuming direct control
					if (trainer==Player.trainer){
						Player.pokemonActive = true;
						Debug.Log("Assuming direct control");
					}
				}
			}
		}
	}

	public static void ThrowPokeBall(Trainer trainer){
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

	public static void CapturePokemon() {
		string printme="";
		PokemonObj targetPokemon = Player.pokemon.obj.enemy;
		if (targetPokemon != null) {
			float statusAilment = 0;	//statusAilment = 12 if poisoned/burned/paralyzed, 25 if frozen or asleep, 0 otherwise.
			float ballMod = 150;		//ballMod = 255 if using a Poké Ball, 200 if using a Great Ball, and 150 otherwise.
			float captureOne = statusAilment / (ballMod+1);
			float captureRate = 22;	//need to put this into DB: http://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_catch_rate
			float ballFactor = 12;
			float f = (((targetPokemon.pokemon.TotalHP())*255) / ballFactor) / (targetPokemon.pokemon.hp / 4);
			/*
			 * 	f = (HPmax * 255 / Ball Factor) / (HPcurrent / 4), where all divisions 
			 * are rounded down to the nearest integer (the denominator is set to 1 if 
			 * it is 0 as a result). The Ball Factor is 8 if a Great Ball is used, and 
			 * 12 otherwise. The resulting value is capped at a maximum of 255. 
			 */	
			float captureTwo = ((captureRate+1)/(ballMod+1)) * ((f+1)/256);

			//printme = "capture " + targetPokemon.pokemon.name + ". It has " + targetPokemon.pokemon.hp + "hp remaining!";
			//if (targetPokemon.pokemon.hp*100 < 15) {
			float captureChance = captureOne+captureTwo;
			if (captureChance >= Random.value) {
				//printme = printme + "\n Okay!";
				printme = "You've captured a " + targetPokemon.pokemon.GetName() + "!";
				targetPokemon.Return();
				Player.trainer.pokemon.Add(new Pokemon(targetPokemon.pokemon.number,true));
			}
			else {
				//printme = printme + "\n It's too strong!";
				printme = "You tried to capture " + targetPokemon.pokemon.GetName() + ", but it broke free!";
			}
			//printme += "\n " + captureChance;
		}
		else {
			//printme = "Nothing found to capture!";
		}
		gamegui.SetChatWindow(printme);
	}
}