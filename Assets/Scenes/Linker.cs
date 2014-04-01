using UnityEngine;
using System.Collections;

public class Linker : MonoBehaviour {
	public string zone = "";

	void OnTriggerEnter (Collider col) {
		if (col.gameObject==Player.This.gameObject){
			Application.LoadLevel(zone);
		}
	}
}