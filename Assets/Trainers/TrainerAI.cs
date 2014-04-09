using UnityEngine;
using System.Collections;

public class TrainerAI : MonoBehaviour {
	Trainer trainer = null;
	Trainer enemyTrainer = null;

	Pokemon currentPokemon = null;
	Vector3 trainerPos = Vector3.zero;
	enum States {Idle, InBattle, Defeated};
	States currentState = States.Idle;

	void Start(){
		trainer = GetComponent<Trainer>();
	}

	void Update(){
		if (Player.trainer==this)	return;
		
		switch(currentState){
			
		case States.Idle:{
			Vector3 direct = Player.trainer.transform.position - transform.position;
			if (direct.sqrMagnitude<10*10 && Vector3.Dot(direct, transform.forward)>0){
				
				Dialog.inDialog = true;
				Dialog.NPCobj = gameObject;
				Dialog.NPCname = "Young Trainer";
				Dialog.text = "You're a pokemon trainer right? That means we have to battle!";
				if (Dialog.doneDialog){
					Dialog.inDialog = false;
					
					currentState = States.InBattle;
					trainerPos = transform.position - direct.normalized*10;
					enemyTrainer = Player.trainer;
				}
			}
			break;}
			
		case States.InBattle:	InBattle();	break;
			
		}
	}
	
	void InBattle(){
		//move trainer to position
		Vector3 direct = trainerPos-transform.position;
		direct.y = 0;
		if (direct.sqrMagnitude>2){
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", true);
		}else{
			if (direct.sqrMagnitude>1)	transform.position += direct;
			if (currentPokemon==null){
				currentPokemon = trainer.pokemon[0];
			}

			if (currentPokemon.obj!=null){
				direct = currentPokemon.obj.transform.position-transform.position;
			}else{
				direct = enemyTrainer.transform.position-transform.position;
			}
			direct.y = 0;
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", false);
			if (currentPokemon.obj==null)	trainer.ThrowPokemon(trainer.pokemon[0]);
		}
		
		/*if (currentPokemonObj!=null){
			PokemonTrainer pokeComp = currentPokemonObj.GetComponent<PokemonTrainer>;
			if (pokeComp!=null){
				if (Player.pokemonObj!=null){
					pokeComp.AttackEnemy(Player.pokemonObj);
				}
			}
		}*/
	}
}