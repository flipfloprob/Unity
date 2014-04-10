#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;

//This component is the conceptual equivalent of the Reverb zone.  However, any effect can be used.  This is defined in the Wwise project.
//It simply demonstrates one way to manage multiple environements. 
//All the real meat is in AkEnvironementAware.cs.  This class could be replaced by a simple tag on a collider.
public class AkAuxSend : MonoBehaviour 
{
	public string auxBusName;
	public float rollOffDistance;
	private uint m_auxBusID;
	
	public uint GetAuxBusID()
	{
		return m_auxBusID;
	}
	
	public virtual float GetAuxSendValueForPosition(Vector3 in_pos)
	{
		return 1.0f;
	}
	
	void Awake()
	{
		//Cache the ID to avoid repetitive calls to GetIDFromString that will give the same result.
		m_auxBusID = AkSoundEngine.GetIDFromString(auxBusName);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
