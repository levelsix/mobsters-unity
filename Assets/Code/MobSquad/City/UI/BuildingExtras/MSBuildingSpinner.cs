using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBuildingSpinner
/// </summary>
public class MSBuildingSpinner : MonoBehaviour {

	[SerializeField]
	float time = 1;

	[SerializeField]
	float curr = 0;

	[SerializeField]
	SpriteRenderer sprite;

	[SerializeField]
	MSBuilding building;

	TweenRotation rotate;

	void Awake()
	{
		rotate = GetComponent<TweenRotation>();
	}

	void OnEnable()
	{
		SetAlpha (0f);
		if (building.upgrade != null)
		{
			building.upgrade.OnFinishUpgrade += OnUpgradeFinish;
		}
	}

	void OnDisable()
	{
		if (building.upgrade != null)
		{
			building.upgrade.OnFinishUpgrade -= OnUpgradeFinish;
		}
	}

	[ContextMenu ("Play")]
	void OnUpgradeFinish()
	{
		StartCoroutine(DoAnimations());
	}

	IEnumerator DoAnimations()
	{
		rotate.duration = time*1.1f;
		rotate.ResetToBeginning();
		rotate.PlayForward();

		float startAlphaTime = time/10f;
		curr = 0;
		while (curr < time)
		{
			curr += Time.deltaTime;
			if (curr < startAlphaTime)
			{
				SetAlpha(curr/startAlphaTime);
			}
			else if (curr > time - startAlphaTime)
			{
				SetAlpha((time-curr)/startAlphaTime);
			}
			else
			{
				SetAlpha(1);
			}
			yield return null;
		}
	}

	void SetAlpha(float alph)
	{
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alph);
	}


}
