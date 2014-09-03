using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UIFont))]
public class MSResetFont : MonoBehaviour 
{

	[SerializeField] string fontSpriteName;

	public void SetAtlas(UIAtlas atlas)
	{
		UIFont font = GetComponent<UIFont>();
		font.atlas = atlas;
		font.spriteName = fontSpriteName;
	}
}
