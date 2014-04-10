#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;

//Add this script on a Game Object that will emit sounds and that will move during gameplay.
//For more information, see the Wwise SDK doc in the section AK::SoundEngine::SetObjectPosition
[AddComponentMenu("Wwise/Game Object Tracker")]
public class AkGameObjectTracker: AkGameObject
{    
    private Vector3 m_Position;
    private Vector3 m_Forward;
	private bool m_bHasMoved;
	public bool HasMovedInLastFrame() {return m_bHasMoved;}
    
    void Update()
    {
        if (m_Position == transform.position && m_Forward == transform.forward)
		{
			m_bHasMoved = false;
            return;
		}

        m_Position = transform.position;
        m_Forward = transform.forward;    
		m_bHasMoved = true;

        //Update position
        AkSoundEngine.SetObjectPosition(
            gameObject, 
            transform.position.x, 
            transform.position.y, 
            transform.position.z, 
            transform.forward.x,
            transform.forward.y, 
            transform.forward.z);

        //Update Object-Listener distance RTPC, if needed. (TODO)
    }   
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
