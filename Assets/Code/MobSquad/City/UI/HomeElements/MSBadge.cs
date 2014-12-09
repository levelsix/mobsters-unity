using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.lvl6.proto;

/// <summary>
/// @author Rob Giusti
/// MSBadge
/// </summary>
public class MSBadge : MonoBehaviour {

	[SerializeField] 
	UILabel label;

	public TweenRotation labelTween
	{
		get
		{
			Debug.LogWarning("get tween", this);
			return label.GetComponent<TweenRotation>();
		}
	}

	[SerializeField]
	public UISprite sprite;

	int _nots = 0;

	public int notifications
	{
		get
		{
			return _nots;
		}
		set
		{
			_nots = value;
			sprite.alpha = value > 0 ? 1 : 0;
			label.text = value.ToString();
		}
	}

	void Awake()
	{
		notifications = _nots;
//		labelTween = label.GetComponent<TweenRotation>();
	}
}
