using UnityEngine;
using System.Collections;

public class PZBackground : MonoBehaviour {

	[SerializeField]
	bool top;

	[SerializeField]
	bool left;

	SpriteRenderer sprite;

	void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		PZScrollingBackground.instance.SetSingleBackground(top, left, sprite);
	}
}
