using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class PZDamageNumber : MonoBehaviour, CBKPoolable {

	GameObject gameObj;
	Transform trans;

	public GameObject gObj
	{
		get
		{
			return gameObj;
		}
	}

	public Transform transf
	{
		get
		{
			return trans;
		}
	}

	PZDamageNumber _prefab;
	public CBKPoolable prefab
	{
		get
		{
			return _prefab;
		}
		set
		{
			_prefab = value as PZDamageNumber;
		}
	}

	UILabel label;

	[SerializeField]
	UIFont[] fonts;

	[SerializeField]
	TweenAlpha alphaTween;

	[SerializeField]
	TweenPosition heightTween;

	static readonly Vector3 START_OFFSET = new Vector3(0, 36, 0);

	static readonly Vector3 TWEEN_SHIFT = new Vector3(0, 27, 0);

	public CBKPoolable Make (Vector3 origin)
	{
		PZDamageNumber number = Instantiate(this, origin, Quaternion.identity) as PZDamageNumber;
		number.prefab = this;
		return number;
	}

	public void Pool()
	{
		CBKPoolManager.instance.Pool(this);
	}

	void Awake()
	{
		label = GetComponent<UILabel>();
		trans = transform;
		gameObj = gameObject;
	}

	public void Init(PZGem gem)
	{
		label.font = fonts[(gem.colorIndex>4 ? gem.colorIndex-5 : gem.colorIndex)];
		label.text = "+" + ((int)PZCombatManager.instance.activePlayer.monster.attackDamages[(gem.colorIndex>4 ? gem.colorIndex-5 : gem.colorIndex)]).ToString();
		label.alpha = 1;

		trans.parent = gem.transf.parent;
		trans.localScale = Vector3.one;
		trans.localPosition = gem.transf.localPosition;

		alphaTween.Reset();
		alphaTween.PlayForward();

		heightTween.from = trans.localPosition + START_OFFSET;
		heightTween.to = trans.localPosition + START_OFFSET + TWEEN_SHIFT;

		heightTween.Reset();
		heightTween.PlayForward();
	}


}
