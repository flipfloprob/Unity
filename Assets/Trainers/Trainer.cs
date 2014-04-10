using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trainer : MonoBehaviour {
	[System.Serializable]
	public class PartyPokemon{
		public Pokemon_Names pokemon = Pokemon_Names.Bulbasaur;
		public string name = "MyPokemon";
		public int level = 5;
	}
	public PartyPokemon[] pokemon;
	bool pokemonActive = false;
	GameObject currentPokemonObj = null;
	public Pokemon[] party;
	Vector3 trainerPosition = Vector3.zero;

	enum States {Ready, InBattle, Defeated};
	States currentState = States.Ready;

	void Update(){
		switch(currentState){

		case States.Ready:{
			Vector3 direct = Player.This.transform.position - transform.position; //Null reference exception crash is thrown here (assertion failed)
			if (direct.sqrMagnitude<10*10 && Vector3.Dot(direct, transform.forward)>0){

				Dialog.inDialog = true;
				Dialog.NPCobj = gameObject;
				Dialog.NPCname = "Young Trainer";
				Dialog.text = "You're a pokemon trainer right? That means we have to battle!";
				if (Dialog.doneDialog){
					Dialog.inDialog = false;
					//populate pokemon
					party = new Pokemon[pokemon.Length];
					for(int i=0; i<pokemon.Length; i++){
						party[i] = new Pokemon((int)(pokemon[i].pokemon), false, pokemon[i].level);
						party[i].name = pokemon[i].name;
					}
					currentState = States.InBattle;
					trainerPosition = transform.position - direct.normalized*10;
				}
			}
			break;}

		case States.InBattle:	InBattle();	break;

		}
	}

	void InBattle(){
		//move trainer to position
		Vector3 direct = trainerPosition-transform.position;
		direct.y = 0;
		if (direct.sqrMagnitude>1){
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", true);
		}else{
			if (currentPokemonObj!=null){
				direct = currentPokemonObj.transform.position-transform.position;
			}else{
				direct = Player.This.transform.position-transform.position;
			}
			direct.y = 0;
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", false);
			if (pokemonActive==false)	ThrowPokemon(party[0]);
		}
	}

	void ThrowPokemon(Pokemon poke){
		pokemonActive = true;
		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));
		ball.transform.position = transform.position;
		ball.rigidbody.AddForce( (transform.forward*2+ transform.up)*500 );
		ball.GetComponent<Pokeball>().pokemon = poke;
		ball.GetComponent<Pokeball>().trainer = gameObject;
		//gamegui.SetChatWindow(ball.GetComponent<Pokeball>().pokemon.GetName() + "! I choose you!");
	}
}