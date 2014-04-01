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

			GUI.Label(new Rect(50,100,Screen.width-100,Screen.height-200), 
			          "POKÉMON NXT’ CONTAINS COPYRIGHTED MODEL/IMAGE DATA FROM NINTENDO/GAMEFREAK.\n" +
			          "This game has content which Nintendo has made but that the game as an entire entity " +
			          "is NOT approved or endorsed in any way by neither Nintendo, nor Gamefreak. This data " +
			          "has been repachaged and redistributed on the understanding that it has been used for " +
			          "non-profit. Pokémon NXT does not, in any way, ‘muscle’ Nintendo/Gamefreak out " +
			          "of any business. If they were to make a Pokémon PC game, it’d probably be better and " +
			          "we’re 100% open to discussion, but until then, we’re providing something to the fans that " +
			          "Nintendo have yet to.");
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
