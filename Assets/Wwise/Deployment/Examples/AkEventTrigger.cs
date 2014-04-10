#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;

//Example of an event triggered by a Collider trigger.  
//For more information about PostEvent (which you should know by heart...) check the Wwise SDK Doc about AK::SoundEngine::PostEvent.
[RequireComponent(typeof(Collider))]
public class AkEventTrigger : MonoBehaviour 
{
	public string enterEventName = "";
	public string exitEventName = "";
	public bool playOnOtherObject = true;
	public GameObject soundEmitterObject = null;
	
	void OnTriggerEnter(Collider other)
	{		
		if (enterEventName == "")
			return;
		
		GameObject obj = soundEmitterObject;
		if (obj == null)
		{
			if(playOnOtherObject)
				obj = other.gameObject;
			else
				obj = gameObject;
		}
				
		AkSoundEngine.PostEvent(enterEventName, obj);
	}

	void EventFinished(object in_cookie, AkCallbackType out_type, object out_info)
	{
		print("Finished");
	}
	
	void OnTriggerExit(Collider other)
	{
		if (exitEventName == "")	
			return;
		
		GameObject obj = soundEmitterObject;
		if (obj == null)
		{
			if(playOnOtherObject)
				obj = other.gameObject;
			else
				obj = gameObject;
		}
		
		AkSoundEngine.PostEvent(exitEventName,  obj);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
