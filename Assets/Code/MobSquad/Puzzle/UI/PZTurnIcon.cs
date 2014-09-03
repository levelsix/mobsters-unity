using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// PZTurnIcon
/// @author Rob Giusti
/// </summary>
[RequireComponent (typeof (MSSimplePoolable))]
[RequireComponent (typeof (SpringPosition))]
public class PZTurnIcon : MonoBehaviour 
{
	[SerializeField] UISprite bgSprite;

	[SerializeField] UI2DSprite thumb;

	[SerializeField] Vector3 exitOffset;

	[SerializeField] TweenScale tweenScale;

	[SerializeField] Vector3 thumbPos;

	public void Init(PZMonster monster)
	{
		bgSprite.spriteName = MSGoonCard.smallBackgrounds[monster.monster.monsterElement];
		thumb.transform.localScale = Vector3.one;
		thumb.transform.localPosition = thumbPos;
		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Thumbnail", thumb);
	}

	public void Leave()
	{
		transform.parent = transform.parent.parent;
		SpringPosition spring = SpringPosition.Begin(gameObject, transform.localPosition + exitOffset, 15);
		spring.onFinished = delegate { this.GetComponent<MSSimplePoolable>().Pool(); };
	}

	public Coroutine RunFlip(PZMonster flipTo)
	{
		return StartCoroutine(Flip(flipTo));
	}

	IEnumerator Flip(PZMonster flipTo)
	{
		tweenScale.PlayForward();
		while (tweenScale.tweenFactor < 1)
		{
			yield return null;
		}
		Init (flipTo);
		tweenScale.PlayReverse();
		while (tweenScale.tweenFactor > 0)
		{
			yield return null;
		}
		thumb.transform.localScale = Vector3.one;
		thumb.transform.localPosition = thumbPos;
		thumb.MakePixelPerfect();
	}
}
