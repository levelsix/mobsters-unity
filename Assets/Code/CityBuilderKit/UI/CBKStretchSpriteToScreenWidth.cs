using UnityEngine;
using System.Collections;

public class CBKStretchSpriteToScreenWidth : MonoBehaviour {

	[SerializeField]
	bool width = true;

	void Start()
	{
		if (width)
		{
			GetComponent<UISprite>().width = Mathf.CeilToInt(Screen.width * 640f / Screen.height);
		}
		else
		{
			GetComponent<UISprite>().height = Mathf.CeilToInt(Screen.width * 640f / Screen.height);
		}
	}
}
