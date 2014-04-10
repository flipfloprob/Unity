#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System;

//This component is an example of a way to load sound banks in background.
[RequireComponent(typeof(BoxCollider))]
public class AkBankLoadTrigger : MonoBehaviour 
{
	public string bankName = "";
	
	private uint m_BankID;
	
	void OnTriggerEnter(Collider other)
	{
        AkSoundEngine.LoadBank(bankName, BankCallback, null, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
	}	
	
	void OnTriggerExit(Collider other)
	{
		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
        AkSoundEngine.UnloadBank(m_BankID, in_pInMemoryBankPtr, BankCallback, null);
	}
    
    void BankCallback(uint in_bankID, IntPtr in_pInMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie)
    {
        //The bank has completed loading or unloading.  This trigger doesn't care about that, but you could do something...
    }
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
