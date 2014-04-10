#include "stdafx.h"
#include "AkCallbackSerializer.h"
#include <AK/Tools/Common/AkLock.h>
#include <stdio.h>
#include "ExtraCallbacks.h"


static CAkLock m_Lock;	//Defined here to avoid exposing the CAkLock class through SWIG.

#pragma pack(push, 4)
struct AkCommonCallback
{
	void * pPackage;		//The C# CallbackPackage to return to C#
	AkCommonCallback * pNext;//Pointer to the next callback
	AkCallbackType eType;	//The type of structure following
};
#pragma pack(pop)

AkCommonCallback *AkCallbackSerializer::m_pLockedPtr = NULL;
AkCommonCallback **AkCallbackSerializer::m_pLastNextPtr = NULL;
AkCommonCallback *AkCallbackSerializer::m_pFirst = NULL;
AkEvent AkCallbackSerializer::m_DrainEvent;
void *AkCallbackSerializer::m_pBlockStart = NULL;
void *AkCallbackSerializer::m_pBlockEnd = NULL;
AkCommonCallback *AkCallbackSerializer::m_pNextAvailable = NULL;
AkThreadID AkCallbackSerializer::m_idThread = 0;


AKRESULT AkCallbackSerializer::Init(void * in_pMemory, AkUInt32 in_uSize)
{
	if (m_pBlockStart == NULL)
	{
		m_pBlockStart = in_pMemory;
		m_pBlockEnd = (char*)in_pMemory + in_uSize;
		m_pNextAvailable = (AkCommonCallback*)m_pBlockStart;
		AKPLATFORM::AkCreateEvent(m_DrainEvent);
		m_idThread = AKPLATFORM::CurrentThread();
	}
	return AK_Success;
}

void AkCallbackSerializer::Term()
{
	m_Lock.Lock();
	if (m_pBlockStart != NULL)
	{
		AKPLATFORM::AkSignalEvent(m_DrainEvent);
		AKPLATFORM::AkDestroyEvent(m_DrainEvent);
		m_pBlockStart = NULL;
		m_pBlockEnd = NULL;
		m_pNextAvailable = NULL;
	}

	AK::Monitor::SetLocalOutput();

	m_Lock.Unlock();
}

void AkCallbackSerializer::EventCallback(AkCallbackType in_eType, AkCallbackInfo* in_pCallbackInfo)
{
	if (in_pCallbackInfo == NULL)
		return;	//There wasn't enough memory to store the callback when the user registered it.  We don't know where to call to.

	AkMarkerCallbackInfo *pMarkerInfo = (AkMarkerCallbackInfo *)in_pCallbackInfo;
	AkMusicSyncCallbackInfo *pCueInfo = (AkMusicSyncCallbackInfo *)in_pCallbackInfo;
	AkUInt32 uItemSize = sizeof(AkCommonCallback);
	AkUInt32 uInfoSize = 0;
	switch(in_eType)
	{
	case AK_EndOfEvent:	
		uInfoSize = sizeof(AkEventCallbackInfo); 
		break;
	case AK_EndOfDynamicSequenceItem:
		uInfoSize = sizeof(AkDynamicSequenceItemCallbackInfo); 
		break;
	case AK_Marker:	
		uInfoSize = sizeof(AkMarkerCallbackInfo) - sizeof(char*); 
		uItemSize += (AkUInt32)strlen(pMarkerInfo->strLabel) + 1;
		break;
	case AK_Duration:
		uInfoSize = sizeof(AkDurationCallbackInfo); 
		break;
	
	case AK_MusicSyncUserCue:
		uInfoSize = sizeof(AkMusicSyncCallbackInfo) - sizeof(char*);
		if (pCueInfo->pszUserCueName != NULL)
			uItemSize += (AkUInt32)strlen(pCueInfo->pszUserCueName) + 1;
		else
			uItemSize += 1;
		break;
		//Deliberate fall-through
	case AK_MusicPlayStarted:
	case AK_MusicSyncBeat:			
	case AK_MusicSyncBar:					
	case AK_MusicSyncEntry:
	case AK_MusicSyncExit:				
	case AK_MusicSyncGrid:					
	case AK_MusicSyncPoint:
		uInfoSize = sizeof(AkMusicSyncCallbackInfo); 
		break;
	};

	uItemSize += uInfoSize;
	
	AkCommonCallback *pCommon = AllocNewCall(uItemSize, true);
	if (pCommon == NULL)
		return;

	pCommon->eType = in_eType;
	pCommon->pPackage = in_pCallbackInfo->pCookie;

	memcpy((pCommon+1), in_pCallbackInfo, uInfoSize);

	//Other special treatment...
	if (in_eType == AK_Marker)
	{
		char* pString = (char*)pCommon + uInfoSize + sizeof(AkCommonCallback);
		strcpy(pString, pMarkerInfo->strLabel);
	}	

	if (in_eType == AK_MusicSyncUserCue)
	{
		char* pString = (char*)pCommon + uInfoSize + sizeof(AkCommonCallback);
		if (pCueInfo->pszUserCueName != NULL)
			strcpy(pString, pCueInfo->pszUserCueName);
		else
			*pString = 0;
	}	
}

