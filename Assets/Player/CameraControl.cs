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
	public static bool releaseCursor = false;
	Target activeTarget;
	//public GameGUI gamegui = new GameGUI();


	void Start() {
		//activeTarget = GetComponent<Target>();
		gameObject.AddComponent ("Target");
	}

	void Update() {
		//camera input
		if (!GameGUI.menuActive || Input.GetMouseButton(1)){
			ax -= Input.GetAxis("Mouse Y")*5;
			ay += Input.GetAxis("Mouse X")*5;
			ax = Mathf.Clamp(ax,-80,80);
			ay = ay%360;
		}

		//release cursor true so you can click on stuff
		if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
			releaseCursor = true;
			if (Input.GetMouseButtonDown(0)){ // when button clicked...
				RaycastHit hit; // cast a ray from mouse pointer:
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				// if enemy hit...
				if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("pokemon")){
					//gamegui.SetChatWindow("Found");
					if (activeTarget.GetActiveTarget()) {
						activeTarget.UnHighlightTarget();
					}
					activeTarget.SetTarget(hit.transform);
					activeTarget.HighlightTarget();
				}
			}
		}
		//Capture cursor
		if (Input.GetKeyUp (KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) {
			releaseCursor = false;
		}

		//Zoom camera in/out with mouse scrollwheel
		if (Input.GetAxis ("Mouse ScrollWheel")!=0) {
			cameraZoom-=(Input.GetAxis("Mouse ScrollWheel")*4);
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
			if (pokemon!=null && pokemonActive && !releaseCursor) {
				//focus on current pokemon
				cameraFocus = pokemon.obj.transform.position + Vector3.up;
				transform.rotation = pokemon.obj.transform.rotation * Quaternion.Euler(ax,0,0);
			}
			else if (releaseCursor) {
				//do nothing
				//keep camera steady and let user click around
			}
			else{
				//focus on player
				cameraFocus = trainer.transform.position+Vector3.up*2;
				Camera.main.transform.rotation = Quaternion.Euler(ax,ay,0);
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