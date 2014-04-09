using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {
	//Initialise database upon game start and free on destroy
	void Start(){
		Data.Init();
	}
	
	void OnDestroy(){
		Data.Free();
	}

	void Update(){
		//eventually fill this with some opening cinematic or similar

		//for now just load the demo scene
		Application.LoadLevel("Route01");
	}
}