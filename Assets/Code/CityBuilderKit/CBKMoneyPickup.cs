using UnityEngine;
using System.Collections;

public class CBKMoneyPickup : MonoBehaviour, CBKPoolable {
	
	#region Poolable Variables
	
	/// <summary>
	/// Gets or sets the prefab.
	/// Shouldn't actually really be set from here, but
	/// we need that to implement Poolable. 
	/// </summary>
	/// <value>
	/// The prefab.
	/// </value>
	public CBKPoolable prefab {
		get {
			return CBKPrefabList.instance.moneyPrefab;
		}
		set {
			CBKPrefabList.instance.moneyPrefab = value as CBKMoneyPickup;
		}
	}
	
	/// <summary>
	/// Pointer to the game object.
	/// </summary>
	[HideInInspector]
	public GameObject gameObj;
	
	/// <summary>
	/// Gets the game object.
	/// Used in Poolable implementation
	/// </summary>
	public GameObject gObj {
		get {
			return gameObj;
		}
	}
	
	/// <summary>
	/// Pointer to the transform.
	/// </summary>
	[HideInInspector]
	public Transform trans;
	
	/// <summary>
	/// Gets the transform.
	/// Used in Poolable implementation.
	/// </summary>
	public Transform transf {
		get {
			return trans;
		}
	}
	
	#endregion
	
	/// <summary>
	/// Upon being initialized, the money object will wait this long to be clicked.
	/// After this time, it will 
	/// </summary>
	const float TIME_TO_PICKUP = 3f;
	
	const float DROP_TIME = 1f;
	
	const float MENU_TIME = 1f;
	
	/// <summary>
	/// The amount of money in this pickup.
	/// </summary>
	int amount;
	
	/// <summary>
	/// Whether this money pickup has been clicked yet.
	/// Also, triggered by the passage of time if not clicked.
	/// </summary>
	bool _clicked;
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="CBKMoneyPickup"/> is clicked.
	/// Starts the coroutine that moves the money towards the menu when set from false to true.
	/// </summary>
	/// <value>
	/// <c>true</c> if clicked; otherwise, <c>false</c>.
	/// </value>
	bool clicked
	{
		get
		{
			return _clicked;
		}
		set
		{
			if (!_clicked && value)
			{
				StartCoroutine(SendToMenu());
			}
			_clicked = value;
		}
	}
	
	CBKPointAtCamera gameUI;
	
	#region Movement Variables
	
	Vector3 startPos;
	
	Vector3 endPos;
	
	static readonly Vector3 startOffset = new Vector3(-0.3f, 0, -0.3f);
	
	const float dropDist = 1f;
	
	#endregion
	
	public CBKPoolable Make (Vector3 origin)
	{
		CBKMoneyPickup money = Instantiate(this, origin, Quaternion.identity) as CBKMoneyPickup;
		return money;
	}
	
	public void Awake()
	{
		gameObj = gameObject;
		trans = transform;
		gameUI = GetComponent<CBKPointAtCamera>();
	}
	
	public void Init(CBKBuilding source, int money)
	{
		clicked = false;
		trans.position = source.trans.position + startOffset;
		trans.parent = source.trans;
		gameUI.Start();
		amount = money;
		StartCoroutine(Drop());
	}
	
	Vector3 GetAngledMovementVector()
	{
		float lean = Random.value - .5f;
		Vector3 temp = new Vector3(-dropDist, 0, -dropDist);
		if (lean > 0)
		{
			temp.x += dropDist * lean * 2;
		}
		else
		{
			temp.z += dropDist * -lean * 2;
		}
		return temp;
	}
	
	IEnumerator Drop()
	{
		float time = 0;
		startPos = Vector3.zero;
		endPos = GetAngledMovementVector();
		endPos.y = 0;
		Vector3 currOffset;
		while(time < DROP_TIME && !clicked)
		{
			currOffset = Vector3.Lerp(startPos, endPos, time/DROP_TIME);
			currOffset.y = -Mathf.Pow(((time/DROP_TIME - 0.3f)*1.7f), 2f) + 1f;
			if (float.IsNaN(currOffset.y))
			{
				currOffset.y = 0;
			}
			time += Time.deltaTime;
			trans.localPosition = currOffset;
			yield return null;
		}
		StartCoroutine(WaitForClick());
	}
	
	IEnumerator WaitForClick()
	{
		yield return new WaitForSeconds(TIME_TO_PICKUP);
		clicked = true;
	}
	
	IEnumerator SendToMenu()
	{
		trans.parent = Camera.main.transform;
		yield return null;
		CBKResourceManager.instance.Collect(CBKResourceManager.ResourceType.FREE, amount);
		if (CBKEventManager.Quest.OnMoneyCollected != null)
		{
			CBKEventManager.Quest.OnMoneyCollected(amount);
		}
		Pool();
	}
	
	void OnClick()
	{
		clicked = true;
	}
	
	public void Pool ()
	{
		CBKPoolManager.instance.Pool(this);
	}
}
