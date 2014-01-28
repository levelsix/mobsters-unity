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
	CBKActionButton spinButton;

	#region DEBUG
	void Start()
	{
		Init (CBKDataManager.instance.Get<BoosterPackProto>(1));
	}
	#endregion

	void Init(BoosterPackProto pack)
	{
		spinner.Init(pack);

		machine.spriteName = CBKUtil.StripExtensions(pack.machineImgName);

		spinButton.label.text = pack.gemPrice + " (G) SPIN";

		spinButton.onClick = delegate { StartCoroutine(spinner.Spin(pack.boosterPackId)); };
	}
}
