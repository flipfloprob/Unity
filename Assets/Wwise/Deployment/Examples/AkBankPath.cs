#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;


// This class is used for returning correct path strings for retrieving various soundbank file locations,
// based on Unity usecases. The main concerns include platform sub-folders and path separator conventions.
// The class makes path string retrieval transparent for all platforms in all contexts. Clients of the class
// only needs to use the public methods to get physically correct path strings after setting flag isToUsePosixPathSeparator. By default, the flag is turned off for non-buildPipeline usecases.
//
// Unity usecases:
// - A BuildPipeline user context uses POSIX path convention for all platforms, including Windows and Xbox360.
// - Other usecases use platform-specific path conventions.

public class AkBankPath
{

	private static string defaultBasePath = Path.Combine("Audio", "GeneratedSoundBanks");	//Default value.  Will be overwritten by the user.
	private static bool isToUsePosixPathSeparator = false;
	private static bool isToAppendTrailingPathSeparator = true;
	
	static AkBankPath ()
	{
		isToUsePosixPathSeparator = false;
	}

	public static void UsePosixPath() { isToUsePosixPathSeparator = true; }
	public static void UsePlatformSpecificPath() { isToUsePosixPathSeparator = false; }
	
	public static void SetToAppendTrailingPathSeparator(bool add) { isToAppendTrailingPathSeparator = add; }

#if !UNITY_METRO
	public static bool Exists(string path)
	{
		DirectoryInfo basePathDir = new DirectoryInfo(path);
		return basePathDir.Exists;
	}
#endif // #if !UNITY_METRO

	public static string GetDefaultPath() { return defaultBasePath; }
	
	public static string GetFullBasePath() 
	{
		// Get full path of base path
#if UNITY_ANDROID && ! UNITY_EDITOR
		// Wwise Android SDK now loads SoundBanks from APKs.
	#if AK_LOAD_BANK_IN_MEMORY
		string fullBasePath = Path.Combine(Application.streamingAssetsPath, AkGlobalSoundEngineInitializer.GetBasePath());
	#else
 		string fullBasePath = AkGlobalSoundEngineInitializer.GetBasePath();
	#endif // #if AK_LOAD_BANK_IN_MEMORY
 		
#elif UNITY_PS3 && ! UNITY_EDITOR
		// NOTE: Work-around for Unity PS3 (up till 3.5.2) bug: Application.streamingAssetsPath points to wrong location: /app_home/PS3_GAME/USRDIR/Raw
		const string StreamingAssetsPath = "/app_home/PS3_GAME/USRDIR/Media/Raw";
		string fullBasePath = Path.Combine(StreamingAssetsPath, AkGlobalSoundEngineInitializer.GetBasePath());
#else
		string fullBasePath = Path.Combine(Application.streamingAssetsPath, AkGlobalSoundEngineInitializer.GetBasePath());
#endif
		LazyAppendTrailingSeparator(ref fullBasePath);
		LazyConvertPathConvention(ref fullBasePath);
		return fullBasePath;
	}
	
	public static string GetPlatformBasePath()
	{
		// Combine base path with platform sub-folder
		string platformBasePath = Path.Combine(GetFullBasePath(), GetPlatformSubDirectory());
		
		LazyAppendTrailingSeparator(ref platformBasePath);

		LazyConvertPathConvention(ref platformBasePath);

		return platformBasePath;
	}

	static public string GetPlatformSubDirectory()
	{
		string platformSubDir = "Undefined platform sub-folder";        
		
#if UNITY_EDITOR_WIN
        // NOTE: Due to a Unity3.5 bug, projects upgraded from 3.4 will have malfunctioning platform preprocessors
		// We have to use Path.DirectorySeparatorChar to know if we are in the Unity Editor for Windows or Mac.
        platformSubDir = "Windows";
#elif UNITY_EDITOR_OSX
		platformSubDir = "Mac";
#elif UNITY_STANDALONE_WIN
		platformSubDir = Path.DirectorySeparatorChar == '/' ? "Mac" : "Windows";
#elif UNITY_STANDALONE_OSX
		platformSubDir = Path.DirectorySeparatorChar == '/' ? "Mac" : "Windows";
#elif UNITY_XBOX360
		platformSubDir = "XBox360";
#elif UNITY_XBOXONE
		platformSubDir = "XBoxOne";
#elif UNITY_IOS
		platformSubDir = "iOS";
#elif UNITY_ANDROID
		platformSubDir = "Android";
#elif UNITY_PS3
		platformSubDir = "PS3";
#elif UNITY_PS4
		platformSubDir = "PS4";
#elif UNITY_METRO
	    platformSubDir = "Windows";
#elif UNITY_PSP2
#if AK_ARCH_VITA_SW
        platformSubDir = "VitaSW";
#elif AK_ARCH_VITA_HW
		platformSubDir = "VitaHW";
#else
		platformSubDir = "VitaSW";
#endif
#endif
        return platformSubDir;
	}

	public static void LazyConvertPathConvention(ref string path)
	{
		if (isToUsePosixPathSeparator)
			ConvertToPosixPath(ref path);
		else
		{
#if !UNITY_METRO
			if (Path.DirectorySeparatorChar == '/')
				ConvertToPosixPath(ref path);
			else
				ConvertToWindowsPath(ref path);
#else
			ConvertToWindowsPath(ref path);
#endif // #if !UNITY_METRO
		}
	} 
	
	public static void ConvertToWindowsPath(ref string path)
    {
        path.Trim();
        path = path.Replace("/", "\\");
        path = path.TrimStart('\\');
    }

	public static void ConvertToWindowsCommandPath(ref string path)
    {
        path.Trim();
        path = path.Replace("/", "\\\\");
		path = path.Replace("\\", "\\\\");
        path = path.TrimStart('\\');
    }    
	
	public static void ConvertToPosixPath(ref string path)
    {
        path.Trim();
        path = path.Replace("\\", "/");
        path = path.TrimStart('\\');
    }
	
	public static void LazyAppendTrailingSeparator(ref string path)
	{
		if ( ! isToAppendTrailingPathSeparator )
			return;
#if !UNITY_METRO
		if ( ! path.EndsWith(Path.DirectorySeparatorChar.ToString()) )
        {
            path += Path.DirectorySeparatorChar;
        }
#else
		if ( ! path.EndsWith("\\") )
        {
            path += "\\";
        }
#endif // #if !UNITY_METRO
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
