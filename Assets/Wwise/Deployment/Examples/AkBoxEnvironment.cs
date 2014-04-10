#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;

//This is an example of environement management with Wwise.  
//"Environments" are synonymous with "reverbs" except that Wwise doesn't limit that to the Reverb effect.
//This component will set its effect on all AkEnviromentAware objects that enter it.  There is a ramp-up
//at the edge of the box.  This is a quick implementation, as an example...  I'm sure you can do better :)
public class AkBoxEnvironment : AkAuxSend
{
    //This function is called by AkEnvironmentAware::UpdateEnv to transform the position of the object into
    //an environment percentage.
	public override float GetAuxSendValueForPosition(Vector3 in_pos)
	{
		Vector3 relativePos = in_pos - transform.position;
		relativePos.x = Math.Abs(relativePos.x);
		relativePos.y = Math.Abs(relativePos.y);
		relativePos.z = Math.Abs(relativePos.z);
		
		Vector3 halfSize = transform.lossyScale/2;
		relativePos = Vector3.Min(relativePos, halfSize);
		Vector3 limitCorner = halfSize - new Vector3(rollOffDistance, rollOffDistance, rollOffDistance);
		
		//Check if the gameObject is in the central section, where 100% of the environment is heard
		//If not, do some kind of linear interpolation
		float val = rollOffDistance;
		float limit = 0.0f;
		if (relativePos.x > limitCorner.x)
		{
			val = relativePos.x;
			limit = limitCorner.x;
		}
		else if(relativePos.z > limitCorner.z)
		{
			val = relativePos.z;
			limit = limitCorner.z;
		}		
		else if(relativePos.y > limitCorner.y)		
		{
			val = relativePos.y;
			limit = limitCorner.y;
		}
		
		if(limit == 0.0f)
			return 1.0f;
		
		//Inside the roll-off zone, interpolate.
		return 1.0f - ((val - limit)/rollOffDistance);				
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
