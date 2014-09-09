using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class PZCrate : MonoBehaviour {
	[SerializeField]
	float moveTime;

	[SerializeField]
	float moveSpeed;

	bool started = false;

	[SerializeField]
	Vector3 fallDist;

	[SerializeField]
	float fallTime;

	[SerializeField]
	TweenPosition xTween;

	[SerializeField]
	TweenPosition yTween;

	[SerializeField]
	TweenPosition bounceTween;

	[SerializeField]
	TweenScale scaleTween;

	[SerializeField]
	UISprite sprite;

	Transform trans;

	void Awake()
	{
		trans = transform;
	}

	void OnEnable()
	{
		started = false;
		//StartCoroutine(Fall());

	}

	/// <summary>
	/// Initialize the crate to have a different sprite based on what is being dropped by the monster.
	/// </summary>
	/// <param name="taskMonster">dropped item info.</param>
	public void initCrate(TaskStageMonsterProto taskMonster){
		foreach (Transform trans in GetComponentsInChildren<Transform> ()) {
			trans.position = Vector3.zero;
		}
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(taskMonster.puzzlePieceMonsterId);

		sprite.transform.localScale = Vector3.one;

		bounceTween.ResetToBeginning ();
		bounceTween.PlayForward ();


		if (taskMonster.itemId > 0) {
			string name = MSDataManager.instance.Get<ItemProto>(taskMonster.itemId).imgName;
			name = name.Substring(0, name.Length - ".png".Length);
			sprite.spriteName = name;
		}else if (monster.numPuzzlePieces > 1) {
			sprite.spriteName = "gacha" + monster.quality.ToString().ToLower() + "piece";
		} else {
			sprite.spriteName = "gacha" + monster.quality.ToString().ToLower() + "ball";
		}

		sprite.MakePixelPerfect ();
	}

	void OnTriggerEnter(Collider other)
	{
		PZCombatUnit combat = other.GetComponent<PZCombatUnit>();
		if (!started && combat != null && combat == PZCombatManager.instance.activePlayer)
		{
			//StartCoroutine(Move());
			CollectionAnimation();
			trans.parent = PZCombatManager.instance.prizeQuantityLabel.parent.transform;
		}
	}

	void CollectionAnimation(){
		Vector3 deltaPosition = PZCombatManager.instance.prizeQuantityLabel.transform.parent.position - trans.position;
		xTween.to = new Vector3 (-trans.localPosition.x , 0f, 0f);

		yTween.to = new Vector3 (0f, -trans.localPosition.y, 0f);

		xTween.ResetToBeginning ();
		xTween.PlayForward ();

		yTween.ResetToBeginning ();
		yTween.PlayForward ();

		scaleTween.ResetToBeginning ();
		scaleTween.PlayForward ();
	}

	/// <summary>
	/// This gets called when the tween for X is finnished
	/// </summary>
	public void endOfAnimation(){
		PZCombatManager.instance.crate = null;
		GetComponent<MSSimplePoolable>().Pool();
		UILabel label = PZCombatManager.instance.prizeQuantityLabel;
		int newCount = int.Parse (label.text) + 1;
		label.text = newCount.ToString();
	}
}
