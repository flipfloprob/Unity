using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public static bool click = false;
	static bool jumpCool = true;

	public static Trainer trainer = null;
	public static Pokemon pokemon = null;
	public static Item item = null;
	public static bool pokemonActive = false;

	//public static float ax = 0;
	//public static float ay = 0;
	//public static Vector3 cameraFocus = Vector3.zero;

	//Vector3 camPos = Vector3.zero;
	//float cameraZoom = 6;

	public static GameGUI gamegui = new GameGUI();

	void Start(){
		trainer = GameObject.Find("Player").GetComponent<Trainer>();
		gameObject.AddComponent ("CameraControl");
	}

	void Update(){
		//do nothing if in dialog
		if (Dialog.inDialog){
			Screen.lockCursor = false;
			Screen.showCursor = true;
			trainer.SetVelocity(Vector3.zero);
			return;
		}

		//menu
		if (GameGUI.menuActive && !pokemonActive){
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}else{
			Screen.lockCursor = true;
			Screen.showCursor = false;
		}
	/*
		//camera input
		if (!GameGUI.menuActive || Input.GetMouseButton(1)){
			ax -= Input.GetAxis("Mouse Y")*5;
			ay += Input.GetAxis("Mouse X")*5;
			ax = Mathf.Clamp(ax,-80,80);
			ay = ay%360;
		}
	*/	
		//player control
		if (pokemonActive && pokemon.obj!=null){
			//move pokemon
			trainer.SetVelocity(Vector3.zero);

			Vector3 velocity = Vector3.zero;
			velocity += pokemon.obj.transform.forward * Input.GetAxis("Vertical");
			velocity += pokemon.obj.transform.right * Input.GetAxis("Horizontal");
			velocity *= pokemon.obj.speed;

			pokemon.obj.SetVelocity(velocity);
			pokemon.obj.transform.Rotate(pokemon.obj.transform.up, Input.GetAxis("Mouse X"));
			
			if(Input.GetButton("Jump") && jumpCool && Physics.Raycast(pokemon.obj.transform.position+Vector3.up*0.1f, Vector3.down, 0.2f)){
				pokemon.obj.rigidbody.AddForce(Vector3.up*3000);
				jumpCool = false;
			}
			if(!Input.GetButton("Jump"))	jumpCool = true;
			
			pokemon.pp -= Time.deltaTime/500;
			if (pokemon.pp<=0){
				pokemonActive = false;
				pokemon.obj.Return();
			}

		}else{
			//move trainer
			Vector3 vel = Quaternion.Euler(0,CameraControl.ay,0) * (Vector3.forward*Input.GetAxis("Vertical") + Vector3.right*Input.GetAxis("Horizontal"));
			trainer.SetVelocity(vel);
		}

		//swap pokemon
		if (!click && !pokemonActive){
			Pokemon oldPokemonSelection = pokemon;
			if (trainer.pokemon.Count>0 && (Input.GetKey(KeyCode.Alpha1) ||  Input.GetKey(KeyCode.Keypad1)))	pokemon = trainer.pokemon[0];
			if (trainer.pokemon.Count>1 && (Input.GetKey(KeyCode.Alpha2) ||  Input.GetKey(KeyCode.Keypad2)))	pokemon = trainer.pokemon[1];
			if (trainer.pokemon.Count>2 && (Input.GetKey(KeyCode.Alpha3) ||  Input.GetKey(KeyCode.Keypad3)))	pokemon = trainer.pokemon[2];
			if (trainer.pokemon.Count>3 && (Input.GetKey(KeyCode.Alpha4) ||  Input.GetKey(KeyCode.Keypad4)))	pokemon = trainer.pokemon[3];
			if (trainer.pokemon.Count>4 && (Input.GetKey(KeyCode.Alpha5) ||  Input.GetKey(KeyCode.Keypad5)))	pokemon = trainer.pokemon[4];
			if (trainer.pokemon.Count>5 && (Input.GetKey(KeyCode.Alpha6) ||  Input.GetKey(KeyCode.Keypad6)))	pokemon = trainer.pokemon[5];
			if (trainer.pokemon.Count>6 && (Input.GetKey(KeyCode.Alpha7) ||  Input.GetKey(KeyCode.Keypad7)))	pokemon = trainer.pokemon[6];
			if (trainer.pokemon.Count>7 && (Input.GetKey(KeyCode.Alpha8) ||  Input.GetKey(KeyCode.Keypad8)))	pokemon = trainer.pokemon[7];
			if (trainer.pokemon.Count>8 && (Input.GetKey(KeyCode.Alpha9) ||  Input.GetKey(KeyCode.Keypad9)))	pokemon = trainer.pokemon[8];
			if (trainer.pokemon.Count>9 && (Input.GetKey(KeyCode.Alpha0) ||  Input.GetKey(KeyCode.Keypad0)))	pokemon = trainer.pokemon[9];
			if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)){
				if (pokemon==trainer.pokemon[0])
					pokemon = trainer.pokemon[trainer.pokemon.Count-1];
				else
					if (trainer.pokemon.Contains(pokemon))	pokemon = trainer.pokemon[trainer.pokemon.IndexOf(pokemon)-1];
			}
			if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Plus)|| Input.GetKey(KeyCode.KeypadPlus)){
				if (pokemon==trainer.pokemon[trainer.pokemon.Count-1])
					pokemon = trainer.pokemon[0];
				else
					if (trainer.pokemon.Contains(pokemon))	pokemon = trainer.pokemon[trainer.pokemon.IndexOf(pokemon)+1];
			}
			if (oldPokemonSelection!=pokemon){
				click = true;
				if (oldPokemonSelection.obj!=null){
					oldPokemonSelection.obj.Return();
					trainer.ThrowPokemon(pokemon);
				}
			}
		}

		if (!trainer.pokemon.Contains(pokemon))				pokemon = null;
		if (pokemon==null && trainer.pokemon.Count>0)		pokemon = trainer.pokemon[0];

		if (!trainer.inventory.Contains(item))			item = null;
		if (item==null && trainer.inventory.Count>0)	item = trainer.inventory[0];

		//throw pokemon
		if (!click && Input.GetKey(KeyCode.Return)){
			if (pokemon.obj==null){
				trainer.ThrowPokemon(pokemon);
			}else{
				if (pokemonActive){
					pokemon.obj.Return();
					pokemonActive = false;
				}else{
					pokemonActive = true;
				}
			}
			click = true;
		}

		//activate menu
		if (Input.GetKey(KeyCode.Escape) && !click){
			if (pokemonActive)
				pokemonActive = false;
			else
				GameGUI.menuActive = !GameGUI.menuActive;
			click = true;
		}

		//capture pokemon
		if(Input.GetKeyDown("c")) {
			GameGUI gamegui = GetComponent<GameGUI>();
			CapturePokemon();
			click = true;
		}
		
		//chat window
		if(Input.GetKeyDown ("i")){
			if(GameGUI.chatActive)
				GameGUI.chatActive=false;
			else
				GameGUI.chatActive=true;
			
			click = true;
		}

		if (Input.GetKeyDown ("h")) {
			PokeCenter.HealPokemon ();
		}
	/*
	 * don't try using this right now, because it doesn't exist!
		if (Input.GetKeyDown ("k")) {
			Populate okasf = new Populate();
			okasf.Test();
		}
	*/
		//anticlick
		if (!Input.GetKey(KeyCode.Alpha1) &&  !Input.GetKey(KeyCode.Keypad1)
		    && !Input.GetKey(KeyCode.Alpha2) &&  !Input.GetKey(KeyCode.Keypad2)
		    && !Input.GetKey(KeyCode.Alpha3) &&  !Input.GetKey(KeyCode.Keypad3)
		    && !Input.GetKey(KeyCode.Alpha4) &&  !Input.GetKey(KeyCode.Keypad4)
		    && !Input.GetKey(KeyCode.Alpha5) &&  !Input.GetKey(KeyCode.Keypad5)
		    && !Input.GetKey(KeyCode.Alpha6) &&  !Input.GetKey(KeyCode.Keypad6)
		    && !Input.GetKey(KeyCode.Alpha7) &&  !Input.GetKey(KeyCode.Keypad7)
		    && !Input.GetKey(KeyCode.Alpha8) &&  !Input.GetKey(KeyCode.Keypad8)
		    && !Input.GetKey(KeyCode.Alpha9) &&  !Input.GetKey(KeyCode.Keypad9)
		    && !Input.GetKey(KeyCode.PageDown) &&  !Input.GetKey(KeyCode.PageUp)
		    && !Input.GetKey(KeyCode.KeypadMinus) &&  !Input.GetKey(KeyCode.KeypadPlus)
		    && !Input.GetKey(KeyCode.Minus) &&  !Input.GetKey(KeyCode.Equals)
		    && !Input.GetKey(KeyCode.Comma) &&  !Input.GetKey(KeyCode.Period)
			&& !Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.Escape)
		    && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
			click = false;
	}

	public static void CapturePokemon(){
		/*
		GameGUI gamegui = new GameGUI();
		Debug.LogError("Capture Pokemon");
		Vector3 pokemonPositon = pokemonObj.transform.position;
		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));
		//ball.transform.position = GameObject.Find("_PokeballHolder").transform.position;
		GameObject.Find ("_PokeballHolder").transform.LookAt(pokemonPositon);
		ball.transform.position = GameObject.Find ("_PokeballHolder").transform.position;
		//ball.rigidbody.AddForce
		//	( Camera.main.transform.forward*500+ Camera.main.transform.up*300 );
		ball.rigidbody.AddForce(pokemonPositon*500 + Camera.main.transform.up*300);
		Pokeball.CapturePokemon();
		Destroy (ball, 1);*/
	}
