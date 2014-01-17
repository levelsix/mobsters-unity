using UnityEngine;
using System.Collections;

public class PZMiniHealthBar : MonoBehaviour {

	[SerializeField]
	UISprite bar;

	[SerializeField]
	UISprite background;

	[SerializeField]
	UILabel health;

	const float FADE_START_TIME = 2f;
	const float FADE_TIME = 2f;

	void Start()
	{

	}

	void UpdateFromValues(int currHealth, int maxhealth)
	{

	}

}
