using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UISprite))]
public class CBKCurrencySprite : MonoBehaviour {
	
	[SerializeField]
	CBKResourceManager.ResourceType _type = CBKResourceManager.ResourceType.FREE;
	
	[SerializeField]
	string freeCurrencySpriteName = "moneystack";
	
	[SerializeField]
	string premiumCurrencySpriteName = "diamond";

	UISprite sprite;
	
	public CBKResourceManager.ResourceType type
	{
		get
		{
			return _type;
		}
		set
		{
			if (value == CBKResourceManager.ResourceType.FREE)
			{
				sprite.spriteName = freeCurrencySpriteName;
			}
			else
			{
				sprite.spriteName = premiumCurrencySpriteName;
			}
			_type = value;
		}
	}
	
	void Awake()
	{
		sprite = GetComponent<UISprite>();
	}
}
