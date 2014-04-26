using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGachaScreen : MonoBehaviour {

	[SerializeField]
	MSGachaSpinner spinner;

	[SerializeField]
	UISprite machine;

	[SerializeField]
	MSActionButton[] spinButtons;

	[SerializeField]
	MSGachaFeatureMover featureController;

	public void Init(BoosterPackProto pack)
	{
		spinner.Init(pack);

		machine.spriteName = MSUtil.StripExtensions(pack.machineImgName);

		foreach (MSActionButton spinButton in spinButtons)
		{
			spinButton.label.text = pack.gemPrice + " (G) SPIN";

			spinButton.onClick = delegate { StartCoroutine(spinner.Spin(pack.boosterPackId)); };
		}

		featureController.Init(pack.specialItems);
	}
}
