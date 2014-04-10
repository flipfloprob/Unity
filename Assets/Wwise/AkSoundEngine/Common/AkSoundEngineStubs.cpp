//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"

#ifdef AK_METRO
	#define AK_IMPLEMENT_THREAD_EMULATION
	#include <AK/Tools/Win32/ThreadEmulation.h>
#endif // #ifdef AK_METRO

#include "AkSoundEngineStubs.h"
#include <AK/Tools/Common/AKAssert.h>
#if defined (__APPLE__)
	#include <malloc/malloc.h>
	#include <sys/mman.h>
	#include <AK/Tools/Mac/AkPlatformFuncs.h>
#else
	#if ! defined (__PPU__) && ! defined (__SPU__)
		#include <stdlib.h>
	#else
		#include <ppu/include/stdlib.h>
	#endif // #if ! defined (__PPU__) && ! defined (__SPU__)
#endif // #if defined (__APPLE__)

#include <AK/SoundEngine/Common/AkMemoryMgr.h>
#include <AK/SoundEngine/Common/IAkStreamMgr.h>
#include "AkDefaultIOHookBlocking.h"
#ifndef AK_OPTIMIZED
	#include <AK/Comm/AkCommunication.h>
#endif // #ifndef AK_OPTIMIZED

#include <string>

// Plug-ins
#include <AK/Plugin/AllPluginsRegistrationHelpers.h>

#ifdef AK_ANDROID
#include "../Android/jni/AkUnityAndroidIO.h"
extern JavaVM* java_vm;
#endif // #ifdef AK_ANDROID

#ifdef AK_IOS
#include "AkCallbackSerializer.h"
#endif // #ifdef AK_IOS

#ifdef AK_XBOXONE
#include <xaudio2.h>
#endif

// Defines.

// Default memory manager settings.
#define COMM_POOL_SIZE			(256 * 1024)
#define COMM_POOL_BLOCK_SIZE	(48)

#if defined (WIN32) || defined (WIN64) || defined (_XBOX_VER)
	#define AK_PLATFORM_PATH_SEPARATOR '\\'
#else
	#define AK_PLATFORM_PATH_SEPARATOR '/'
#endif // #if defined (WIN32) || defined (WIN64) || defined (_XBOX_VER)

namespace AK
{
	void * AllocHook( size_t in_size )
	{
		return malloc( in_size );
	}
	void FreeHook( void * in_ptr )
	{
		free( in_ptr );
	}
#if defined (WIN32) || _XBOX_VER >= 200
	void * VirtualAllocHook(
		void * in_pMemAddress,
		size_t in_size,
		DWORD in_dwAllocationType,
		DWORD in_dwProtect
		)
	{
#if defined(AK_METRO)
		return malloc( in_size );
#else
		return VirtualAlloc( in_pMemAddress, in_size, in_dwAllocationType, in_dwProtect );
#endif // #if defined(AK_METRO)

	}
	void VirtualFreeHook( 
		void * in_pMemAddress,
		size_t in_size,
		DWORD in_dwFreeType
		)
	{
#if defined(AK_METRO)
		free( in_pMemAddress );
#else
		VirtualFree( in_pMemAddress, in_size, in_dwFreeType );
#endif // #if defined(AK_METRO)
	}
#endif // #if defined (WIN32) || _XBOX_VER >= 200

#ifdef AK_XBOXONE
	void * APUAllocHook( 
		size_t in_size,				///< Number of bytes to allocate.
		unsigned int in_alignment	///< Alignment in bytes (must be power of two, greater than or equal to four).
		)
	{
		void * pReturn = nullptr;
		ApuAlloc( &pReturn, NULL, (UINT32) in_size, in_alignment );
		return pReturn;
	}

	void APUFreeHook( 
		void * in_pMemAddress	///< Virtual address as returned by APUAllocHook.
		)
	{
		ApuFree( in_pMemAddress );
	}
#endif
}

CAkDefaultIOHookBlocking g_lowLevelIO;


