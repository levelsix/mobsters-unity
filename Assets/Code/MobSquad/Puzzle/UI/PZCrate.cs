using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class PZCrate : MonoBehaviour {

	#region spriteList

	[SerializeField]
	Sprite capsuleCommon;

	[SerializeField]
	Sprite capsuleRare;

	[SerializeField]
	Sprite capsuleUltra;

	[SerializeField]
	Sprite capsuleEpic;

	[SerializeField]
	Sprite capsuleLegendary;

	[SerializeField]
	Sprite pieceCommon;
	
	[SerializeField]
	Sprite pieceRare;
	
	[SerializeField]
	Sprite pieceUltra;
	
	[SerializeField]
	Sprite pieceEpic;
	
	[SerializeField]
	Sprite pieceLegendary;

	#endregion

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
	SpriteRenderer sprite;

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
	/// <param name="monster">The monster that is dropping the item.</param>
	public void initCrate(PZMonster monster){
		foreach (Transform trans in GetComponentsInChildren<Transform> ()) {
			trans.position = Vector3.zero;
		}

		transform.localScale = Vector3.one;

		bounceTween.ResetToBeginning ();
		bounceTween.PlayForward ();

		if (monster.monster.numPuzzlePieces > 1) {
			switch(monster.monster.quality){

			case Quality.COMMON:
				sprite.sprite = pieceCommon;
				break;
			case Quality.RARE:
				sprite.sprite = pieceRare;
				break;
			case Quality.ULTRA:
				sprite.sprite = pieceUltra;
				break;
			case Quality.EPIC:
				sprite.sprite = pieceEpic;
				break;
			case Quality.LEGENDARY:
				sprite.sprite = pieceLegendary;
				break;
			default:
				//if something isn't right then the sprite will default to the prefab crate
				break;
			}
		} else {
			switch(monster.monster.quality){
				
			case Quality.COMMON:
				sprite.sprite = capsuleCommon;
				break;
			case Quality.RARE:
				sprite.sprite = capsuleRare;
				break;
			case Quality.ULTRA:
				sprite.sprite = capsuleUltra;
				break;
			case Quality.EPIC:
				sprite.sprite = capsuleEpic;
				break;
			case Quality.LEGENDARY:
				sprite.sprite = capsuleLegendary;
				break;
			default:
				//if something isn't right then the sprite will default to the prefab crate
				break;
			}
		}
		
	}

//	IEnumerator Fall()
//	{
//		Vector3 start = trans.localPosition;
//		Vector3 target = start + fallDist;
//		float curr = 0;
//		while (curr < fallTime)
//		{
//			curr += Time.deltaTime;
//			trans.localPosition = Vector3.Lerp(start, target, curr/fallTime);
//			yield return null;
//		}
//	}

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

//	IEnumerator Move()
//	{
//		started = true;
//		Vector3 originalScale = trans.localScale;
//		float curr = 0;
//		while (curr < moveTime)
//		{
//			curr += Time.deltaTime;
//			
//			float lp = curr/moveTime;
//			trans.localPosition += Time.deltaTime * moveSpeed * lp *
//				Vector3.Lerp(new Vector3(1.2f, 3f), new Vector3(1.2f, -1f), lp);
//			trans.localScale = Vector3.Lerp (originalScale, Vector3.zero, lp);
//
//			yield return null;
//		}
//
//		PZCombatManager.instance.crate = null;
//		GetComponent<MSSimplePoolable>().Pool();
//		UILabel label = PZCombatManager.instance.prizeQuantityLabel;
//		int newCount = int.Parse (label.text) + 1;
//		label.text = newCount.ToString();
//	}

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
