using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MSSimplePoolable))]
public class MSPickIconEntry : MonoBehaviour {

	[SerializeField]
	UI2DSprite sprite;

	int iconId;

	Transform selectionBg;

	public void Init(int iconId, Transform selectionBg)
	{
		this.iconId = iconId;
		this.selectionBg = selectionBg;
		MSSpriteUtil.instance.SetSprite("clanicon", "clanicon" + iconId, sprite);
	}

	void OnClick()
	{
		selectionBg.parent = transform;
		selectionBg.localPosition = Vector3.zero;

		if (MSActionManager.UI.OnChangeClanIcon != null)
		{
			MSActionManager.UI.OnChangeClanIcon(iconId);
		}
	}
}
