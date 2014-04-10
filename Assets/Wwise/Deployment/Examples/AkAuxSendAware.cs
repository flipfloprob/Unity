#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

//TODO potential optimization: split this class into 2 objects, one for moving objects and one for static objects.

//This component should be added to any object that needs to have environment effects (e.g. reverb) applied to it.
//This works hand in hand with AkAuxSend-derived classes (e.g. AkBoxAuxSendironment).  
//When the AkAuxSendAware is within an AkAuxSend, an environment percentage value is computed (the amount of wet
//signal the AuxSendironment is contributing) and applied to this object.
[RequireComponent(typeof(Collider))]
public class AkAuxSendAware : MonoBehaviour 
{			
	private List<AkAuxSend> m_activeAuxSends = new List<AkAuxSend>();
	private AkAuxSendArray m_auxSendValues;
	
	//When starting, check if any of our parent objects have the AkAuxSend component.
	//We'll assume that this object is then affected by the same AuxSendironment setting.
	void Start()
	{
		GameObject currentParent = gameObject;
		while(currentParent != null && currentParent.GetComponent("AkAuxSend") == null && currentParent.transform.parent != null)
			currentParent = currentParent.transform.parent.gameObject;
		
		AddAuxSend(currentParent);		
	}
	
	//When entering an AuxSendironment, add it to the list of active AuxSendironments
	void OnTriggerEnter(Collider other)
	{
		AddAuxSend(other.gameObject);
	}
	
	void AddAuxSend(GameObject in_AuxSendObject)
	{
		AkAuxSend AuxSend = (AkAuxSend)in_AuxSendObject.GetComponent("AkAuxSend");
		if (AuxSend != null)
		{
			m_activeAuxSends.Add(AuxSend);
			m_auxSendValues = null;
			UpdateAuxSend();			
		}
	}
	
	//When exiting an AuxSendironment, remove it from active AuxSendironments
	void OnTriggerExit(Collider other)
	{
		AkAuxSend AuxSend = (AkAuxSend)other.gameObject.GetComponent("AkAuxSend");
		if(AuxSend != null)
		{
			m_activeAuxSends.Remove(AuxSend);
			m_auxSendValues = null;
			UpdateAuxSend();			
		}
	}
	
	void Update()
	{
		//For this example, we assume:
		//- The AkAuxSend objects don't move.
		//- The Game Object has a AkGameObjectTracker component.
		//- The Collider position anchor is at the center.
				
		//If we know this object hasn't moved, don't update the AuxSendironment data uselessly.				
		AkGameObjectTracker tracker = (AkGameObjectTracker)gameObject.GetComponent("AkGameObjectTracker");
		if (tracker != null && tracker.HasMovedInLastFrame())
		{
			UpdateAuxSend();
		}		
	}
	
	void UpdateAuxSend()
	{
		if (m_auxSendValues == null)
			m_auxSendValues = new AkAuxSendArray((uint)m_activeAuxSends.Count);
		else
			m_auxSendValues.Reset();				
				
		foreach(AkAuxSend AuxSend in m_activeAuxSends)
			m_auxSendValues.Add(AuxSend.GetAuxBusID(), AuxSend.GetAuxSendValueForPosition(gameObject.transform.position));
		
		AkSoundEngine.SetGameObjectAuxSendValues(gameObject, m_auxSendValues, (uint)m_activeAuxSends.Count);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.
