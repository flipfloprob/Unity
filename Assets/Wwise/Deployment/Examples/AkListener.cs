#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;

//Add this script on the game object that represent an audio listener.  It will track its position in Wwise.
//More information about Listeners in the Wwise SDK documentation : 
//Wwise SDK ﾻ Sound Engine Integration Walkthrough ﾻ Integrate Wwise Elements into Your Game ﾻ Integrating Listeners 
public class AkListener : MonoBehaviour
{
	public int listenerId = 0;	//Wwise supports up to 8 listeners.  [0-7]
	private Vector3 m_Position;
    private Vector3 m_Top;
	private Vector3 m_Front;
    
    void Update()
    {
        if (m_Position == transform.position && m_Front == transform.forward && m_Top == transform.up)
            return;	//Position didn't change, no need to update.

        m_Position = transform.position;
        m_Front = transform.forward;      
		m_Top = transform.up;

        //Update position
        AkSoundEngine.SetListenerPosition(    
		    transform.forward.x,
            transform.forward.y, 
            transform.forward.z,
		    transform.up.x,
            transform.up.y, 
            transform.up.z,
            transform.position.x, 
            transform.position.y, 
            transform.position.z,
#if UNITY_PS3 && !UNITY_EDITOR
            (ulong)listenerId);
#else
            (uint)listenerId);
#endif // #if UNITY_PS3
    }
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
