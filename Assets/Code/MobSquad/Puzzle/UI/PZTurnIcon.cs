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

	[SerializeField] GameObject enemyLabel;

	[SerializeField] GameObject enemySprite;

	//[SerializeField] UILabel debugLabel;



	public void Init(bool enemy)
	{
		PZMonster monster = enemy ? PZCombatManager.instance.activeEnemy.monster : PZCombatManager.instance.activePlayer.monster;
		bgSprite.spriteName = MSGoonCard.smallBackgrounds[monster.monster.monsterElement];
		MSSpriteUtil.instance.SetSprite(monster.monster.imagePrefix, monster.monster.imagePrefix + "Thumbnail", thumb, 1, AfterSpriteSet);
		thumb.transform.localScale = Vector3.one;
		thumb.transform.localPosition = thumbPos;

		bool showEnemyMarks = enemy && PZCombatManager.instance.activeEnemy.monster.monster.monsterElement == PZCombatManager.instance.activePlayer.monster.monster.monsterElement;
		enemyLabel.SetActive(showEnemyMarks);
		enemySprite.SetActive(showEnemyMarks);
	}

	void AfterSpriteSet()
	{
		thumb.transform.localScale = Vector3.one;
		thumb.transform.localPosition = thumbPos;
		thumb.MakePixelPerfect();
	}

	public void Leave()
	{
		transform.parent = transform.parent.parent;
		SpringPosition spring = SpringPosition.Begin(gameObject, transform.localPosition + exitOffset, 15);
		spring.onFinished = delegate { this.GetComponent<MSSimplePoolable>().Pool(); };
	}

	public Coroutine RunFlip(bool enemy)
	{
		return StartCoroutine(Flip(enemy));
	}

	IEnumerator Flip(bool enemy)
	{
		tweenScale.PlayForward();
		while (tweenScale.tweenFactor < 1)
		{
			yield return null;
		}
		Init (enemy);
		tweenScale.PlayReverse();
		while (tweenScale.tweenFactor > 0)
		{
			yield return null;
		}
		thumb.transform.localScale = Vector3.one;
		thumb.transform.localPosition = thumbPos;
		thumb.MakePixelPerfect();
	}

	void Update()
	{
		//debugLabel.text = thumb.transform.localPosition + "\n" + thumb.transform.localScale + "\n" + thumb.width + ", " + thumb.height;
	}
}