namespace 
{
	// If in Unity Editor, user didn't append path-separator at the end of the 
	// soundbank folder path, add it automatically before concatenate it with 
	// soundbank filename.
	void SafeAppendTrailingPathSeparator(AkOSChar* io_folderPath)
	{
		size_t pathLen = AKPLATFORM::OsStrLen(io_folderPath);
		bool isZeroLenPath = ( pathLen == 0 || io_folderPath == NULL);
		if (isZeroLenPath) 
		{
			return;
		}

		AkOSChar* cstringEnd = io_folderPath + pathLen;
		AkOSChar trailingChar = *(cstringEnd-1);
		bool hasTrailingSeparator = (trailingChar == AK_PLATFORM_PATH_SEPARATOR);
		if (hasTrailingSeparator) 
		{
			return;
		}

		// We have a folder path without trailing separator
		// Here we assume input and output path buffer size is big enough
		*cstringEnd = AK_PLATFORM_PATH_SEPARATOR;

		bool isAnsiPath = ( sizeof(AkOSChar) == 1 );
		const AkOSChar CStringEndChar = isAnsiPath ? '\0' : L'0';
		*(cstringEnd+1) = CStringEndChar;

	}
}

//-----------------------------------------------------------------------------------------
// Sound Engine initialization.
//-----------------------------------------------------------------------------------------
extern "C" 
{
	AKRESULT Init( 
		AkMemSettings *     in_pMemSettings,
		AkStreamMgrSettings * in_pStmSettings,
		AkDeviceSettings *  in_pDefaultDeviceSettings,
		AkInitSettings *    in_pSettings,
		AkPlatformInitSettings * in_pPlatformSettings,
		AkMusicSettings *	in_pMusicSettings
		)
	{
		// Check required arguments.
		if ( !in_pMemSettings ||
			 !in_pStmSettings ||
			 !in_pDefaultDeviceSettings )
		{
			AKASSERT( !"Invalid arguments" );
			return AK_InvalidParameter;
		}

		// Create and initialise an instance of our memory manager.
		if ( AK::MemoryMgr::Init( in_pMemSettings ) != AK_Success )
		{
			AKASSERT( !"Could not create the memory manager." );
			return AK_Fail;
		}

		// Create and initialise an instance of the default stream manager.
		if ( !AK::StreamMgr::Create( *in_pStmSettings ) )
		{
			AKASSERT( !"Could not create the Stream Manager" );
			return AK_Fail;
		}

		// Create an IO device.
#ifdef AK_ANDROID
		if (g_assetManager == NULL)
		{
			AKASSERT( !"Could not find Android asset manager" );
			return AK_Fail;
		}

		g_lowLevelIO.SetAssetManager(g_assetManager);
#endif 

		if ( g_lowLevelIO.Init( *in_pDefaultDeviceSettings ) != AK_Success )
		{
			AKASSERT( !"Cannot create streaming I/O device" );
			return AK_Fail;
		}

#ifdef AK_ANDROID
		in_pPlatformSettings->pJavaVM = java_vm;
#endif

#ifdef AK_IOS
		in_pPlatformSettings->interruptionCallback = AkCallbackSerializer::AudioInterruptionCallbackFunc;
#endif // #ifdef AK_IOS

		// Initialize sound engine.
		if (  AK::SoundEngine::Init( in_pSettings, in_pPlatformSettings ) != AK_Success )
		{
			AKASSERT( !"Cannot initialize sound engine" );
			return AK_Fail;
		}

		// Initialize music engine.
		if ( AK::MusicEngine::Init( in_pMusicSettings ) != AK_Success )
		{
			AKASSERT( !"Cannot initialize music engine" );
			return AK_Fail;
		}

#ifndef AK_OPTIMIZED
	#ifdef AK_XBOXONE
		try
		{
			// Make sure networkmanifest.xml is loaded by instantiating a Microsoft.Xbox.Networking object.
			auto secureDeviceAssociationTemplate = Windows::Xbox::Networking::SecureDeviceAssociationTemplate::GetTemplateByName( "WwiseDiscovery" );
		}
		catch( Platform::Exception ^e )
		{
			AKPLATFORM::OutputDebugMsg( "Wwise network sockets not found in manifest file. Profiling will be impossible." );
		}
	#endif

	#ifndef AK_METRO
		// Initialize communication.
		AkCommSettings settingsComm;
		AK::Comm::GetDefaultInitSettings( settingsComm );
		if ( AK::Comm::Init( settingsComm ) != AK_Success )
		{
			AKASSERT( !"Cannot initialize music communication" );
		}
	#endif // #ifndef AK_METRO
#endif // AK_OPTIMIZED
		
		// Register plugins
		if ( AK::SoundEngine::RegisterAllPlugins( ) != AK_Success )
		{
			AKASSERT( !"Error while registering plug-ins" );
			return AK_Fail;
		}

		return AK_Success;
	}

	//-----------------------------------------------------------------------------------------
	// Sound Engine termination.
	//-----------------------------------------------------------------------------------------
	void Term( )
	{
		if (!AK::SoundEngine::IsInitialized())
			return;
		
		AK::SoundEngine::StopAll();

#ifndef AK_OPTIMIZED
	#ifndef AK_METRO
		AK::Comm::Term();
	#endif // #ifndef AK_METRO
#endif // AK_OPTIMIZED

		AK::MusicEngine::Term();

		AK::SoundEngine::Term();

		g_lowLevelIO.Term();
		if ( AK::IAkStreamMgr::Get() )
			AK::IAkStreamMgr::Get()->Destroy();
			
		AK::MemoryMgr::Term();
	}

	void GetDefaultStreamSettings(AkStreamMgrSettings & out_settings)
	{
		AK::StreamMgr::GetDefaultSettings(out_settings);
	}

	void GetDefaultDeviceSettings(AkDeviceSettings & out_settings)
	{
		AK::StreamMgr::GetDefaultDeviceSettings(out_settings);
	}

	void GetDefaultMusicSettings(AkMusicSettings &out_settings)
	{
		AK::MusicEngine::GetDefaultInitSettings(out_settings);
	}

	void GetDefaultInitSettings(AkInitSettings & out_settings)
	{
		AK::SoundEngine::GetDefaultInitSettings(out_settings);
	}

	void GetDefaultPlatformInitSettings(AkPlatformInitSettings &out_settings)
	{
		AK::SoundEngine::GetDefaultPlatformInitSettings(out_settings);
	}

//-----------------------------------------------------------------------------------------
// Access to LowLevelIO's file localization.
//-----------------------------------------------------------------------------------------
#ifndef __ANDROID__
	// File system interface.
	AKRESULT SetBasePath(const wchar_t* in_pszBasePath)
	{
		AkOSChar* basePathOsString = NULL;

		CONVERT_WIDE_TO_OSCHAR( in_pszBasePath, basePathOsString );

		SafeAppendTrailingPathSeparator(basePathOsString);

		return g_lowLevelIO.SetBasePath( basePathOsString );
	}

	AKRESULT SetBankPath(const wchar_t* in_pszBankPath)
	{
		AkOSChar* bankPathOsString = NULL;

		CONVERT_WIDE_TO_OSCHAR( in_pszBankPath, bankPathOsString );
		
		SafeAppendTrailingPathSeparator(bankPathOsString);

		return g_lowLevelIO.SetBankPath( bankPathOsString );
	}

	AKRESULT SetAudioSrcPath(const wchar_t* in_pszAudioSrcPath)
	{
		AkOSChar* audioSrcPathOsString = NULL;

		CONVERT_WIDE_TO_OSCHAR( in_pszAudioSrcPath, audioSrcPathOsString );

		SafeAppendTrailingPathSeparator(audioSrcPathOsString);

		return g_lowLevelIO.SetAudioSrcPath( audioSrcPathOsString );
	}

	AKRESULT SetCurrentLanguage( const wchar_t*	in_pszLanguageName)
	{
		AkOSChar* languageOsString = NULL;

		CONVERT_WIDE_TO_OSCHAR( in_pszLanguageName, languageOsString );
		
		return AK::StreamMgr::SetCurrentLanguage( languageOsString );
	}

#else // #ifndef __ANDROID__

	AKRESULT SetBasePath(AkOSChar* in_pszBasePath)
	{
		SafeAppendTrailingPathSeparator((AkOSChar*)in_pszBasePath);

		g_lowLevelIO.SetBasePath(in_pszBasePath);
		return AK_Success;
	}

	AKRESULT SetBankPath(AkOSChar* in_pszBankPath)
	{
		return AK_Success;
	}

	AKRESULT SetAudioSrcPath(AkOSChar* in_pszAudioSrcPath)
	{
		return AK_Success;
	}

	AKRESULT SetCurrentLanguage(const AkOSChar* in_pszLanguageName)
	{
		return AK::StreamMgr::SetCurrentLanguage( in_pszLanguageName );
	}

	AKRESULT AddBasePath( AkOSChar* in_pszBasePath	)
	{
		SafeAppendTrailingPathSeparator(in_pszBasePath);
		return g_lowLevelIO.AddBasePath(in_pszBasePath);
	}

#endif // #ifndef __ANDROID__
}
