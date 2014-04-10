//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2006 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

#ifndef AK_SOUND_ENGINE_DLL_H_
#define AK_SOUND_ENGINE_DLL_H_

#include <AK/SoundEngine/Common/AkSoundEngine.h>
#include <AK/MusicEngine/Common/AkMusicEngine.h>
#include <AK/SoundEngine/Common/AkModule.h>
#include <AK/SoundEngine/Common/AkStreamMgrModule.h>

#include "AkSoundEngineExports.h"

namespace AK
{
    namespace AkSoundEngine
    {
		extern "C"
		{
			AKSOUNDENGINEDLL_API AKRESULT Init( 
				AkMemSettings *     in_pMemSettings,
				AkStreamMgrSettings *  in_pStmSettings,
				AkDeviceSettings *  in_pDefaultDeviceSettings,
				AkInitSettings *    in_pSettings,
				AkPlatformInitSettings * in_pPlatformSettings,
				AkMusicSettings *	in_pMusicSettings
				);
			void     Term();

			// File system interface.
#ifndef __ANDROID__
			AKSOUNDENGINEDLL_API AKRESULT SetBasePath(
				const wchar_t*   in_pszBasePath
				);
			AKSOUNDENGINEDLL_API AKRESULT SetBankPath(
				const wchar_t*   in_pszBankPath
				);
			AKSOUNDENGINEDLL_API AKRESULT SetAudioSrcPath(
				const wchar_t*   in_pszAudioSrcPath
				);
			AKSOUNDENGINEDLL_API AKRESULT SetCurrentLanguage(const wchar_t* in_pszLanguageName);
#else
			AKSOUNDENGINEDLL_API AKRESULT SetBasePath(
				AkOSChar*   in_pszBasePath
				);
			AKSOUNDENGINEDLL_API AKRESULT SetBankPath(
				AkOSChar*   in_pszBankPath
				);
			AKSOUNDENGINEDLL_API AKRESULT SetAudioSrcPath(
				AkOSChar*   in_pszAudioSrcPath
				);
			AKSOUNDENGINEDLL_API AKRESULT SetCurrentLanguage(const AkOSChar* in_pszLanguageName);

			AKSOUNDENGINEDLL_API AKRESULT AddBasePath( AkOSChar* in_pszBasePath	);	//Adds a POSIX path
#endif // #ifndef __ANDROID__
			AKSOUNDENGINEDLL_API AKRESULT SetLangSpecificDirName(
				const AkOSChar*   in_pszDirName
				);

			AKSOUNDENGINEDLL_API AKRESULT SetObjectPosition( AkGameObjectID in_GameObjectID, 
				AkReal32 PosX, AkReal32 PosY, AkReal32 PosZ, 
				AkReal32 OrientationX, AkReal32 OrientationY, AkReal32 OrientationZ, 
				AkUInt32 in_ulListenerIndex = AK_INVALID_LISTENER_INDEX);

			AKSOUNDENGINEDLL_API AKRESULT SetListenerPosition( AkReal32 FrontX, AkReal32 FrontY, AkReal32 FrontZ, 
				AkReal32 TopX, AkReal32 TopY, AkReal32 TopZ, 
				AkReal32 PosX, AkReal32 PosY, AkReal32 PosZ, 
				AkUInt32 in_ulListenerIndex);

			AKSOUNDENGINEDLL_API bool IsInitialized();

		}
    }
}

#endif //AK_SOUND_ENGINE_DLL_H_
