using UnityEngine;
using System.Collections;

public class PokemonDomesticated : MonoBehaviour {
	public enum Orders {Heel, Idle, Charge}
	public Orders currentOrder = Orders.Heel;
	public Trainer trainer = null;
	
	bool letsGo = false;
	PokemonObj pokemonObj;
	GameGUI gamegui = new GameGUI();

	void Start(){
		pokemonObj = GetComponent<PokemonObj>();
		pokemonObj.pokemon.pp = 1;
	}

	void Update(){
		if (Player.pokemon.obj==pokemonObj && Player.pokemonActive)	return;

		switch(currentOrder){
		case Orders.Heel:{
			Vector3 direct = trainer.transform.position -transform.position;
			direct.y = 0;
			if (letsGo){
				transform.rotation = Quaternion.LookRotation(direct);
				pokemonObj.SetVelocity(direct.normalized * pokemonObj.speed);
			}
			if (direct.sqrMagnitude<10)	letsGo = false;
			if (direct.sqrMagnitude>20)	letsGo = true;
			break;}
		}
	}

	public void BattleGUI(){
		GUI.DrawTexture(new Rect(0,Screen.height-90,200,100), GUImgr.gradRight);
		float ypos = Screen.height-85;
		GUI.Label(new Rect(10,ypos,200,20), name+" lvl"+pokemonObj.pokemon.level.ToString());
		
		//stats
		ypos+=20;
		GUI.Label(new Rect(10,ypos,200,20), "HP");
		GUImgr.DrawBar(new Rect(35,ypos+5,200,10), pokemonObj.pokemon.hp, GUImgr.hp);
		
		ypos+=20;
		GUI.Label(new Rect(10,ypos,200,20), "PP");
		GUImgr.DrawBar(new Rect(35,ypos+5,200,10), pokemonObj.pokemon.pp, GUImgr.pp);
		
		ypos+=20;
		GUI.Label(new Rect(10,ypos,200,20), "XP");
		GUImgr.DrawBar(new Rect(35,ypos+5,200,10), pokemonObj.pokemon.xp, GUImgr.xp);

		//current target
		if (pokemonObj.enemy!=null){
			if (pokemonObj.enemy.pokemon!=null){
				GUI.DrawTexture(new Rect(0,0,200,60), GUImgr.gradRight);
				ypos = 5;
				GUI.Label(new Rect(10,ypos,200,20), pokemonObj.enemy.name+" lvl"+pokemonObj.enemy.pokemon.level.ToString());
				ypos+=20;
				GUI.Label(new Rect(10,ypos,200,20), "HP");
				GUImgr.DrawBar(new Rect(35,ypos+5,200,10), pokemonObj.enemy.pokemon.hp, GUImgr.hp);;
			}
		}

		//moves
		float height = pokemonObj.pokemon.moves.Count*40+10;
		GUI.DrawTexture(new Rect(Screen.width-200,Screen.height-height,200,height), GUImgr.gradLeft);
		ypos = Screen.height-40;
		float xpos = Screen.width-150;
		int moveN = pokemonObj.pokemon.moves.Count;
		foreach(Move move in pokemonObj.pokemon.moves){
			GUI.Label(new Rect(xpos,ypos,200,20), moveN.ToString()+" - "+move.moveType.ToString());
			GUImgr.DrawBar(new Rect(xpos,ypos+20,100,5), move.cooldown, GUImgr.pp);
			ypos -= 40;
			bool useMove = false;
			if (!Player.click){
				switch(moveN){
				case 1:	if(Input.GetKey(KeyCode.Alpha1) ||  Input.GetKey(KeyCode.Keypad1))	useMove=true;	break;
				case 2:	if(Input.GetKey(KeyCode.Alpha2) ||  Input.GetKey(KeyCode.Keypad2))	useMove=true;	break;
				case 3:	if(Input.GetKey(KeyCode.Alpha3) ||  Input.GetKey(KeyCode.Keypad3))	useMove=true;	break;
				case 4:	if(Input.GetKey(KeyCode.Alpha4) ||  Input.GetKey(KeyCode.Keypad4))	useMove=true;	break;
				case 5:	if(Input.GetKey(KeyCode.Alpha5) ||  Input.GetKey(KeyCode.Keypad5))	useMove=true;	break;
				case 6:	if(Input.GetKey(KeyCode.Alpha6) ||  Input.GetKey(KeyCode.Keypad6))	useMove=true;	break;
				case 7:	if(Input.GetKey(KeyCode.Alpha7) ||  Input.GetKey(KeyCode.Keypad7))	useMove=true;	break;
				case 8:	if(Input.GetKey(KeyCode.Alpha8) ||  Input.GetKey(KeyCode.Keypad8))	useMove=true;	break;
				case 9:	if(Input.GetKey(KeyCode.Alpha9) ||  Input.GetKey(KeyCode.Keypad9))	useMove=true;	break;
				}
			}
			if (useMove){
				pokemonObj.UseMove(transform.forward, move);
			}
			moveN--;
		}
	}
}