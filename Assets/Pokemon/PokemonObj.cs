using UnityEngine;
using System.Collections;

public class PokemonObj : MonoBehaviour {
	public float speed = 5;
	public Pokemon pokemon = null;
	public PokemonObj enemy = null;
	
	Vector3 velocity = Vector3.zero;
	bool returning = false;
	GameGUI gamegui = new GameGUI();
	
	void Update(){
		velocity -= rigidbody.velocity;
		velocity.y = 0;
		if (velocity.sqrMagnitude>speed*speed)	velocity = velocity.normalized*speed;
		rigidbody.AddForce(velocity, ForceMode.VelocityChange);
		velocity = Vector3.zero;
		
		if (pokemon!=null){
			foreach(Move move in pokemon.moves){
				move.cooldown += Time.deltaTime;
				move.cooldown = Mathf.Clamp01(move.cooldown);
			}
			
			if (pokemon.hp<=0){
				Return();
			}
		}
	}
	
	public void SetVelocity(Vector3 vel){
		velocity = vel;
	}
	
	public void Return(){
		if (returning)	return;
		if (Player.pokemon == pokemon) {
			Player.pokemonActive = false;
			//gamegui.SetChatWindow(gameObject.GetComponent<Pokeball>().pokemon.GetName() + "! Return!");
			//gamegui.SetChatWindow(Player.pokemonObj.GetComponent<Pokeball>().pokemon.GetName() + "! Return!");
			gamegui.SetChatWindow(pokemon.GetName() + "! Return!");
		}
		returning = true;
		GameObject effect = (GameObject)Instantiate(Resources.Load("ReturnEffect"));
		effect.transform.position = transform.position;
		effect.transform.parent = transform;
		Destroy(gameObject,1);
		pokemon.thrown = false;
	}
	
	public bool UseMove(Vector3 direction, Move move){
		if (move.GetPPCost()>pokemon.pp)	return false;
		string attackChat = "";
		if (pokemon.isPlayer) {
			attackChat = "Your ";
		}
		else {
			attackChat = "Enemy ";
		}
		attackChat += pokemon.name + " used " + move.moveType + "!";
		switch(move.moveType){
			
		case MoveNames.Growl:{
			if (move.cooldown<1)	return false;
			const float range = 10;
			RaycastHit[] hits = Physics.SphereCastAll(transform.position+Vector3.up, 1, direction ,range, 1<<10);
			foreach(RaycastHit hit in hits){
				if (hit.collider.gameObject!=gameObject){
					PokemonObj enemyObj = hit.collider.GetComponent<PokemonObj>();
					
					if ((enemyObj.transform.position-transform.position).sqrMagnitude<range*range){
						GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Debuff"));
						newEffect.transform.position = enemyObj.transform.position+Vector3.up*0.2f;
						newEffect.transform.parent = enemyObj.transform;
					}
					
					if (enemyObj){
						if (enemyObj.pokemon!=null)	enemyObj.pokemon.DeBuff(pokemon,move);
						enemy = enemyObj;
						enemyObj.enemy = this;
					}
				}
			}
			audio.PlayOneShot((AudioClip)Resources.Load("Audio/Growl"));
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
			
		case MoveNames.TailWhip:{
			if (move.cooldown<1)	return false;
			const float range = 10;
			RaycastHit[] hits = Physics.SphereCastAll(transform.position+Vector3.up, 1, direction ,range, 1<<10);
			foreach(RaycastHit hit in hits){
				if (hit.collider.gameObject!=gameObject){
					PokemonObj enemyObj = hit.collider.GetComponent<PokemonObj>();
					
					if ((enemyObj.transform.position-transform.position).sqrMagnitude<range*range){
						GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Debuff"));
						newEffect.transform.position = enemyObj.transform.position+Vector3.up*0.2f;
						newEffect.transform.parent = enemyObj.transform;
					}
					
					if (enemyObj){
						if (enemyObj.pokemon!=null)	enemyObj.pokemon.DeBuff(pokemon,move);
						enemy = enemyObj;
						enemyObj.enemy = this;
					}
				}
			}
			
			
			
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
			
		case MoveNames.Tackle:{
			if (move.cooldown<1)	return false;
			const float range = 2;
			RaycastHit[] hits = Physics.SphereCastAll(transform.position+Vector3.up, 1, direction ,range, 1<<10);
			foreach(RaycastHit hit in hits){
				if (hit.collider.gameObject!=gameObject){
					PokemonObj enemyObj = hit.collider.GetComponent<PokemonObj>();
					GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Bash"));
					newEffect.transform.position = hit.point;
					if (enemyObj){
						if (enemyObj.pokemon!=null)	enemyObj.pokemon.Damage(pokemon,move);
						enemy = enemyObj;
						enemyObj.enemy = this;
					}
				}
			}
			rigidbody.AddForce(direction*range*rigidbody.mass*500);
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
			
		case MoveNames.Scratch:{
			if (move.cooldown<1)	return false;
			const float range = 2;
			RaycastHit[] hits = Physics.SphereCastAll(transform.position+Vector3.up, 1, direction ,range, 1<<10);
			foreach(RaycastHit hit in hits){
				if (hit.collider.gameObject!=gameObject){
					PokemonObj enemyObj = hit.collider.GetComponent<PokemonObj>();
					GameObject newEffect = (GameObject)Instantiate(Resources.Load("Effects/Scratch"));
					newEffect.transform.position = hit.point;
					if (enemyObj){
						if (enemyObj.pokemon!=null)	enemyObj.pokemon.Damage(pokemon,move);
						enemy = enemyObj;
						enemyObj.enemy = this;
					}
					move.cooldown = 0;
					pokemon.pp-=move.GetPPCost();
					return true;
				}
			}
			GameObject neweffect = (GameObject)Instantiate(Resources.Load("Effects/Scratch"));
			neweffect.transform.position = transform.position+Vector3.up+direction;
			move.cooldown = 0;
			pokemon.pp-=move.GetPPCost();
			return true;}
		}
		gamegui.SetChatWindow (attackChat);
		return false;
	}
}