/*
	void LateUpdate(){
		Quaternion camRot = transform.rotation;

		if (Dialog.NPCobj=null){
			//focus on person speaking to you
			Vector3 camFocus = Dialog.NPCobj.transform.position+Vector3.up;
			transform.rotation = Quaternion.LookRotation(transform.position-camFocus);
		}else{
			if (pokemon.obj!=null && pokemonActive){
				//focus on current pokemon
				cameraFocus = pokemon.obj.transform.position + Vector3.up;
				transform.rotation = pokemon.obj.transform.rotation * Quaternion.Euler(ax,0,0);
			}else{
				//focus on player
				cameraFocus = trainer.transform.position+Vector3.up*2;
				transform.rotation = Quaternion.Euler(ax,ay,0);
			}
		}
		transform.position = cameraFocus;
		transform.Translate(0,0,-cameraZoom);

		transform.position = Vector3.Lerp(camPos, transform.position, Time.deltaTime*5);
		transform.rotation = Quaternion.Lerp(camRot, transform.rotation, Time.deltaTime*5);
		camPos = transform.position;

		RaycastHit hit;
		Vector3 camDirect = transform.position - cameraFocus;
		if (Physics.Raycast(cameraFocus, camDirect, out hit, camDirect.magnitude, 1)){
			transform.position = hit.point - camDirect.normalized*0.5f;
		}
	}
*/
}