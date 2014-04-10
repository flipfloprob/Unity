//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

%module AkSoundEngine	//Names the C# namespace.  Must be the same thing as the name of the DLL.
%csconst(1);		//Generate static const variables in C# for all defines and const variables seen on the C++ side.

%include "../../SWIG/Lib/csharp/wchar.i"				//Treat wchar* as strings in C#.
%include "../../SWIG/Lib/csharp/typemaps.i"

//Remove warning about duplicate functions because of char and wchar use in the interface.
#pragma SWIG nowarn=516	
#pragma SWIG nowarn=844

// Translate custom SWIG command defines into AK defines.
// Order: Must be before any dependent #ifs
// For compatibility with Wwise 2012.2 where <AK/AkPlatforms.h> no longer contains importable defines.
#ifdef NN_PLATFORM_CTR
	#define AK_3DS
#elif defined( XBox360 )
	#define AK_XBOX360
#elif defined( _XBOX_ONE )
	#define AK_XBOXONE
	#define AK_SUPPORT_WCHAR	
#elif defined( WIN32 ) || defined ( WIN64 ) || defined( WINAPI_FAMILY )
	#define AK_WIN
#elif defined( __APPLE__ )
	#define AK_APPLE 
#elif defined (__PPU__) || defined (__SPU__)
	#define AK_PS3
#elif defined( CAFE ) || defined( RVL_OS )
	#define AK_WII_FAMILY
#elif defined( __ANDROID__ )
	#define AK_ANDROID
#elif defined( __native_client__ )
	#define AK_NGP
#elif defined( __SCE__ ) && defined( __arm__ )
	#define AK_VITA
#elif defined(__ORBIS__)
	#define AK_PS4
#endif 

// 64-bit platforms
#if defined(AK_PS4) || defined(AK_XBOXONE)
	#define SWIG_64BIT_PLATFORM
#endif

#if defined (AK_ANDROID) || defined (AK_PS3)
%{
#pragma GCC diagnostic ignored "-Wuninitialized"
#pragma GCC diagnostic ignored "-Wmaybe-uninitialized"
%}
#endif // #if defined (AK_ANDROID) || defined (AK_PS3)

#if defined(AK_XBOXONE)
%{
// Disable "unreferenced formal parameter" (4100) and "potentially uninitialized" (4701) warnings.
#pragma warning(disable: 4100)
#pragma warning(disable: 4701)
%}
#endif // defined(AK_XBOXONE)

%ignore AkEmitterListenerPair;
%ignore AkPolarCoord;
%ignore AkSphericalCoord;
%ignore AkListener;

//This typemap will map the "void*" to a IntPtr on the C# side.
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
	%typemap(ctype) void * "void *"
	%typemap(imtype) void * "IntPtr"
	%typemap(cstype) void * "IntPtr"
	%typemap(csin) void * "$csinput"
	%typemap(in) void * %{ $1 = $input; %}
	%typemap(out) void * %{ $result = $1; %}
	%typemap(csout) void * { return $imcall; }
	%typemap(csvarin, excode=SWIGEXCODE2)  void* %{ set { $imcall;$excode } %}
	%typemap(csvarout, excode=SWIGEXCODE2)  void* %{ get { return $imcall;$excode }%}

// Order: Must be before %include <AK/SoundEngine/Platforms/Windows/AkTypes.h>, for e.g., _w64
#if defined(AK_WIN) || defined(AK_XBOX360) || defined(AK_XBOXONE)
	%include <windows.i>
#endif // #if defined(AK_WIN) || defined(AK_XBOX360) || defined(AK_XBOXONE)

// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
#if defined( AK_3DS )
	%include <AK/SoundEngine/Platforms/3DS/AkTypes.h>
#elif defined( AK_XBOX360 )
	%include <AK/SoundEngine/Platforms/XBox360/AkTypes.h>
#elif defined( AK_XBOXONE )
	%include <AK/SoundEngine/Platforms/XBoxOne/AkTypes.h>
#elif defined( AK_WIN )
	%include <AK/SoundEngine/Platforms/Windows/AkTypes.h>
#elif defined( AK_APPLE )
	#if defined(AK_MAC_OS_X_10_9) || defined(AK_IOS_7)
		%import <sys/cdefs.h>
		%import <sys/_types/_int8_t.h>
		%import <sys/_types/_int16_t.h>
		%import <sys/_types/_int32_t.h>
		%import <sys/_types/_int64_t.h>
		%import <_types/_uint8_t.h>
		%import <_types/_uint16_t.h>
		%import <_types/_uint32_t.h>
		%import <_types/_uint64_t.h>
	#endif // #if defined(AK_MAC_OS_X_10_9) || defined(AK_IOS_7)
	%import <sys/_types.h>
	%import <sys/types.h>
	%import <stdint.h>
	%include <AK/SoundEngine/Platforms/Mac/AkTypes.h>
	%include <AK/SoundEngine/Platforms/POSIX/AkTypes.h>
#elif defined ( AK_PS3 )
	%include <AK/SoundEngine/Platforms/PS3/AkTypes.h>
#elif defined ( AK_PS4 )
	%include <PS4_types.h>
	%include <AK/SoundEngine/Platforms/PS4/AkTypes.h>
#elif defined( AK_WII_FAMILY )
	%include <AK/SoundEngine/Platforms/WiiFamily/AkTypes.h>
#elif defined( AK_VITA )
	%include <AK/SoundEngine/Platforms/Vita/AkTypes.h>
