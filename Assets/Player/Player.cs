using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public static Player This;
	public static float ax = 0;
	public static float ay = 0;
	public static Vector3 cameraFocus = Vector3.zero;
	Vector3 camPos = Vector3.zero;
	float cameraZoom = 6;
	public static bool click = false;

	public static bool pokemonActive = false;
	public static GameObject pokemonObj = null;
	public static GameGUI gamegui = new GameGUI();

	void Start(){
		if (Pokemon.party.Count == 0) {
			GUImgr.Start ();
			This = this;
			Pokemon.party.Add (new Pokemon (1, true));
			Pokemon.party.Add (new Pokemon (4, true));
			Pokemon.party.Add (new Pokemon (7, true));
			Pokedex.states [1] = Pokedex.State.Captured;
			Pokedex.states [4] = Pokedex.State.Captured;
			Pokedex.states [7] = Pokedex.State.Captured;

			Item.inventory.Add (new Item (ItemTypes.Pokeball, 5));
			Item.inventory.Add (new Item (ItemTypes.Potion, 2));
		}
	}

	void Update(){
		if (Dialog.inDialog){
			Screen.lockCursor = false;
			Screen.showCursor = true;
			GetComponent<Animator>().SetBool("run",false);
			return;
		}


		if (GameGUI.menuActive && !pokemonActive){
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}else{
			Screen.lockCursor = true;
			Screen.showCursor = false;
		}

		//camera input
		if (!GameGUI.menuActive || Input.GetMouseButton(1)){
			ax -= Input.GetAxis("Mouse Y")*5;
			ay += Input.GetAxis("Mouse X")*5;
			ax = Mathf.Clamp(ax,-80,80);
			ay = ay%360;
		}
		
		//player control / animation
		Animator ani = GetComponent<Animator>();
		if (!pokemonActive || pokemonObj==null){
			Vector3 vel = Quaternion.Euler(0,ay,0) * (Vector3.forward*Input.GetAxis("Vertical") + Vector3.right*Input.GetAxis("Horizontal"));
			if (vel.magnitude>0.1f){
				ani.SetBool("run",true);
				transform.rotation = Quaternion.LookRotation(vel);
			}else
				ani.SetBool("run",false);
		}else{
			ani.SetBool("run",false);
		}

		//swap pokemon
		if (!click && !pokemonActive){
			Pokemon oldPokemonSelection = Pokemon.selected;
			if (Pokemon.party.Count>0 && (Input.GetKey(KeyCode.Alpha1) ||  Input.GetKey(KeyCode.Keypad1)))	Pokemon.selected = Pokemon.party[0];
			if (Pokemon.party.Count>1 && (Input.GetKey(KeyCode.Alpha2) ||  Input.GetKey(KeyCode.Keypad2)))	Pokemon.selected = Pokemon.party[1];
			if (Pokemon.party.Count>2 && (Input.GetKey(KeyCode.Alpha3) ||  Input.GetKey(KeyCode.Keypad3)))	Pokemon.selected = Pokemon.party[2];
			if (Pokemon.party.Count>3 && (Input.GetKey(KeyCode.Alpha4) ||  Input.GetKey(KeyCode.Keypad4)))	Pokemon.selected = Pokemon.party[3];
			if (Pokemon.party.Count>4 && (Input.GetKey(KeyCode.Alpha5) ||  Input.GetKey(KeyCode.Keypad5)))	Pokemon.selected = Pokemon.party[4];
			if (Pokemon.party.Count>5 && (Input.GetKey(KeyCode.Alpha6) ||  Input.GetKey(KeyCode.Keypad6)))	Pokemon.selected = Pokemon.party[5];
			if (Pokemon.party.Count>6 && (Input.GetKey(KeyCode.Alpha7) ||  Input.GetKey(KeyCode.Keypad7)))	Pokemon.selected = Pokemon.party[6];
			if (Pokemon.party.Count>7 && (Input.GetKey(KeyCode.Alpha8) ||  Input.GetKey(KeyCode.Keypad8)))	Pokemon.selected = Pokemon.party[7];
			if (Pokemon.party.Count>8 && (Input.GetKey(KeyCode.Alpha9) ||  Input.GetKey(KeyCode.Keypad9)))	Pokemon.selected = Pokemon.party[8];
			if (Pokemon.party.Count>9 && (Input.GetKey(KeyCode.Alpha0) ||  Input.GetKey(KeyCode.Keypad0)))	Pokemon.selected = Pokemon.party[9];
			if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)){
				if (Pokemon.selected==Pokemon.party[0])
					Pokemon.selected = Pokemon.party[Pokemon.party.Count-1];
				else
					if (Pokemon.party.Contains(Pokemon.selected))	Pokemon.selected = Pokemon.party[Pokemon.party.IndexOf(Pokemon.selected)-1];
			}
			if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Plus)|| Input.GetKey(KeyCode.KeypadPlus)){
				if (Pokemon.selected==Pokemon.party[Pokemon.party.Count-1])
					Pokemon.selected = Pokemon.party[0];
				else
					if (Pokemon.party.Contains(Pokemon.selected))	Pokemon.selected = Pokemon.party[Pokemon.party.IndexOf(Pokemon.selected)+1];
			}
			if (oldPokemonSelection!=Pokemon.selected){
				click = true;
				if (pokemonObj!=null){
					pokemonObj.GetComponent<PokemonObj>().Return();
					ThrowPokemon();
				}
			}
		}

		if (!Pokemon.party.Contains(Pokemon.selected))		Pokemon.selected = null;
		if (Pokemon.selected==null && Pokemon.party.Count>0)		Pokemon.selected = Pokemon.party[0];

		if (!Item.inventory.Contains(Item.selected))		Item.selected = null;
		if (Item.selected==null && Item.inventory.Count>0)	Item.selected = Item.inventory[0];

		//release pokemon
		if (!click && Input.GetKey(KeyCode.Return)){
			if (pokemonObj==null){
				if (!pokemonActive && Pokemon.selected!=null)	ThrowPokemon();
			}else{
				if (pokemonActive){
					pokemonObj.GetComponent<PokemonObj>().Return();
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

		//inventoryMGR
		for(int i=0; i<Item.inventory.Count; i++){
			if (Item.inventory[i].number<=0)	Item.inventory.Remove(Item.inventory[i]);
		}
	}

	public static void ThrowPokemon(){
		if (pokemonActive)	return;
		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));
		ball.transform.position = GameObject.Find("_PokeballHolder").transform.position;
		ball.rigidbody.AddForce
			( Camera.main.transform.forward*500+ Camera.main.transform.up*300 );
		ball.GetComponent<Pokeball>().pokemon = Pokemon.selected;
		ball.GetComponent<Pokeball>().trainer = This.gameObject;
		pokemonActive = true;
		click = true;
		gamegui.SetChatWindow(ball.GetComponent<Pokeball>().pokemon.GetName() + "! I choose you!");
	}

	public static void CapturePokemon(){
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
		Destroy (ball, 1);
	}

	void LateUpdate(){
		Quaternion camRot = Camera.main.transform.rotation;

		if (Dialog.NPCobj=null){
			//focus on person speaking to you
			Vector3 camFocus = Dialog.NPCobj.transform.position+Vector3.up;
			Camera.main.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position-camFocus);
		}else{
			if (pokemonObj!=null && pokemonActive){
				//focus on current pokemon
				Camera.main.transform.rotation = pokemonObj.transform.rotation * Quaternion.Euler(ax,0,0);
			}else{
				//focus on player
				cameraFocus = transform.position+Vector3.up*2;
				Camera.main.transform.rotation = Quaternion.Euler(ax,ay,0);
			}
		}
		Camera.main.transform.position = cameraFocus;
		Camera.main.transform.Translate(0,0,-cameraZoom);

		Camera.main.transform.position = Vector3.Lerp(camPos, Camera.main.transform.position, Time.deltaTime*5);
		Camera.main.transform.rotation = Quaternion.Lerp(camRot, Camera.main.transform.rotation, Time.deltaTime*5);
		camPos = Camera.main.transform.position;

		RaycastHit hit;
		Vector3 camDirect = Camera.main.transform.position - cameraFocus;
		if (Physics.Raycast(cameraFocus, camDirect, out hit, camDirect.magnitude, 1)){
			Camera.main.transform.position = hit.point - camDirect.normalized*0.5f;
		}
	}
}