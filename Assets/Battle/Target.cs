using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Target : MonoBehaviour {
	private List<Transform> allPokemon = new List<Transform>();
	private Transform targetedPokemon;
	private Transform playerTransform;
	public bool activeTarget = false;
	GameGUI gamegui = new GameGUI();
	public GameObject highlightSparkles;

	void Start() {
		Debug.Log ("Target started");
		allPokemon = new List<Transform>();
		Debug.Log ("new list initiated");
		targetedPokemon = null;
		//playerTransform = transform;
		Debug.Log ("player position set");
		//AddTargetPokemon();
	}

	void OnEnable() {
		//gamegui.SetChatWindow ("Enabled");
	}

	void Update() {

		if (Input.GetMouseButtonDown(0)){ // when button clicked...
			RaycastHit hit; // cast a ray from mouse pointer:
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// if enemy hit...
			if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("pokemon")){
				//UnHighlightTarget();
				//gamegui.SetChatWindow("hit");
				TargetPokemon(hit.transform);
			}
		}

		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (activeTarget) {
				UnHighlightTarget();
			}
			TargetPokemon(FindNearestPokemon());
		}
		/*
		if (Input.GetKey(KeyCode.Escape) && GetActiveTarget()){
			SetActiveTarget(false);
		}
		*/
		if (activeTarget) {
			//highlightSparkles.transform.position=targetedPokemon.transform.position;
		}
	}

	private void AddTargetPokemon() {
		foreach(GameObject tmpPokemon in GameObject.FindGameObjectsWithTag("pokemon")){
			AddTarget(tmpPokemon.transform);
		}
	}
	
	private void AddTarget(Transform addThisPokemon) {
		allPokemon.Add(addThisPokemon);
	}
	/*
	private void TargetPokemon(Transform targetThis) {
		targetedPokemon = targetThis;
	}
*/
	public Transform GetTargetPokemon() {
		return targetedPokemon;
	}
	/*
	public GameObject GetHighlightSparkles() {
		return this.highlightSparkles;
	}
	*/
	public Transform GetHighlightSparkles() {
		return this.highlightSparkles.transform;
	}
	private void SortTargetsByDistance() {
		Debug.Log ("Sort By Distance");
		allPokemon.Sort (delegate(Transform t1, Transform t2) {
				return Vector3.Distance (t1.position, playerTransform.position).CompareTo (Vector3.Distance (t2.position, playerTransform.position));
		});
	}

	private void LimitTargetDistance(float limit) {
		foreach (Transform limitTmp in allPokemon) {
			if (Vector3.Distance(limitTmp.position,playerTransform.position) > limit) {
				allPokemon.Remove(limitTmp);
			}
		}
	}

	private Transform FindNearestPokemon() {
		playerTransform = Player.trainer.transform;
		int numFound = allPokemon.Count;
		if (numFound == 0) {
			AddTargetPokemon();
		}
		//LimitTargetDistance(float 25.0)
		numFound = allPokemon.Count;
		SortTargetsByDistance ();
		//gamegui.SetChatWindow (numFound + " wild pokemon found");

		return allPokemon [0];
	}

	public void TargetPokemon(Transform targetThis) {
		Debug.Log ("Target Pokemon");
		if (targetedPokemon != null) {
			UnHighlightTarget ();
		}
		SetTarget(targetThis);
		gamegui.SetChatWindow("Targeted " + targetThis.name);
		HighlightTarget();
		//gamegui.SetChatWindow ("Position: " + targetedPokemon.GetInstanceID().ToString());
	}

	public void HighlightTarget() {
		//gamegui.SetChatWindow ("highlighting");
		highlightSparkles = (GameObject)Instantiate (Resources.Load ("ReturnEffect"));
		SetActiveTarget (true);
	}

	public void UnHighlightTarget() {
		//gamegui.SetChatWindow ("un highlighting");
		targetedPokemon = null;
		SetActiveTarget (false);
		/*
		if (highlightSparkles != null) {
			Destroy (highlightSparkles);
		}
		*/
	}

	public Transform GetTarget() {
		return targetedPokemon;
	}

	public void SetTarget(Transform newTarget) {
		targetedPokemon = newTarget;
	}

	public bool GetActiveTarget() {
		return activeTarget;
	}

	public void SetActiveTarget(bool status) {
		activeTarget = status;
	}

}