#elif defined( AK_ANDROID )
	#define	__signed signed
	// %import <sys/cdefs.h>
	%import <machine/_types.h>
	%import <sys/_types.h>
	%import <sys/types.h>
	%import <stdint.h>
	%import <AK/SoundEngine/Platforms/POSIX/AkTypes.h>
	%import <AK/SoundEngine/Platforms/Android/AkTypes.h>
#elif defined( AK_NACL )
	%include <AK/SoundEngine/Platforms/nacl/AkTypes.h>
#endif

//Transform calls with AkOSChar properly.  On the C# side, they are simple strings.  They transfer as Unicode to C++ (Windows), then converted to the proper native type.
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>, for e.g., AkExternalSourceInfo::szFile
	%typemap(cstype) AkOSChar* "string"
	%typemap(csin) AkOSChar* "$csinput"
	%typemap(csout) AkOSChar* { return $imcall; }
	%typemap(csvarin, excode=SWIGEXCODE2)  AkOSChar* %{	set { $imcall;$excode } %}
	%typemap(csvarout, excode=SWIGEXCODE2)  AkOSChar* %{ get { return $imcall;$excode } %}
#if defined(AK_WIN) || defined(AK_XBOXONE)
// #ifdef AK_SUPPORT_WCHAR	
	%typemap(ctype) AkOSChar* "wchar_t*"
	%typemap(imtype, inattributes="[MarshalAs(UnmanagedType.LPWStr)]") AkOSChar* "string"
	%typemap(in) AkOSChar* %{ CONVERT_WIDE_TO_OSCHAR($input, $1); %}
	%typemap(out) AkOSChar* %{ CONVERT_OSCHAR_TO_WIDE($1, $result); %}
#else // Current Non-Windows platforms all use char as native char. 
	%typemap(ctype) AkOSChar* "char*"
	%typemap(imtype, inattributes="[MarshalAs(UnmanagedType.LPStr)]") AkOSChar* "string"
	%typemap(in) AkOSChar* %{ $1 = (char*) $input; %}
	%typemap(out) AkOSChar* %{ $result = $1; %}
#endif // #ifdef AK_WIN

// Declaring our constants to avoid getter functions
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
%constant unsigned int		AK_INVALID_PLUGINID					= 0xFFFFFFFF;		
%constant unsigned int		AK_INVALID_GAME_OBJECT				= 0xFFFFFFFF;	
%constant unsigned int		AK_INVALID_UNIQUE_ID				=  0;					
%constant unsigned int		AK_INVALID_RTPC_ID					=  AK_INVALID_UNIQUE_ID;
%constant unsigned int		AK_INVALID_LISTENER_INDEX			= 0xFFFFFFFF;			
%constant unsigned int		AK_INVALID_PLAYING_ID				=  AK_INVALID_UNIQUE_ID;
%constant unsigned int		AK_DEFAULT_SWITCH_STATE				=  0;					
%constant unsigned int		AK_INVALID_POOL_ID					= 0xFFFFFFFF;					
%constant int		AK_DEFAULT_POOL_ID					= -1;					
%constant unsigned int		AK_INVALID_ENV_ID					=  AK_INVALID_UNIQUE_ID;
%constant unsigned int		AK_INVALID_FILE_ID					= 0xFFFFFFFF;			
%constant unsigned int		AK_INVALID_DEVICE_ID				= 0xFFFFFFFF;		
%constant unsigned int		AK_INVALID_BANK_ID					=  AK_INVALID_UNIQUE_ID;
%constant unsigned int		AK_FALLBACK_ARGUMENTVALUE_ID		=  0;					
%constant unsigned int		AK_DEFAULT_PRIORITY          = 50;   
%constant unsigned int		AK_MIN_PRIORITY		        = 0;    
%constant unsigned int		AK_MAX_PRIORITY		        = 100;  
%constant unsigned int		AK_DEFAULT_BANK_IO_PRIORITY = AK_DEFAULT_PRIORITY; 	
%constant double		AK_DEFAULT_BANK_THROUGHPUT  = 1*1024*1024/1000.0;

%ignore AK_INVALID_PLUGINID			;
%ignore AK_INVALID_GAME_OBJECT		;
%ignore AK_INVALID_UNIQUE_ID		;
%ignore AK_INVALID_RTPC_ID			;
%ignore AK_INVALID_LISTENER_INDEX	;
%ignore AK_INVALID_PLAYING_ID		;
%ignore AK_DEFAULT_SWITCH_STATE		;
%ignore AK_INVALID_POOL_ID			;
%ignore AK_DEFAULT_POOL_ID			;
%ignore AK_INVALID_ENV_ID			;
%ignore AK_INVALID_FILE_ID			;
%ignore AK_INVALID_DEVICE_ID		;
%ignore AK_INVALID_BANK_ID			;
%ignore AK_FALLBACK_ARGUMENTVALUE_ID;
%ignore AK_DEFAULT_PRIORITY         ;
%ignore AK_MIN_PRIORITY		        ;
%ignore AK_MAX_PRIORITY		        ;
%ignore AK_DEFAULT_BANK_IO_PRIORITY ;
%ignore AK_DEFAULT_BANK_THROUGHPUT  ;

// Must be before %include <AK/SoundEngine/Common/AkTypes.h>
%rename(IsEqualTo) WwiseObjectIDext::operator==;
// WG-21816
%rename(GetNodeType) WwiseObjectIDext::GetType();

// Include all our public interfaces: Must be below WwiseOBjectIDext operators
%{
#include <AK/SoundEngine/Common/AkTypes.h>
%}
%include <AK/SoundEngine/Common/AkTypes.h>

//%ignore HWND;	//AkPlatformInitSettings has this on Windows, we ignore it for now. REVIEW!  We need to find a way to avoid exposing the specifics of each platforms.
#define __cdecl


