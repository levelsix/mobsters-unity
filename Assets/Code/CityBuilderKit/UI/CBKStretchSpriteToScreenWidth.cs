using UnityEngine;
using System.Collections;

public class CBKStretchSpriteToScreenWidth : MonoBehaviour {

	[SerializeField]
	bool wide = true;

	void Start()
	{
		UISprite uiSprite = GetComponent<UISprite>();
		UI2DSprite sprite2d = GetComponent<UI2DSprite>();

		int width = Mathf.CeilToInt(Screen.width * 640f / Screen.height);

		if (wide)
		{
			if (uiSprite != null)
			{
				uiSprite.width = width;
			}
			if (sprite2d != null)
			{
				sprite2d.width = width;
			}
		}
		else
		{
			if (uiSprite != null)
			{
				uiSprite.height = width;
			}
			if (sprite2d != null)
			{
				sprite2d.height = width;
			}
		}
	}
}
