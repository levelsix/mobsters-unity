using UnityEngine;
using System.Collections;

public class PZBackground : MonoBehaviour {

	[SerializeField]
	bool top;

	[SerializeField]
	bool left;

	SpriteRenderer sprite;

	void OnEnable()
	{
		if (sprite == null)
		{
			sprite = GetComponent<SpriteRenderer>();
		}
		PZScrollingBackground.instance.SetSingleBackground(top, left, sprite);
	}
}
