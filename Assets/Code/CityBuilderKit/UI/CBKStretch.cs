using UnityEngine;
using System.Collections;

public class CBKStretch : MonoBehaviour {

	[SerializeField]
	bool rotated = false;

	[SerializeField]
	bool fixWidth = true;

	[SerializeField]
	bool fixHeight = false;

	void Start()
	{
		UISprite uiSprite = GetComponent<UISprite>();
		UI2DSprite sprite2d = GetComponent<UI2DSprite>();

		int width = Mathf.CeilToInt(Screen.width * 640f / Screen.height);
		int height = 640;

		if (!rotated)
		{
			if (uiSprite != null)
			{
				if (fixWidth)
				{
					uiSprite.width = width;
				}
				if (fixHeight)
				{
					uiSprite.height = height;
				}
			}
			if (sprite2d != null)
			{
				if (fixWidth)
				{
					sprite2d.width = width;
				}
				if (fixHeight)
				{
					sprite2d.height = height;
				}
			}
		}
		else
		{
			if (uiSprite != null)
			{
				if (fixWidth)
				{
					uiSprite.height = width;
				}
				if (fixHeight)
				{
					uiSprite.width = height;
				}
			}
			if (sprite2d != null)
			{
				if (fixWidth)
				{
					sprite2d.height = width;
				}
				if (fixHeight)
				{
					sprite2d.width = height;
				}
			}
		}
	}
}
