using UnityEngine;
using System.Collections;

public class CinematicEffect : MonoBehaviour {
	static Texture2D screenEdgeShadow;

	void Start(){
		int res = 1024;
		screenEdgeShadow = new Texture2D(res,res);
		for(int x=0; x<res; x++)	for(int y=0; y<res; y++){
			float xd = (float)(x-res/2)/(float)(res/2);
			float yd = (float)(y-res/2)/(float)(res/2);
			float dist = xd*xd + yd*yd;
			dist = Mathf.Sqrt(dist);
			if (dist<1){
				screenEdgeShadow.SetPixel(x,y,Color.clear);
			}else{
				float fade = (dist-1)*2;
				screenEdgeShadow.SetPixel(x,y,new Color(0,0,0,Mathf.Clamp01(fade)));
			}
		}
		screenEdgeShadow.Apply();
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(0,0, Screen.width,Screen.height), screenEdgeShadow);
	}
}
