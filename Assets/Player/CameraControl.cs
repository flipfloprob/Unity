using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {
	public static Vector3 cameraFocus = Vector3.zero;
	Pokemon pokemon = null;
	bool pokemonActive = false;
	public static float ax = 0;
	public static float ay = 0;
	Trainer trainer = null;
	Vector3 camPos = Vector3.zero;
	float cameraZoom = 6;

	void Update() {
		//camera input
		if (!GameGUI.menuActive || Input.GetMouseButton(1)){
			ax -= Input.GetAxis("Mouse Y")*5;
			ay += Input.GetAxis("Mouse X")*5;
			ax = Mathf.Clamp(ax,-80,80);
			ay = ay%360;
		}
	}

	void LateUpdate(){
		pokemon = Player.pokemon;
		pokemonActive = Player.pokemonActive;
		trainer = Player.trainer;
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
}