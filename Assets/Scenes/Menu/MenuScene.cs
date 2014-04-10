using UnityEngine;
using System.Collections;

public class MenuScene : MonoBehaviour {
	public GUISkin GuiSkin = null;
	public Texture2D title = null;
	float startTime = 0;
	enum MenuWindow {Main, New};
	MenuWindow currentWindow = MenuWindow.Main;
	MenuWindow newWindow = MenuWindow.Main;
	bool click = true;

	void Start(){
	}

	void Update(){
		startTime += Time.deltaTime;
		if (!GameObject.Find("MusicBox").audio.isPlaying){
			GameObject.Find("MusicBox").audio.Play();
			DontDestroyOnLoad(GameObject.Find("MusicBox"));
		}
		if (!Input.GetMouseButton(0))	click = false;

		currentWindow = newWindow;
	}

	void OnGUI(){
		GUI.skin = GuiSkin;

		switch(currentWindow){
		case MenuWindow.Main: MainWindow(); break;
		case MenuWindow.New: NewWindow(); break;
		}
	}

	void MainWindow(){
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		GUI.skin.label.fontSize = 40;
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;

		GUI.DrawTexture(new Rect(Screen.width-500,0,500, 140), GUImgr.gradLeft);
		GUI.DrawTexture(new Rect(Screen.width-400,0,400,128), title);

		float xpos = Screen.width-300;
		float ypos = 200;

		if (mx>xpos && my>ypos && my<ypos+50){
			GUI.DrawTexture(new Rect(xpos,ypos,300, 50), GUImgr.gradLeft);
		}
		GUI.Label(new Rect(xpos, ypos, 2000,50), "Continue");
		
		xpos+=50; ypos+=50;
		if (mx>xpos && my>ypos && my<ypos+50){
			GUI.DrawTexture(new Rect(xpos,ypos,300, 50), GUImgr.gradLeft);
			if (Input.GetMouseButton(0) && !click){
				newWindow = MenuWindow.New;
				click = true;
			}
		}
		GUI.Label(new Rect(xpos, ypos, 2000,50), "New Game");
	}

	void NewWindow(){
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		GUI.skin.label.fontSize = 40;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0,00, Screen.width,50), "Select Combat System");

		float xpos = Screen.width/2;
		float ypos = 100;
		if ((mx>xpos && my>ypos && my<ypos+50)){
			GUI.DrawTexture(new Rect(xpos,ypos,Screen.width/2, 50), GUImgr.gradLeft);
			if (Input.GetMouseButton(0) && !click){
				PlayerPrefs.SetInt("CombatSystem", 0);
				Application.LoadLevel("GameStart");
				click = true;
			}
		}
		GUI.Label(new Rect(xpos, ypos, Screen.width/2,50), "NXT Action");

		ypos = Screen.height/2;
		if (mx>xpos && my>ypos && my<ypos+50){
			GUI.DrawTexture(new Rect(xpos,ypos,Screen.width/2, 50), GUImgr.gradLeft);
			if (Input.GetMouseButton(0) && !click){
				PlayerPrefs.SetInt("CombatSystem", 1);
				Application.LoadLevel("GameStart");
				click = true;
			}
		}
		GUI.Label(new Rect(xpos,ypos, Screen.width/2,50), "Classical");

		xpos = 200;
		ypos = Screen.height-50;
		if (mx<xpos && my>ypos){
			GUI.DrawTexture(new Rect(0,ypos,200,50), GUImgr.gradRight);
			if (Input.GetMouseButton(0) && !click){
				newWindow = MenuWindow.Main;
				click = true;
			}
		}
		GUI.Label(new Rect(25, ypos, 175,50), "Cancel");

		GUI.skin.label.fontSize = 15;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;

		GUI.Label(new Rect(Screen.width/2, 150, Screen.width/2-20,500),
		          "Directly control your pokemon in fast pased action combat. Combat is real time and pokemon " +
		          "moves are hotkeyed. This is one of the major additions of Pokemon NXT and how the game is " +
		          "supposed to be played.");

		GUI.Label(new Rect(Screen.width/2, Screen.height/2+50, Screen.width/2-20,500), 
		          "Can't get to grips with the new combat system? Not to worry, you can use the classical " +
		          "turn based system. NXT rebalancing and other additions are still implimented but the " +
		          "game will feel more like a classic pokemon game.");

		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, 40, Screen.width,25), "This can be changed at anytime in-game.");

	}
}