%include "arrays_csharp.i"

// Rules for input/output arguments
// Note: typedefs cannot be used in %apply rule name. Use define instead

// AkUInt32 apply aliase
#if defined(AK_WIN) || defined(AK_XBOX360) || defined(AK_XBOXONE) || defined(AK_PS3) || defined(AK_VITA)
	#define ApplyAkUInt32 unsigned long
#else	
	#define ApplyAkUInt32 unsigned int	
#endif // #if defined(AK_WIN) || defined(AK_XBOX360) || defined(AK_PS3)

// AkInt32 apply aliase
#if defined(AK_WIN) || defined(AK_XBOX360) || defined(AK_XBOXONE) || defined(AK_PS3) || defined(AK_VITA)
	#define ApplyAkInt32 long
#else
	#define ApplyAkInt32 int
#endif // #if defined(AK_WIN) || defined(AK_XBOX360) || defined(AK_PS3)

// AkUIntPtr apply aliase
#if defined(AK_WIN) || defined(AK_XBOX360)
	#define ApplyAkUIntPtr __w64 unsigned int
#elif defined(AK_XBOXONE)
	#define ApplyAkUintPtr unsigned __int64
#elif defined(AK_PS3) || defined(AK_PS4)
	#define ApplyAkUIntPtr unsigned long
#else
	#define ApplyAkUIntPtr unsigned int
#endif

// AkReal32 apply aliase
#define ApplyAkReal32 float

// bool and enum arguments
#define ApplyBoolEnum int

// AkUInt8 arguments
#define ApplyAkUInt8 unsigned char

// enum arguments
#define ApplyOutputEnum int

#if defined(AK_PS4)
	#define ApplyAkUInt64 unsigned long
	
	%apply ApplyAkUInt64 { SceKernelCpumask };
#endif // #if defined(AK_PS4)

%apply ApplyAkUIntPtr { AkGameObjectID };

// AkUInt32 arguments
// - as input array
%apply ApplyAkUInt32 INPUT[] { AkUInt32* in_paGameSyncID, AkUInt32*	in_paGameSyncID, AkUniqueID*, AkArgumentValueID*};
// - as output array
%apply ApplyAkUInt32 OUTPUT[] { AkPlayingID* out_aPlayingIDs };
// - as single output
%apply ApplyAkUInt32 &OUTPUT { AkUInt32& out_ruListenerMask, AkStateID& out_rState, AkSwitchStateID& out_rSwitchState, AkBankID& out_bankID};
// - as single input/output
%apply ApplyAkUInt32 &INOUT { AkUInt32&	io_ruNumEnvValues, AkUInt32& io_ruNumItems, AkUInt32& io_ruNumIDs, AkUInt32& io_ruNumSendValues, AkUInt32 &io_uChannelMask, AkUInt32& io_uNumAngles};

// AkInt32 arguments
// - as single output
%apply ApplyAkInt32 *OUTPUT { AkTimeMs*, AkMemPoolId* };
%apply ApplyAkInt32 &OUTPUT { AkInt32& out_iValue };

// AkReal32 arguments
%apply ApplyAkReal32 &OUTPUT { AkReal32&, AkRtpcValue& };
%apply ApplyAkReal32 *INOUT { AkReal32* io_pfSpeakerAngles };
%apply ApplyAkReal32 *INPUT { AkReal32* in_pfSpeakerAngles };

%typemap(in) AkGroupType INPUT { $1 = (AkGroupType)$input; }

// Misc arguments
%apply ApplyBoolEnum &OUTPUT {bool&};
%apply ApplyBoolEnum &INOUT {AK::SoundEngine::Query::RTPCValue_type&};
%apply ApplyAkUInt8 &OUTPUT { AkUInt8& };
%apply ApplyOutputEnum &OUTPUT { AkPanningRule & }; // GetPanningRule()

//Typemap that will route the AkCallbackFunc parameter through the AkCallbackSerializer
//The cookie must point to a CallbackPackage in C#.  The package will have the real C# call back plus the C# cookie.

#ifdef AK_PS3
	%typemap(cstype) AkUInt32 in_ulListenerIndex "ulong"
	%typemap(imtype) AkUInt32 in_ulListenerIndex "ulong"
#endif // #ifdef AK_PS3

#ifdef AK_PS4
	%typemap(cstype) SceKernelCpumask dwAffinityMask "ulong"
	%typemap(imtype) SceKernelCpumask dwAffinityMask "ulong"
	%typemap(csvarout) SceKernelCpumask, ulong 
	%{ 
	get
	{ 
		return (ulong)($imcall); 
	} 
	%} 
#endif // AK_PS4

	//This will force AkCallbackSerializer::EventCallback to be the callback on the C++ side.  Thus, it forces serialization through AkCallbackSerializer
	%typemap(in) AkCallbackFunc %{ $1 = AkCallbackSerializer::EventCallback; %}
	
	//This overrides the void* typemap for parameters named "in_pCookie" on the C# side.  It maps to "object" as input parameter.
	%typemap(cstype) void * in_pCookie "object"
	%typemap(csin) void * in_pCookie "(IntPtr)$csinput.GetHashCode()"
	
	//This maps AkCallbackFunc to AkCallbackManager.EventCallback delegate type on the C# side.
	%typemap(cstype) AkCallbackFunc "AkCallbackManager.EventCallback"
	%typemap(imtype) AkCallbackFunc "IntPtr"
	
	//This typemap triggers when receiving a AkCallbackFunc(AkCallbackManager.EventCallback) as input parameter on the C# side and injects the conversion code.
	%typemap(csin, pre="		in_pCookie = new AkCallbackManager.EventCallbackPackage($csinput, in_pCookie);") AkCallbackFunc "(IntPtr)0"
	
