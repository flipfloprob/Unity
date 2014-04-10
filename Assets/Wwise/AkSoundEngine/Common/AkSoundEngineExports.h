//////////////////////////////////////////////////////////////////////
//
// AkSoundEngineExports.h
//
// Export/import DLL macro.
//
// Copyright (c) 2006 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////
#ifndef _AK_SOUND_ENGINE_EXPORT_H_
#define _AK_SOUND_ENGINE_EXPORT_H_

#if ! defined(__ANDROID__) && ! defined(__PPU__) && ! defined(__SPU__) && ! defined(__APPLE__)
	#ifdef AKSOUNDENGINEDLL_EXPORTS
		#define AKSOUNDENGINEDLL_API __declspec(dllexport)
	#else
		#define AKSOUNDENGINEDLL_API __declspec(dllimport)
	#endif
#else
	#ifdef AKSOUNDENGINEDLL_EXPORTS
		#define AKSOUNDENGINEDLL_API 
	#else
		#define AKSOUNDENGINEDLL_API 
	#endif
#endif // #ifndef __ANDROID__


#endif  //_AK_SOUND_ENGINE_EXPORT_H_
