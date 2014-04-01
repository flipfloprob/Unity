using UnityEngine;
using System.Collections;

public class MapCamera : MonoBehaviour {
	Vector3 focus = Vector3.zero;
	float zoom = 500;

	void Update(){
		transform.position = focus;
		transform.rotation = Quaternion.Euler(60,0,0);
		transform.Translate(0,0,-zoom);

		zoom *= 1-Input.GetAxis("Mouse ScrollWheel");
		zoom = Mathf.Clamp(zoom,100,500);

		focus += Vector3.forward*Input.GetAxis("Vertical")*Time.deltaTime*zoom;
		focus += Vector3.right*Input.GetAxis("Horizontal")*Time.deltaTime*zoom;

		focus.x = Mathf.Clamp(focus.x,-400,400);
		focus.z = Mathf.Clamp(focus.z,-500,200);
	}
}