//Typemap that will route the AkBankCallbackFunc parameter through the AkCallbackSerializer
//The cookie must point to a CallbackPackage in C#.  The package will have the real C# call back plus the C# cookie.

	//This will force AkCallbackSerializer::BankCallback to be the callback on the C++ side.  Thus, it forces serialization through AkCallbackSerializer
	%typemap(in) AkBankCallbackFunc %{ $1 = (AkBankCallbackFunc) AkCallbackSerializer::BankCallback; %}
	
	//This overrides the void* typemap for parameters named "in_pCookie" on the C# side.  It maps to "object" as input parameter.
	%typemap(cstype) void * in_pCookie "object"
	%typemap(csin) void * in_pCookie "(IntPtr)$csinput.GetHashCode()"
	
	//This maps AkBankCallbackFunc to AkCallbackManager.EventCallback delegate type on the C# side.
	%typemap(cstype) AkBankCallbackFunc "AkCallbackManager.BankCallback"
	%typemap(imtype) AkBankCallbackFunc "IntPtr"
	
	//This typemap triggers when receiving a AkBankCallbackFunc(AkCallbackManager.BankCallback) as input parameter on the C# side and injects the conversion code.
	%typemap(csin, pre="		in_pCookie = new AkCallbackManager.BankCallbackPackage($csinput, in_pCookie);") AkBankCallbackFunc "(IntPtr)0"
	
