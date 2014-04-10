#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;
#pragma warning disable 0219, 0414

// This script deals with termination of the Wwise audio engine.  
// It must be present on one Game Object that gets destroyed last in the game.
// It must be executed AFTER any other monoBehaviors that use AkSoundEngine.
// For more information about Wwise initialization and termination see the Wwise SDK doc:
// Wwise SDK | Sound Engine Integration Walkthrough | Initialize the Different Modules of the Sound Engine 
// and also, check AK::SoundEngine::Init & Term.
public class AkGlobalSoundEngineTerminator : MonoBehaviour
{
	static private AkGlobalSoundEngineTerminator ms_Instance = null;

	void Awake()
	{
		if (ms_Instance != null)
            return; //Don't init twice

		DontDestroyOnLoad(this);
		ms_Instance = this;
		// Do nothing. AkGlobalSoundEngineTerminator handles sound engine initialization.
	}

	void OnDestroy()
    {
		Terminate();
    }

	void OnApplicationQuit()
	{
		// Do nothing. Called before OnDestroy.
	}
	
	void Terminate()
	{
		if (ms_Instance == null)
		{
            return; //Don't term twice
        }

        // NOTE: Do not check AkGlobalSoundEngine.IsInitialized()
        //  since its OnDestroy() has been called first in the project exec priority list.
		if (AkSoundEngine.IsInitialized())
		{
            AkSoundEngine.Term();
            // NOTE: AkCallbackManager needs to handle last few events after sound engine terminates
            // So it has to terminate after sound engine does.
            AkCallbackManager.Term();
		}

		ms_Instance = null;

	}
	 
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
