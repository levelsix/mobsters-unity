using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKGachaScreen : MonoBehaviour {

	[SerializeField]
	CBKGachaSpinner spinner;

	[SerializeField]
	UISprite machine;

	[SerializeField]
	CBKActionButton[] spinButtons;

	[SerializeField]
	CBKGachaFeatureMover featureController;

	public void Init(BoosterPackProto pack)
	{
		spinner.Init(pack);

		machine.spriteName = CBKUtil.StripExtensions(pack.machineImgName);

		foreach (CBKActionButton spinButton in spinButtons)
		{
			spinButton.label.text = pack.gemPrice + " (G) SPIN";

			spinButton.onClick = delegate { StartCoroutine(spinner.Spin(pack.boosterPackId)); };
		}

		featureController.Init(pack.specialItems);
	}
}