void* AkCallbackSerializer::Lock()
{
	m_Lock.Lock();
	AkCommonCallback* pRead = NULL;
	if (m_pFirst != NULL)
	{
		//Terminate the linked list.
		*m_pLastNextPtr = NULL;
		m_pLastNextPtr = NULL;	
		m_pLockedPtr = m_pFirst;
		pRead = m_pLockedPtr;
		m_pFirst = NULL;
	}
	m_Lock.Unlock();
	
	return pRead;
}	

void AkCallbackSerializer::Unlock()
{
	m_Lock.Lock();
	m_pLockedPtr = NULL;
	m_Lock.Unlock();
	AKPLATFORM::AkSignalEvent(m_DrainEvent);
}


void AkCallbackSerializer::LocalOutput( AK::Monitor::ErrorCode in_eErrorCode, const AkOSChar* in_pszError, AK::Monitor::ErrorLevel in_eErrorLevel, AkPlayingID in_playingID, AkGameObjectID in_gameObjID )
{
	
	AkUInt32 uLen = ((AkUInt32)AKPLATFORM::OsStrLen(in_pszError) +1) * (AkUInt32)sizeof(AkOSChar)  /*for null*/;
	AkUInt32 uItemSize = sizeof(AkCommonCallback) + sizeof(AK::Monitor::ErrorCode) + sizeof(AK::Monitor::ErrorLevel) + sizeof(AkPlayingID) + sizeof(AkGameObjectID) + uLen;

	AkCommonCallback *pCommon = AllocNewCall(uItemSize, false);
	if (pCommon == NULL)
		return;	//No space, can't log this.  Expected if the ErrorLevel is set to ALL as some logging is done on the game thread

	pCommon->eType = (AkCallbackType)AK_Monitoring_Val;	//Ak_Monitoring isn't defined on the regular SDK.  It's a modification that only the C# side sees.

	char* pData = (char*)(pCommon+1);
	*(AK::Monitor::ErrorCode *)pData = in_eErrorCode;
	pData += sizeof(AK::Monitor::ErrorCode);

	*(AK::Monitor::ErrorLevel *)pData = in_eErrorLevel;
	pData += sizeof(AK::Monitor::ErrorLevel);

	*(AkPlayingID *)pData = in_playingID;
	pData += sizeof(AkPlayingID);

	*(AkGameObjectID *)pData = in_gameObjID;
	pData += sizeof(AkGameObjectID);

	memcpy(pData, in_pszError, uLen);
}

void AkCallbackSerializer::SetLocalOutput( AkUInt32 in_uErrorLevel )
{
	AK::Monitor::SetLocalOutput(in_uErrorLevel, (AK::Monitor::LocalOutputFunc)LocalOutput);
}

