#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

//Sets a switch value on a game object when the collider is triggered.
[RequireComponent(typeof(Collider))]
public class AkSwitchTrigger : MonoBehaviour 
{
	public string switchGroup;
	public string switchValue;
	public bool setOnOtherObject = true;
	public GameObject switchTargetObject = null;
	
	
	void OnTriggerEnter(Collider other)
	{		
		GameObject obj = switchTargetObject;
		if (obj == null)
		{
			if(setOnOtherObject)
				obj = other.gameObject;
			else
				obj = gameObject;
		}
		
		AkSoundEngine.SetSwitch(switchGroup, switchValue, obj);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
