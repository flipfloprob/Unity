using UnityEngine;
using System.Collections;

public class InitScene : MonoBehaviour {
	public Texture2D splash = null;
	float startTime = 0;

	const float disclaimerTime = 5;
	const float splashTime = 3;

	void Update(){
		startTime += Time.deltaTime;
		if (startTime>disclaimerTime+splashTime){
			Application.LoadLevel("Menu");
		}
	}
	
	void OnGUI(){
		if (startTime<disclaimerTime){
			float intens = 1;
			if (startTime<1)	intens = startTime;
			if (startTime>disclaimerTime-1)	intens = disclaimerTime-startTime;
			GUI.color = new Color(intens,intens,intens,1);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			GUI.skin.label.fontSize = 20;
			GUI.skin.label.normal.textColor = Color.white;
			GUI.Label(new Rect(50,50,Screen.width-100,100), "THIS GAME CONTAINS MATERIAL WHICH IS COPYRIGHT NINTENDO .PLC AND GAMEFREAK .INC");

			GUI.skin.label.fontSize = 10;
			GUI.Label(new Rect(50,150,Screen.width-100,Screen.height-200), 
			          "Pokemon, Pokemon and the Pokemon character designs within this game are all Copyright Nintendo and Gamefreak. This game is in no way endorsed or affliated with Gamefreak and Nintendo, and is published under FAIR USE terms on the understanding that it currently provides a service or product which is yet to be provided by the afformentioned companies. Model data and images within the game MAY have origins at Nintendo or Gamefreak, however, their final geometry has been optimised and so is original to the Pokemon NXT project. This game is released by individuals in the United Kingdom under the Copyright, Designs and Patents Act 1988, Chapter III, Exception 76 (which permits non-profit adaptions of copyrighted content for public distribution). Players from other countries should check their local laws permit this content before continuing.\n\nThe Pokemon NXT project does not generate a profit. We will compile a financial report with evidence if the need becomes apparent, or upon request.\n\nWe are very much interested in discussing this project with Nintendo and if they were to make a similar game for the PC and Mac it'd probably be better. Until then we are providing something to the fans that Nintendo have yet to do.");
		}else{
			float intens = 1;
			if (startTime<disclaimerTime+1)	intens = startTime-disclaimerTime;
			if (startTime>splashTime+disclaimerTime-1)	intens = disclaimerTime+splashTime-startTime;
			GUI.color = new Color(intens,intens,intens,1);
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), splash);
			return;
		}
	}
}