AkCommonCallback* AkCallbackSerializer::AllocNewCall( AkUInt32 uItemSize, bool in_bCritical )
{
retry:
	//If the current thread is the main thread that normally pumps the messages, then don't block if the buffer is full!
	in_bCritical = in_bCritical && m_idThread != AKPLATFORM::CurrentThread();

	m_Lock.Lock();
	void* pItemEnd = (char*)m_pNextAvailable + uItemSize;
	void* pReadPtr = m_pLockedPtr != NULL ? m_pLockedPtr : m_pFirst;

	if (m_pBlockStart == NULL || m_pBlockEnd == NULL || m_pNextAvailable == NULL)
	{
		AKPLATFORM::OutputDebugMsg("AkCallbackSerializer::AllocNewCall: Callback serializer terminated but still received event calls. Abort.");
		m_Lock.Unlock();
		return NULL;
	}

	//Is there enough space between the write head and the end of the buffer?
	if (pItemEnd >= m_pBlockEnd)
	{
		//Nope, need to wrap around
		//But is the read ptr in the way?
		if (pReadPtr > m_pNextAvailable)
		{
			//Queue is full, wait for the game to empty it to avoid losing information.
			m_Lock.Unlock();
			if (in_bCritical)
			{
				AKPLATFORM::AkWaitForEvent(m_DrainEvent);
				goto retry; 
			}
			else
				return NULL;	//No memory for that, and we can't block the main game thread.  Expected if the ErrorLevel is set to ALL.
		}
		
		m_pNextAvailable = (AkCommonCallback*)m_pBlockStart;
		pItemEnd = (char*)m_pNextAvailable + uItemSize;
	}

	//Is there enough space up to the read pointer?
	if (m_pNextAvailable == pReadPtr || (m_pNextAvailable < pReadPtr && pItemEnd >= pReadPtr) )	
	{
		//Nope!  Queue is full, wait for the game to empty it to avoid losing information.
		m_Lock.Unlock();
		if (in_bCritical)
		{
			AKPLATFORM::AkWaitForEvent(m_DrainEvent);
			goto retry; 
		}
		else
			return NULL;	//No memory for that, and we can't block the main game thread.  Expected if the ErrorLevel is set to ALL.
	}

	//Link the new item in the list.
	if (m_pFirst == NULL)
		m_pFirst = m_pNextAvailable;
	else
		*m_pLastNextPtr = m_pNextAvailable;
	
	m_pLastNextPtr = &(m_pNextAvailable->pNext);
	m_pNextAvailable->pNext = NULL;

	AkCommonCallback* pRet = m_pNextAvailable;
	m_pNextAvailable = (AkCommonCallback*)pItemEnd;
	
	m_Lock.Unlock();
	return pRet;
}

void AkCallbackSerializer::BankCallback( AkUInt32 in_bankID, const void* in_pInMemoryBankPtr, AKRESULT in_eLoadResult, AkMemPoolId in_memPoolId, void *in_pCookie )
{
	if (in_pCookie == NULL)
		return;	//There wasn't enough memory to store the callback when the user registered it.  We don't know where to call to.

	AkUInt32 uItemSize = sizeof(AkCommonCallback);

	AkUInt32 uInfoSize = sizeof(AkUInt32)+sizeof(AkUIntPtr)+sizeof(AKRESULT)+sizeof(AkMemPoolId);	//in_pCookie will be set in AkCommonCallback

	uItemSize += uInfoSize;

	AkCommonCallback *pCommon = AllocNewCall(uItemSize, true);
	if (pCommon == NULL)
		return;

	pCommon->eType = (AkCallbackType)AK_Bank_Val;	//Ak_Bank isn't defined on the regular SDK.  It's a modification that only the C# side sees.
	pCommon->pPackage = in_pCookie;

	char* pData = (char*)(pCommon+1);
	*(AkUInt32 *)pData = in_bankID;
	pData += sizeof(AkUInt32);
	*(AkUIntPtr *)pData = (AkUIntPtr) in_pInMemoryBankPtr;
	pData += sizeof(AkUIntPtr);
	*(AKRESULT *)pData = in_eLoadResult;
	pData += sizeof(AKRESULT);
	*(AkMemPoolId *)pData = in_memPoolId;
	pData += sizeof(AkMemPoolId);
}

#ifdef AK_IOS
AKRESULT AkCallbackSerializer::AudioInterruptionCallbackFunc(AkInt16 in_bEnterInterruption, AKRESULT in_prevEngineStepResult, void* in_pCookie)
{
	// Cookie is always null in this case.

	// Cast for easier marshalling.
	typedef AkInt32 MarshalBool;
	MarshalBool iEnterInterruption = (MarshalBool) in_bEnterInterruption;

	// Calculate package size.
	AkUInt32 uItemSize = sizeof(AkCommonCallback);
	AkUInt32 uInfoSize = sizeof(MarshalBool)+sizeof(AKRESULT);	//in_pCookie will be set in AkCommonCallback
	uItemSize += uInfoSize;

	// Prepare empty callback argument package (callback queue item).
	AkCommonCallback *pCommon = AllocNewCall(uItemSize, true);
	if (pCommon == NULL)
		return AK_Fail;

	// Fill callback argument package.
	pCommon->eType = (AkCallbackType) AK_AudioInterruption_Val;	// Custimization fir C# only.
	pCommon->pPackage = in_pCookie; // Save memory location of cookie.

	// Copy cookie data
	const AkUInt32 TypeFieldSize = 1;
	char* pData = (char*)(pCommon+TypeFieldSize);
	*(MarshalBool *)pData = iEnterInterruption;
	pData += sizeof(MarshalBool);
	*(AKRESULT *)pData = in_prevEngineStepResult;
	pData += sizeof(AKRESULT);
	
	return AK_Success;
}
#endif // #ifdef AK_IOS