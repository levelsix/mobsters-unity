using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSTriggerGachaButton : MSTriggerPopupButton {

	[SerializeField] int boosterId = 1;

	public override void OnClick ()
	{
		base.OnClick ();
		BoosterPackProto booster = MSDataManager.instance.Get<BoosterPackProto>(boosterId);
		if (booster != null)
		{
			popup.GetComponent<MSGachaScreen>().Init(booster);
		}
	}
}
