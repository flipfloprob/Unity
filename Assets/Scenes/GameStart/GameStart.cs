using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {
	void Update(){
		//eventually fill this with some opening cinematic or similar

		//for now just load the demo scene
		Application.LoadLevel("Route01");
	}
}