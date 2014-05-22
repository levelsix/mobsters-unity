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

	Transform trans;
	
	void Awake()
	{
		trans = transform;
	}

	void OnEnable()
	{
		started = false;
		StartCoroutine(Fall());
	}

	/// <summary>
	/// Initialize the crate to have a different sprite based on what is being dropped by the monster.
	/// </summary>
	/// <param name="monster">The monster that is dropping the item.</param>
	public void initCrate(PZMonster monster){
		if (monster.monster.numPuzzlePieces > 1) {
			switch(monster.monster.quality){

			case Quality.COMMON:
				GetComponent<SpriteRenderer> ().sprite = pieceCommon;
				break;
			case Quality.RARE:
				GetComponent<SpriteRenderer> ().sprite = pieceRare;
				break;
			case Quality.ULTRA:
				GetComponent<SpriteRenderer> ().sprite = pieceUltra;
				break;
			case Quality.EPIC:
				GetComponent<SpriteRenderer> ().sprite = pieceEpic;
				break;
			case Quality.LEGENDARY:
				GetComponent<SpriteRenderer> ().sprite = pieceLegendary;
				break;
			default:
				//if something isn't right then the sprite will default to the prefab crate
				break;
			}
		} else {
			switch(monster.monster.quality){
				
			case Quality.COMMON:
				GetComponent<SpriteRenderer> ().sprite = capsuleCommon;
				break;
			case Quality.RARE:
				GetComponent<SpriteRenderer> ().sprite = capsuleRare;
				break;
			case Quality.ULTRA:
				GetComponent<SpriteRenderer> ().sprite = capsuleUltra;
				break;
			case Quality.EPIC:
				GetComponent<SpriteRenderer> ().sprite = capsuleEpic;
				break;
			case Quality.LEGENDARY:
				GetComponent<SpriteRenderer> ().sprite = capsuleLegendary;
				break;
			default:
				//if something isn't right then the sprite will default to the prefab crate
				break;
			}
		}
	}

	IEnumerator Fall()
	{
		Vector3 start = trans.localPosition;
		Vector3 target = start + fallDist;
		float curr = 0;
		while (curr < fallTime)
		{
			curr += Time.deltaTime;
			trans.localPosition = Vector3.Lerp(start, target, curr/fallTime);
			yield return null;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		PZCombatUnit combat = other.GetComponent<PZCombatUnit>();
		if (!started && combat != null && combat == PZCombatManager.instance.activePlayer)
		{
			StartCoroutine(Move());
		}
	}

	IEnumerator Move()
	{
		started = true;
		Vector3 originalScale = trans.localScale;
		float curr = 0;
		while (curr < moveTime)
		{
			curr += Time.deltaTime;
			
			float lp = curr/moveTime;
			trans.localPosition += Time.deltaTime * moveSpeed * lp *
				Vector3.Lerp(new Vector3(1.2f, 3f), new Vector3(1.2f, -1f), lp);
			trans.localScale = Vector3.Lerp (originalScale, Vector3.zero, lp);

			yield return null;
		}

		PZCombatManager.instance.crate = null;
		GetComponent<MSSimplePoolable>().Pool();
		UILabel label = PZCombatManager.instance.prizeQuantityLabel;
		int newCount = int.Parse (label.text) + 1;
		label.text = newCount.ToString();
	}
}
