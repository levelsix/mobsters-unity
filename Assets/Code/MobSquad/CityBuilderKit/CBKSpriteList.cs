using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CBKSpriteList : MonoBehaviour {
	
	[SerializeField]
	string[] names;

	[SerializeField]
	Sprite[] sprites;

	void Awake()
	{
		if (names.Length != sprites.Length)
		{
			Debug.LogError("Mission Maps: Number of sprites and spritenames do not match!");
		}
	}

	public Sprite GetSprite(string name)
	{
		int index = 0;
		for (index = 0; index < names.Length; index++) 
		{
			if (name == names[index])
			{
				return sprites[index];
			}
		}
		return null;
	}
}
