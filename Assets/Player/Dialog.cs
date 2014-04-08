using UnityEngine;
using System.Collections;

public class Dialog{
	public static bool inDialog = false;
	public static GameObject NPCobj = null;
	public static string NPCname = "";
	public static string text = "";

	public static bool doneDialog = false;

	public static void GUIWindow(){
		if (!Player.click){
			if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space)){
				doneDialog = true;
			}
		}

		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.DrawTexture(new Rect(0,Screen.height/2,Screen.width/2,Screen.height/2), GUImgr.gradRight);
		GUI.Label(new Rect(20,Screen.height/2+20,Screen.width,40), Dialog.NPCname);
		GUI.Label(new Rect(20,Screen.height/2+100,Screen.width/2-50,Screen.height/2-140), Dialog.text);
	}
}