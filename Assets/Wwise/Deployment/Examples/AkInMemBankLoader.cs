#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class AkInMemBankLoader : MonoBehaviour
{
	public string bankName = "";
	public bool isLocalizedBank = false;

	private static WWW ms_www;
	private static GCHandle ms_pinnedArray;
	private static IntPtr ms_pInMemoryBankPtr = IntPtr.Zero;
	[HideInInspector]
	public static uint ms_bankID = AkSoundEngine.AK_INVALID_BANK_ID;

	private const int WaitMs = 50;

	void Start()
	{
		if (isLocalizedBank)
		{
			LoadLocalizedBank(bankName);
		}
		else
		{
			LoadNonLocalizedBank(bankName);
		}
	}

	public static AKRESULT LoadNonLocalizedBank(string in_bankFilename)
	{
		string bankPath = Path.Combine(AkBankPath.GetPlatformBasePath(), in_bankFilename);

		return DoLoadBank(bankPath);
	}
	
	public static AKRESULT LoadLocalizedBank(string in_bankFilename)
	{
		string bankPath = Path.Combine(Path.Combine (AkBankPath.GetPlatformBasePath(), AkGlobalSoundEngineInitializer.GetCurrentLanguage()), in_bankFilename);
		
		return DoLoadBank(bankPath);
	}

	private static AKRESULT DoLoadBank(string in_bankPath)
	{
		ms_www = new WWW(in_bankPath);
		while( ! ms_www.isDone )
		{
#if ! UNITY_METRO
			System.Threading.Thread.Sleep(WaitMs);
#endif // #if ! UNITY_METRO
		}

		uint in_uInMemoryBankSize = 0;
		try
		{
			ms_pinnedArray = GCHandle.Alloc(ms_www.bytes, GCHandleType.Pinned);
			ms_pInMemoryBankPtr = ms_pinnedArray.AddrOfPinnedObject();
			in_uInMemoryBankSize = (uint)ms_www.bytes.Length;	
		}
		catch
		{
			return AKRESULT.AK_Fail;
		}
		
		AKRESULT result = AkSoundEngine.LoadBank(ms_pInMemoryBankPtr, in_uInMemoryBankSize, out ms_bankID);
		
		return result;
	}

	void OnDestroy()
	{
		if (ms_pInMemoryBankPtr != IntPtr.Zero)
		{
			AKRESULT result = AkSoundEngine.UnloadBank(ms_bankID, ms_pInMemoryBankPtr);
			if (result == AKRESULT.AK_Success)
			{
				ms_pinnedArray.Free();	
			}
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
