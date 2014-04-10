using UnityEngine;
using System.Collections;

static public class GUImgr{
	public static Texture2D blank;
	public static Texture2D hp;
	public static Texture2D pp;
	public static Texture2D xp;
	public static Texture2D gradRight;
	public static Texture2D gradLeft;
	public static Texture2D gradDown;

	public static void DrawBar(Rect rect, float value, Texture2D texture){
		Rect subRect = rect;
		subRect.width = rect.width*value;
		GUI.DrawTexture(subRect, texture);
		subRect.x = rect.x+rect.width*value;
		subRect.width = rect.width*(1-value);
		GUI.DrawTexture(subRect, blank);
	}

	public static void Start(){
		blank = new Texture2D(1,1);
		blank.SetPixel(0,0, new Color(0.2f,0.2f,0.2f,0.5f));
		blank.Apply();
		
		hp = new Texture2D(1,1);
		hp.SetPixel(0,0, new Color(1,0.1f,0.1f,0.8f));
		hp.Apply();
		
		pp = new Texture2D(1,1);
		pp.SetPixel(0,0, new Color(0,0.8f,0,0.8f));
		pp.Apply();
		
		xp = new Texture2D(1,1);
		xp.SetPixel(0,0, new Color(0.2f,0.5f,1,0.8f));
		xp.Apply();
		
		gradRight = new Texture2D(100,1);
		gradLeft = new Texture2D(100,1);
		gradDown = new Texture2D(1,100);
		for(int x=0; x<100; x++){
			float color = (float)x /100;
			gradRight.SetPixel(x,0,new Color(1,1,1,1-color));
			gradLeft.SetPixel(x,0,new Color(1,1,1,color));
			gradDown.SetPixel(0,x,new Color(1,1,1,color));
		}
		gradRight.Apply();	gradRight.wrapMode = TextureWrapMode.Clamp;
		gradLeft.Apply();	gradLeft.wrapMode = TextureWrapMode.Clamp;
		gradDown.Apply();	gradDown.wrapMode = TextureWrapMode.Clamp;
	}
}