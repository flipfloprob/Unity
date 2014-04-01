using UnityEngine;
using System.Collections;

public class TimeMgr : MonoBehaviour {
	public enum DayNames {Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday};
	public static int day = 0;
	public static int hour = 12;
	public static float minuite = 0;

	void Update(){
		minuite += Time.deltaTime;
		if (minuite>60){
			hour+=(int)(minuite/60);
			minuite=minuite%60;
			day+=hour/24;
			hour=hour%24;
		}

		float temp = ((float)hour+minuite/60) /12*Mathf.PI;
		transform.position  = Camera.main.transform.position;
		transform.position += new Vector3(Mathf.Sin(temp), -Mathf.Cos(temp), -0.5f) * 5000;
		transform.rotation = Quaternion.LookRotation(Camera.main.transform.position-transform.position);
		light.intensity = Mathf.Clamp01(0.1f-Mathf.Cos(temp)*0.6f);

		Camera.main.GetComponent<Skybox>().material.SetFloat("_Blend", Mathf.Clamp01(Mathf.Cos(temp)+0.5f));


		Color ambient = new Color(0.2f,0.4f,0.5f);
		ambient.r = Mathf.Clamp01(ambient.r*(1-Mathf.Cos(temp)));
		ambient.g = Mathf.Clamp01(ambient.g*(1-Mathf.Cos(temp)));
		ambient.b = Mathf.Clamp01(ambient.b*(1-Mathf.Cos(temp)));
		
		RenderSettings.fogColor = ambient;
		Camera.main.backgroundColor = ambient;
	}
}