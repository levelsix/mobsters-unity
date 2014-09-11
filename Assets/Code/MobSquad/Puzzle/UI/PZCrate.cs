using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class PZCrate : MonoBehaviour {

	bool started = false;

	[SerializeField]
	UISprite sprite;

	Transform trans;

	[SerializeField]
	float startHeight = 50f;

	[SerializeField]
	float gravity = -0.8f;

	[SerializeField]
	float restitution = 0.7f;

	[SerializeField]
	float bounceEnd = 2f;

	float bounceVelocity = 0f;

	[SerializeField]
	Vector3 startDir;

	void Awake()
	{
		trans = transform;
	}

	void OnEnable()
	{
		started = false;
	}

	/// <summary>
	/// Initialize the crate to have a different sprite based on what is being dropped by the monster.
	/// </summary>
	/// <param name="taskMonster">dropped item info.</param>
	public void InitCrate(TaskStageMonsterProto taskMonster){
		foreach (Transform trans in GetComponentsInChildren<Transform> ()) {
			trans.position = Vector3.zero;
		}
		MonsterProto monster = MSDataManager.instance.Get<MonsterProto>(taskMonster.puzzlePieceMonsterId);

		sprite.transform.localScale = Vector3.one;

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
		StartCoroutine(Bounce());
		
	}

	void OnTriggerEnter(Collider other)
	{
		PZCombatUnit combat = other.GetComponent<PZCombatUnit>();
		if (!started && combat != null && combat == PZCombatManager.instance.activePlayer)
		{
			trans.parent = PZCombatManager.instance.prizeQuantityLabel.parent.transform;
			StartCoroutine(CollectionAnimation());
		}
	}



	IEnumerator CollectionAnimation(){
		PZMoveTowards move = GetComponent<PZMoveTowards>();

		TweenScale scale = GetComponent<TweenScale>();
		scale.duration = move.totalTime;
		scale.ResetToBeginning();
		scale.PlayForward();

		Vector3 dest = new Vector3(0f,0f,0f);
		yield return move.RunMoveTowards(dest, startDir);

		EndOfAnimation();
	}

	[ContextMenu("testLeap")]
	void StartLeap()
	{
		PZMoveTowards move = GetComponent<PZMoveTowards>();
		Vector3 dest = new Vector3(0f, 0f, 0f);
		move.RunMoveTowards(dest, startDir);
	}

	[ContextMenu("testBounce")]
	void TestBounce()
	{
		StartCoroutine(Bounce());
	}

	IEnumerator Bounce()
	{
		Transform spriteT = sprite.transform;
		spriteT.localPosition = new Vector3(0f, startHeight, 0f);
		while(true){
			bounceVelocity += gravity;
			spriteT.localPosition = new Vector3(0f, spriteT.localPosition.y + bounceVelocity, 0f);
			Debug.Log(spriteT.localPosition);
			if(spriteT.localPosition.y < 0)
			{
				//missMovement is to catch an excess movement that would have caused the ball to go through the ground.
				//If we just remove it then the ground will seem sticky because the bounce will always spend a frame starting from the ground
				//I also am only messure the percentage missed, not the actually distance missed.
				float missMovement = -spriteT.localPosition.y / bounceVelocity;
				bounceVelocity *= -restitution;
				if(bounceVelocity < bounceEnd)
				{
					spriteT.localPosition = new Vector3(0f,0f,0f);
					break;
				}
				else
				{
					float missedDistance = missMovement * bounceVelocity;
					spriteT.localPosition = new Vector3(0f, bounceVelocity + missedDistance, 0f);
				}
			}
			yield return null;
		}
	}

	/// <summary>
	/// This gets called when the tween for X is finnished
	/// </summary>
	public void EndOfAnimation(){
		PZCombatManager.instance.crate = null;
		UILabel label = PZCombatManager.instance.prizeQuantityLabel;
		int newCount = int.Parse (label.text) + 1;
		label.text = newCount.ToString();
		GetComponent<MSSimplePoolable>().Pool();
	}
}
