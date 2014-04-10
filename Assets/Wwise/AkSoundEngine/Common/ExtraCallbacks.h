//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

// Unity-only callback type codes for native-side callback delegate.
// Note: _Val suffix was added to avoid name clashes with the hacked ones in blob input to SWIG. Do not attempt to rename, and follow this convention when adding new callbacks.

#define AK_Monitoring_Val 0x20000000
#define AK_Bank_Val 0x40000000
#define AK_AudioInterruption_Val 0x22000000