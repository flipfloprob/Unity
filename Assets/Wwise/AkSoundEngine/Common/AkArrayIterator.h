//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

// NOTE: Need to maintain this file depending on SWIG parsing error on nested C++ class.
// AkArray::Iterator proxy class, copied from <AK/Tool/Common/AkArray.h>

#include "SwigExceptionSwitch.h"

#ifdef SWIG
	
PAUSE_SWIG_EXCEPTIONS

	typedef AK::SoundEngine::DynamicSequence::PlaylistItem PlaylistItem;

	%rename(NextIter) Iterator::operator++;
	%rename(PrevIter) Iterator::operator--;
	%rename(GetItem) Iterator::operator*;
	%rename(IsEqualTo) Iterator::operator==;
	%rename(IsDifferentFrom) Iterator::operator!=;

	struct Iterator
	{
		 PlaylistItem* pItem;	///< Pointer to the item in the array.

		 /// ++ operator
		 Iterator& operator++()
		 {
			 AKASSERT( pItem );
			 ++pItem;
			 return *this;
		 }

		 /// -- operator
		 Iterator& operator--()
		 {
			 AKASSERT( pItem );
			 --pItem;
			 return *this;
		 }

		 /// * operator
		 PlaylistItem& operator*()
		 {
			 AKASSERT( pItem );
			 return *pItem;
		 }

		 /// == operator
		 bool operator ==( const Iterator& in_rOp ) const
		 {
			return ( pItem == in_rOp.pItem );
		 }

		 /// != operator
		 bool operator !=( const Iterator& in_rOp ) const
		 {
			return ( pItem != in_rOp.pItem );
		 }
	};

	%nestedworkaround AkArray<AK::SoundEngine::DynamicSequence::PlaylistItem, AK::SoundEngine::DynamicSequence::PlaylistItem const &, ArrayPoolDefault, 4>::Iterator;

RESUME_SWIG_EXCEPTIONS

#endif // #ifdef SWIG