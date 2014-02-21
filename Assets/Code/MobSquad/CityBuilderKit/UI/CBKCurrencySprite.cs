using UnityEngine;
using System.Collections;
using com.lvl6.proto;

[RequireComponent (typeof (UISprite))]
public class CBKCurrencySprite : MonoBehaviour {
	
	[SerializeField]
	ResourceType _type = ResourceType.CASH;
	
	[SerializeField]
	string freeCurrencySpriteName = "moneystack";
	
	[SerializeField]
	string premiumCurrencySpriteName = "diamond";

	UISprite sprite;
	
	public ResourceType type
	{
		get
		{
			return _type;
		}
		set
		{
			if (value == ResourceType.CASH)
			{
				sprite.spriteName = freeCurrencySpriteName;
			}
			else
			{
				sprite.spriteName = premiumCurrencySpriteName;
			}
			UISpriteData data = sprite.GetAtlasSprite();
			sprite.width = data.width;
			sprite.height = data.height;
			_type = value;
		}
	}
	
	void Awake()
	{
		sprite = GetComponent<UISprite>();
	}
}
