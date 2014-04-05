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
	GameObject currentPokemonObj = null;
	public Pokemon[] party;
	Vector3 trainerPosition = Vector3.zero;

	enum States {Ready, InBattle, Defeated};
	States currentState = States.Ready;

	void Update(){
		switch(currentState){

		case States.Ready:{
			Vector3 direct = Player.This.transform.position - transform.position;
			if (direct.sqrMagnitude<10*10 && Vector3.Dot(direct, transform.forward)>0){

				Dialog.inDialog = true;
				Dialog.NPCobj = gameObject;
				Dialog.NPCname = "Young Trainer";
				Dialog.text = "You're a pokemon trainer right? That means we have to battle!";
				if (Dialog.doneDialog){
					//populate pokemon
					party = new Pokemon[pokemon.Length];
					for(int i=0; i<pokemon.Length; i++){
						party[i] = new Pokemon((int)pokemon[i].pokemon, pokemon[i].level);
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
				direct.y = 0;
				transform.rotation = Quaternion.LookRotation(direct);
			}
			GetComponent<Animator>().SetBool("run", false);
		}
	}
}