//This typemap will automatically add a AkGameObject component to the Unity GameObject if none exist.  This will do the Register/Unregister object without
//the user knowing.  All function using game object will have this code.
// APIs as GetGameObjectFromPlayingID() returns the InstanceID for user to search to get gameObject in game.
// On 64-bit platforms, a GameObjectId should be mapped to a ulong on the C# side.
#ifdef SWIG_64BIT_PLATFORM
	%typemap(cstype, out="ulong") AkGameObjectID "UnityEngine.GameObject"
	%typemap(imtype) AkGameObjectID "ulong"

	// WG-20976 Need to replace it with a Unity-friendly returned type for Win32 and Win64 to share API bindings.
	%typemap(csout) AkGameObjectID { return (ulong)$imcall; }
	
	%typemap(csin, pre="    
		ulong temp$csinput;
		if ( $csinput != null )
		{
			temp$csinput = (ulong)$csinput.GetInstanceID();
			// Note: if AkGameObjectTracker is already attached, the following code will be bypassed.
			if ($csinput.GetComponent<AkGameObject>() == null)
			{
				$csinput.AddComponent<AkGameObject>();
			}
		}
		else
		{
			temp$csinput = unchecked((ulong)-1);
		}
		
		") AkGameObjectID "temp$csinput"
#else
	%typemap(cstype, out="uint") AkGameObjectID "UnityEngine.GameObject"
	%typemap(imtype) AkGameObjectID "uint"

	// WG-20976 Need to replace it with a Unity-friendly returned type for Win32 and Win64 to share API bindings.
	%typemap(csout) AkGameObjectID { return (uint)$imcall; }
	
	%typemap(csin, pre="    
		uint temp$csinput;
		if ( $csinput != null )
		{
			temp$csinput = (uint)$csinput.GetInstanceID();
			// Note: if AkGameObjectTracker is already attached, the following code will be bypassed.
			if ($csinput.GetComponent<AkGameObject>() == null)
			{
				$csinput.AddComponent<AkGameObject>();
			}
		}
		else
		{
			temp$csinput = unchecked((uint)-1);
		}
		
		") AkGameObjectID "temp$csinput"
#endif // SWIG_64BIT_PLATFORM
//This typemap replaces the AkAuxSendArray(in pre-2012.2, AkEnvironmentValue) struct array to a hand-written object on the C# side that takes care of the marshalling.
//Unfortunately, SWIG doesn't seem to handle contiguous arrays of structs.
	%typemap(cstype) AkAuxSendValue* "AkAuxSendArray"
	%typemap(csin) AkAuxSendValue* "$csinput.m_Buffer"
	%typemap(imtype) AkAuxSendValue* "IntPtr"
	
//This typemap replaces the AkSoundPosition* struct array to a hand-written object on the C# side that takes care of the marshalling.
//Unfortunately, SWIG doesn't seem to handle contiguous arrays of structs.
	%typemap(cstype) const AkSoundPosition* "AkPositionArray"
	%typemap(csin) const AkSoundPosition* "$csinput.m_Buffer"
	%typemap(imtype) const AkSoundPosition* "IntPtr"

//This typemap replaces the bitfield enum AkMemPoolAttributes with a simple int.  This avoids a "SWIGTYPE" file.
	%typemap(cstype) AkMemPoolAttributes "int"
	%typemap(csin) AkMemPoolAttributes "$csinput"
	%typemap(ctype) AkMemPoolAttributes "int"
	%typemap(imtype) AkMemPoolAttributes "int"
	%typemap(in) AkMemPoolAttributes %{ $1 = (AkMemPoolAttributes)$input; %}
	%typemap(out) AkMemPoolAttributes %{ $result = (AkMemPoolAttributes)$1; %}
	%typemap(csout) AkMemPoolAttributes { return $imcall; }
	%typemap(csvarin, excode=SWIGEXCODE2)  AkMemPoolAttributes %{ set { $imcall;$excode } %}
	%typemap(csvarout, excode=SWIGEXCODE2)  AkMemPoolAttributes %{ get { return $imcall;$excode } %}

//Handle wchar_t as a platform-dependent type
#ifdef AK_SUPPORT_WCHAR
	%typemap(ctype) wchar_t* "wchar_t*"
	%typemap(imtype, inattributes="[MarshalAs(UnmanagedType.LPWStr)]") wchar_t* "string"
	#ifndef __APPLE__
		%typemap(in) wchar_t* %{ $1 = $input; %}
		%typemap(out) wchar_t* %{ $result = $1; %}
	#else // Apple platform wchar_t is encoded with UTF-32, while Windows uses UTF-16.
		%typemap(in) wchar_t* %{ CONVERT_UTF16_TO_WCHAR($input, $1); %}
		%typemap(out) wchar_t* %{ CONVERT_WCHAR_TO_UTF16($1, $result); %}
	#endif // #ifdef __APPLE__
#endif // #ifdef AK_SUPPORT_WCHAR

//AkAssertHook can't be used.  Just make sure we avoid polluting the interface with a "SWIGTYPE" file.
	%typemap(cstype) AkAssertHook "int"
	%typemap(csin) AkAssertHook "0"
	%typemap(csout) AkAssertHook { return 0; }
	%typemap(csvarout, excode=SWIGEXCODE2)  AkAssertHook %{ get { return 0; }%}
	%typemap(csvarin, excode=SWIGEXCODE2)  AkAssertHook %{ set { }%}

//Transform HWND into HandleRef.  This avoids a "SWIGTYPE" file.
	%typemap(cstype) HWND "HandleRef"
	%typemap(csin) HWND "$csinput"
	%typemap(csout) HWND { return $imcall; }
	%typemap(csvarin, excode=SWIGEXCODE2) HWND %{ set { $imcall;$excode } %}
	%typemap(csvarout, excode=SWIGEXCODE2) HWND %{ get { return new HandleRef(null, IntPtr.Zero); } %}

//These typemaps make array of strings come accross properly.
#ifdef AK_SUPPORT_WCHAR
	// C# marshalling: platform independent
	%typemap(ctype) wchar_t const ** "wchar_t const*";
	%typemap(cstype) wchar_t const ** "string []"
	%typemap(imtype) wchar_t const ** "IntPtr"
	%typemap(csin, pre="			
		//Find the required size
		int size = 0;
		foreach(string s in $csinput)
			size += s.Length + 1;
				
		int sizeofChar = 2;	//Unicode
		IntPtr pMem = Marshal.AllocHGlobal(size * sizeofChar);
		
		//Write the length of array
		Marshal.WriteInt16(pMem, (short)$csinput.Length);
		IntPtr pCurrent = (IntPtr)(pMem.ToInt64() + sizeofChar);
		
		//Copy the strings one after the other.
		foreach(string s in $csinput)
		{
			Marshal.Copy(s.ToCharArray(), 0, pCurrent, s.Length);
			pCurrent = (IntPtr)(pCurrent.ToInt64() + sizeofChar * s.Length);
			Marshal.WriteInt16(pCurrent, 0);	//Null-terminated string
			pCurrent = (IntPtr)(pCurrent.ToInt64() + sizeofChar);
		}		
		", post="	Marshal.FreeHGlobal(pMem);") wchar_t const ** "pMem";
	
	#ifndef __APPLE__
		%typemap(in) wchar_t const ** %{
		   //Create a string array of the proper size
		   unsigned short size = *$input;
		   $input++; // skip size word
		   $1 = (wchar_t**)AkAlloca(size*sizeof(wchar_t*));
		   unsigned long len = 0;
		   //Make all pointers point to the right place.
		   for(unsigned long i = 0; i < size; i++)
		   {
				$1[i] = (wchar_t*)$input + len;
				len += (unsigned long)wcslen($1[i]) + 1;
		   }
		%}
	#else // #ifndef __APPLE__
		%typemap(in) wchar_t const ** %{
		   //Create a string array of the proper size
		   unsigned short size = *$input;
		   AkUtf16* csharpString = (AkUtf16*)$input; 
		   ++csharpString; // skip size word
		   $1 = (wchar_t**)AkAlloca(size*sizeof(wchar_t*));
		   unsigned long len = 0;
		   //Make all pointers point to the right place.
		   for(unsigned long i = 0; i < size; i++)
		   {		   		
				AkUtf16* winWcharString = csharpString + len;
				size_t srcLen = AKPLATFORM::AkUtf16StrLen(winWcharString);
				wchar_t* appleWcharString = NULL;
				CONVERT_UTF16_TO_WCHAR(winWcharString, appleWcharString);
				$1[i] = appleWcharString;

				len += (unsigned long)srcLen + 1;
		   }
		%}
	#endif // #ifndef __APPLE__
	
#endif // #ifdef AK_SUPPORT_WCHAR
	
//Typemaps to make char string array come across PINVOKE properly
	%typemap(ctype) char const ** "wchar_t const*";
	%typemap(cstype) char const ** "string []"
	%typemap(imtype) char const ** "IntPtr"
	%typemap(csin, pre="			
		//Find the required size
		int size = 0;
		foreach(string s in $csinput)
			size += s.Length + 1;
				
		int sizeofChar = 2;	// UNICODE
		IntPtr pMem = Marshal.AllocHGlobal(size * sizeofChar);

		//Write the length of array
		Marshal.WriteInt16(pMem, (short)$csinput.Length);
		IntPtr pCurrent = (IntPtr)(pMem.ToInt64() + sizeofChar);
		
		//Copy the strings one after the other.
		foreach(string s in $csinput)
		{
			Marshal.Copy(s.ToCharArray(), 0, pCurrent, s.Length);
			pCurrent = (IntPtr)(pCurrent.ToInt64() + sizeofChar * s.Length);
			Marshal.WriteInt16(pCurrent, 0);	//Null-terminated string
			pCurrent = (IntPtr)(pCurrent.ToInt64() + sizeofChar);
		}		
		", post="	Marshal.FreeHGlobal(pMem);") char const ** "pMem";
		
	// Only Android uses char-based APIs and Android uses UTF-32 for wchar_t.
	%typemap(in) char const ** %{
	   //Create a string array of the proper size
	   unsigned short size = *$input;
	   AkUtf16* csharpString = (AkUtf16*)$input; 
	   ++csharpString; // skip size word
	   $1 = (char**)AkAlloca(size*sizeof(wchar_t*));
	   unsigned long len = 0;
	   //Make all pointers point to the right place.
	   for(unsigned long i = 0; i < size; i++)
	   {		   		
			AkUtf16* winWcharString = csharpString + len;
			size_t srcLen = AKPLATFORM::AkUtf16StrLen(winWcharString);
			char* charString = NULL;
			CONVERT_UTF16_TO_CHAR(winWcharString, charString);
			$1[i] = charString;

			len += (unsigned long)srcLen + 1;
	   }
	%}


%ignore AkEventCallbackInfo;
%ignore RegisterBusVolumeCallback;
%ignore AkBusCallbackFunc;
%ignore AkSpeakerVolumeMatrixCallbackInfo;	//Not supported.
%ignore AkSpeakerVolumeMatrixBusCallbackInfo;	//Not supported.

%ignore SetPosition;	//SetPosition is replaced by SetObjectPosition below
%ignore AK::SoundEngine::SetListenerPosition;	//Replaced by SetListenerPosition defined below.
%ignore AK::SoundEngine::PostEvent(AkUniqueID, AkGameObjectID, AkUInt32, AkCallbackFunc );		//In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::PostEvent(AkUInt32, AkGameObjectID, AkUInt32, AkCallbackFunc );		//In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::PostEvent(char const *, AkGameObjectID, AkUInt32, AkCallbackFunc);	//In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::PostEvent(wchar_t const *,AkGameObjectID, AkUInt32, AkCallbackFunc);	//In C#, the Callback and the cookie must be defined.
%ignore AK::Monitor::SetLocalOutput;
%ignore LocalOutputFunc;
%ignore AkAuxSendValue;
%ignore s_aszErrorCodes;

// C# bindings allow to use only one of wchar_t*/char* API pairs (multiple-definition problem). 
// For platforms that support wchar_t*, in favor of wchar_t* APIs, ignore char* APIs; and vice versa.
#ifdef AK_SUPPORT_WCHAR
	#define IgnoredCharType char
#else
	#define IgnoredCharType wchar_t
#endif
%ignore AK::SoundEngine::PrepareEvent(PreparationType, const IgnoredCharType**, AkUInt32);
%ignore AK::SoundEngine::PrepareEvent(PreparationType, const IgnoredCharType**, AkUInt32, AkBankCallbackFunc, void*);
%ignore AK::SoundEngine::PrepareGameSyncs(PreparationType, AkGroupType, const IgnoredCharType*, const IgnoredCharType**, AkUInt32);
%ignore AK::SoundEngine::PrepareGameSyncs(PreparationType, AkGroupType, const IgnoredCharType*, const IgnoredCharType**, AkUInt32, AkBankCallbackFunc, void *);
// %ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const IgnoredCharType*, const IgnoredCharType**, AkUInt32);
// %ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const IgnoredCharType*, const IgnoredCharType**, AkUInt32, AkPlayingID);
%ignore AK::SoundEngine::LoadBank(const IgnoredCharType*, AkMemPoolId, AkBankID &);
%ignore AK::SoundEngine::LoadBank(const IgnoredCharType*, AkBankCallbackFunc, void *, AkMemPoolId, AkBankID &);
%ignore AK::SoundEngine::Query::GetRTPCValue(const IgnoredCharType*, AkGameObjectID, AkRtpcValue&, RTPCValue_type&);
%ignore AK::SoundEngine::Query::GetSwitch(const IgnoredCharType*, AkGameObjectID, AkSwitchStateID&);
%ignore AK::SoundEngine::Query::GetState(const IgnoredCharType*, AkStateID&);
%ignore AK::SoundEngine::Query::QueryAudioObjectIDs(const IgnoredCharType*, AkUInt32&, AkObjectInfo*);

// As of Unity3.5.2 and Wwise 22012.1.4, ignore this particular string-based API to avoid a crash
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32, AkPlayingID);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const wchar_t*, const wchar_t**, AkUInt32);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const wchar_t*, const wchar_t**, AkUInt32, AkPlayingID);

%ignore AkCallbackSerializer::EventCallback;
%ignore AkCallbackSerializer::BankCallback;
%ignore AkCallbackSerializer::AudioInterruptionCallback;
%ignore AK::SoundEngine::DynamicSequence::Open(AkGameObjectID, AkUInt32, AkCallbackFunc );		//In C#, the Callback and the cookie must be defined.

//These ignores are necessary to avoid the IsInitialized check added by the %exception statement.
%ignore AK::SoundEngine::IsInitialized;
%ignore AK::SoundEngine::GetDefaultInitSettings;
%ignore AK::SoundEngine::GetDefaultPlatformInitSettings;

//These ignores hide the useless Init/Term of the MusicEngine
%ignore AK::MusicEngine::Init;
%ignore AK::MusicEngine::Term;
%ignore AK::MusicEngine::GetDefaultInitSettings;

// Remove modules that were to be removed in old GenC++.cmd script
%ignore AK::SoundEngine::RegisterPlugin;
%ignore AK::SoundEngine::RegisterCodec;
%ignore AK::SoundEngine::Init;
%ignore AK::SoundEngine::Term;
%ignore AK::SoundEngine::RegisterGlobalCallback;
%ignore AK::SoundEngine::UnregisterGlobalCallback;
%ignore AK::SoundEngine::GetDefaultDeviceSettings;
%ignore AK::StreamMgr::GetDefaultDeviceSettings;
%ignore AkCallbackInfo;
%ignore AkMarkerCallbackInfo;
%ignore AkDurationCallbackInfo;
%ignore AkDynamicSequenceItemCallbackInfo;
%ignore AkMusicSyncCallbackInfo;

// Explicit ignores from generated API.
%ignore AK::Monitor::s_aszErrorCodes;
%ignore AkAssertHook;
%ignore AkCallbackFunc;
%ignore AkGlobalCallbackFunc;
%ignore AK::MemoryMgr::Init;

// Until user has specific audio performance requirements, ignore low-level platformInitSettings members for now.
%ignore hWnd;
%ignore IDirectSound8;
%ignore pXAudio2;
%ignore IXAudio2;
%ignore GetWwiseXAudio2Interface;
%ignore GetDirectSoundInstance;
%ignore GetWwiseXAudio2Interface;
%ignore CellSpurs;
%ignore pSpurs;

// Motion: Registration already handled in handwritten Init.
%ignore AkCreatePluginCallback;
%ignore AkCreateSinkPluginCallback;
%ignore pfSinkPluginFactory;
%ignore AK::MotionEngine::RegisterMotionDevice;

// Query: Ignore AkArray-related features, to avoid too much SWIG hacks: SWIG need separate fully-qualified AkArray classes for each typedef/subclass.
%ignore AK::SoundEngine::Query::AkGameObjectsList;
%ignore AK::SoundEngine::Query::GetActiveGameObjects(AkGameObjectsList&);
%ignore AK::SoundEngine::Query::GameObjDst;
%ignore AK::SoundEngine::Query::AkRadiusList;
%ignore AK::SoundEngine::Query::GetMaxRadius(AkRadiusList &);

// DynaimcDialogue: Ignore unsupported AkArray operations (WG-23783)
%ignore AkArray::BinarySearch;

// iOS: Ignore audio input related features
%ignore kAudioSessionCategory_PlayAndRecord;
%ignore AudioInputCallbackFunc;
%ignore AudioInterruptionCallbackFunc;
%ignore inputCallback;
%ignore inputCallbackCookie;
%ignore interruptionCallback;
%ignore interruptionCallbackCookie;
%ignore AudioBufferList;

// Android: Ignore low-level audio settings and API
%ignore pSLEngine;
%ignore GetWwiseOpenSLInterface;
%ignore pJavaVM;

%{
#if defined(AK_VITA) || defined(AK_PS4)
#undef SWIGEXPORT
#define SWIGEXPORT __declspec(dllexport)
#endif

#include <AK/SoundEngine/Common/AkSoundEngine.h>
#include <AK/SoundEngine/Common/AkModule.h>
#include <AK/SoundEngine/Common/AkStreamMgrModule.h>
#include <AK/MusicEngine/Common/AkMusicEngine.h>
#include "../Common/AkCallbackSerializer.h"
#include <AK/Tools/Common/AkMonitorError.h>
#include <AK/Tools/Common/AkPlatformFuncs.h>
#include <AK/SoundEngine/Common/AkDynamicDialogue.h>
#include "../Common/AkDynamicSequence_PlaylistItem.h"
#include "../Common/AkDynamicSequence_Playlist.h"
#include <AK/SoundEngine/Common/AkQueryParameters.h>
#include <AK/MotionEngine/Common/AkMotionEngine.h>

using namespace AK;
using namespace AK::SoundEngine::DynamicSequence;
using namespace AK::SoundEngine::Query;
%}

%ignore AkExternalSourceArray;
%ignore GetExternalSources();

// Translate C++ operators into function calls
%rename(Assign) AK::SoundEngine::DynamicSequence::PlaylistItem::operator=;
%rename(IsEqualTo) AK::SoundEngine::DynamicSequence::PlaylistItem::operator==;
%rename(ItemAtIndex) AkArray::operator[];

// Rename DynamicSequence APIs for clarity after SWIG removes namespaces for clarity.
%rename(DynamicSequenceOpen) AK::SoundEngine::DynamicSequence::Open;
%rename(DynamicSequenceClose) AK::SoundEngine::DynamicSequence::Close;
%rename(DynamicSequencePlay) AK::SoundEngine::DynamicSequence::Play;
%rename(DynamicSequencePause) AK::SoundEngine::DynamicSequence::Pause;
%rename(DynamicSequenceResume) AK::SoundEngine::DynamicSequence::Resume;
%rename(DynamicSequenceStop) AK::SoundEngine::DynamicSequence::Stop;
%rename(DynamicSequenceBreak) AK::SoundEngine::DynamicSequence::Break;
%rename(DynamicSequenceLockPlaylist) AK::SoundEngine::DynamicSequence::LockPlaylist;
%rename(DynamicSequenceUnlockPlaylist) AK::SoundEngine::DynamicSequence::UnlockPlaylist;


// NOTE: From Unity side, script execution order is crucial: API calls must be made after sound engine is initialized and before it is terminated.
// This code will check that the sound engine is initialized for every function call.  Must be done AFTER "Init" is declared (in the inline section above). It's inserted to all APIs.
// Order: Must be before %include <AkUnityApiHeader.h>
%include "SwigExceptionSwitch.h"

// Expose AkArray Iterator: Step 1: Tell SWIG to use a global proxy class for the nested AkArray::Iterator class.
%include "AkArrayIterator.h";
%{
#include "../Common/AkArrayProxy.h"
%}
%include "AkArrayProxy.h"

RESUME_SWIG_EXCEPTIONS

%include "../Common/AkDynamicSequence_PlaylistItem.h"
%template(AkPlaylistArray) AkArray<AK::SoundEngine::DynamicSequence::PlaylistItem, AK::SoundEngine::DynamicSequence::PlaylistItem const &, ArrayPoolDefault,4>;
// Expose AkArray Iterator: Step 2: Tell C++ compiler to use proxy class.
%{
typedef AkArray<AK::SoundEngine::DynamicSequence::PlaylistItem, AK::SoundEngine::DynamicSequence::PlaylistItem const &, ArrayPoolDefault, 4>::Iterator Iterator;
%}
%include "../Common/AkDynamicSequence_Playlist.h"


// API declarations blob.
%include <AkUnityApiHeader.h>

// Order: Must be before %exception {$action}
%inline %{
extern "C" void Term( );
%}

//Reset the exception to nothing.  This is IMPORTANT, the constructors/destructors are generated after all the interface 
//so to avoid the IsInitialized check in the AkInitSetting structure constructor, the exception must be reset.
PAUSE_SWIG_EXCEPTIONS

//Define some functions exposed only for C#.  These will deal with stuff that C# can't deal with (or is harder to deal with)
// These functions don't need SWIG-inserted sound-engine initialization checks.
// Order: Must be after %exception {$action}
%inline %{

extern "C" AKRESULT Init( 
			AkMemSettings *     in_pMemSettings,
			AkStreamMgrSettings * in_pStmSettings,
			AkDeviceSettings *  in_pDefaultDeviceSettings,
			AkInitSettings *    in_pSettings,
			AkPlatformInitSettings * in_pPlatformSettings,
			AkMusicSettings *	in_pMusicSettings
			);
			
extern "C" void GetDefaultStreamSettings(AkStreamMgrSettings & out_settings);
extern "C" void GetDefaultDeviceSettings(AkDeviceSettings & out_settings);
extern "C" void GetDefaultMusicSettings(AkMusicSettings &out_settings);
extern "C" void GetDefaultInitSettings(AkInitSettings & out_settings);
extern "C" void GetDefaultPlatformInitSettings(AkPlatformInitSettings &out_settings);

#ifdef AK_SUPPORT_WCHAR
	extern "C" AKRESULT SetBasePath(const wchar_t* in_pszBasePath);
	extern "C" AKRESULT SetBankPath(const wchar_t* in_pszBankPath);
	extern "C" AKRESULT SetAudioSrcPath(const wchar_t* in_pszAudioSrcPath);
	extern "C" AKRESULT SetCurrentLanguage(const wchar_t* in_pszAudioSrcPath);
#else
	extern "C" AKRESULT SetBasePath(AkOSChar* in_pszBasePath);
	extern "C" AKRESULT SetBankPath(AkOSChar* in_pszBankPath);
	extern "C" AKRESULT SetAudioSrcPath(AkOSChar* in_pszAudioSrcPath);
	extern "C" AKRESULT SetCurrentLanguage(const AkOSChar* in_pszAudioSrcPath);
#endif

#ifdef AK_ANDROID
	extern "C" AKRESULT AddBasePath( AkOSChar* in_pszBasePath );
#endif

//Override for SetPosition to avoid filling a AkSoundPosition in C# and marshall that. 
extern "C" AKRESULT SetObjectPosition( AkGameObjectID in_GameObjectID, 
	AkReal32 PosX, AkReal32 PosY, AkReal32 PosZ, 
	AkReal32 OrientationX, AkReal32 OrientationY, AkReal32 OrientationZ)
{
	if (!AK::SoundEngine::IsInitialized())
		return AK_Fail;

    AkSoundPosition pos;
    pos.Position.X = PosX;
    pos.Position.Y = PosY;
    pos.Position.Z = PosZ;
    pos.Orientation.X = OrientationX;
    pos.Orientation.Y = OrientationY;
    pos.Orientation.Z = OrientationZ;
    
	return AK::SoundEngine::SetPosition(in_GameObjectID, pos);
}

extern "C" AKRESULT SetListenerPosition( AkReal32 FrontX, AkReal32 FrontY, AkReal32 FrontZ, 
									 AkReal32 TopX, AkReal32 TopY, AkReal32 TopZ, 
									 AkReal32 PosX, AkReal32 PosY, AkReal32 PosZ, 
									 AkUInt32 in_ulListenerIndex)
{
	if (!AK::SoundEngine::IsInitialized())
		return AK_Fail;

    AkListenerPosition pos;
    pos.OrientationFront.X = FrontX;
    pos.OrientationFront.Y = FrontY;
    pos.OrientationFront.Z = FrontZ;
    pos.OrientationTop.X = TopX;
    pos.OrientationTop.Y = TopY;
    pos.OrientationTop.Z = TopZ;
    pos.Position.X = PosX;
    pos.Position.Y = PosY;
    pos.Position.Z = PosZ;
    
	return AK::SoundEngine::SetListenerPosition(pos, in_ulListenerIndex);

}

//Redeclaring, despite the ignore above.  It is done this way because the %exception statement below would add a check to the IsInitialized call, to check if the sound engine is initialized...  
extern "C" bool IsInitialized()
{
	return AK::SoundEngine::IsInitialized();
}

%}


