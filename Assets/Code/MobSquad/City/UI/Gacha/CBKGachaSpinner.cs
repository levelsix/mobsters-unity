using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKGachaSpinner : MonoBehaviour {

	[SerializeField]
	List<CBKGachaItem> items;

	[SerializeField]
	float spinSpeed;

	/// <summary>
	/// The friction, applied as negative acceleration
	/// </summary>
	[SerializeField]
	float friction;

	bool spinning = false;

	[SerializeField]
	float currSpeed;

	[SerializeField]
	float timeToLand;

	int lastPack = 0;

	[SerializeField]
	CBKGachaReveal reveal;

	public CBKGachaItem lastToLoop = null;

	public void Init(BoosterPackProto pack)
	{
		foreach (var item in items) 
		{
			item.Init(pack);
			item.spinner = this;
		}
	}

	void OnDrag(Vector2 delta)
	{
		//Debug.Log("Dragged!");
		if (!spinning)
		{
			delta.y = 0;
			currSpeed = delta.x / Time.deltaTime;
		}
	}

	void Update()
	{
		if (Mathf.Abs(currSpeed) > 5)
		{
			Move (currSpeed * Time.deltaTime);
			currSpeed -= ((currSpeed>0) ? friction : -friction) * Time.deltaTime;

		}
	}

	void Move(Vector2 dist)
	{
		foreach (var item in items) 
		{
			item.Drag (dist);
		}
	}

	void Move(float speed)
	{
		Vector2 drag = new Vector2(speed, 0);
		Move (drag);
	}

	public void Spin()
	{
		StartCoroutine(Spin(lastPack));
	}

	public IEnumerator Spin(int packId)
	{
		lastPack = packId;

		spinning = true;

		PurchaseBoosterPackRequestProto request = new PurchaseBoosterPackRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.boosterPackId = packId;
		request.clientTime = MSUtil.timeNowMillis;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PURCHASE_BOOSTER_PACK_EVENT, null);
		
		while (!UMQNetworkManager.responseDict.ContainsKey (tagNum))
		{
			currSpeed = spinSpeed;
			yield return null;
		}
		
		PurchaseBoosterPackResponseProto response = UMQNetworkManager.responseDict[tagNum] as PurchaseBoosterPackResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status == PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus.SUCCESS)
		{
			MSMonsterManager.instance.UpdateOrAddAll(response.updatedOrNew);

			reveal.Init(response.prize);
			Debug.Log("Prize: " + response.prize.monsterId);
		}
		else
		{
			Debug.LogWarning("Purchase booster fail: " + response.status.ToString());
		}

		//Now finish the spinning
		while(currSpeed > 300)
		{
			yield return null;
		}

		currSpeed = 0;

		CBKGachaItem theOne = lastToLoop;

		theOne.label.text = "THIS ONE";

		float startX = lastToLoop.trans.localPosition.x;
		float currTime = 0;
		float lerp;
		float posToBe;
		float posAt;
		while (currTime < timeToLand)
		{
			currTime += Time.deltaTime;

			lerp = (currTime/timeToLand - 1);
			lerp *= lerp;

			posToBe = lerp * startX;
			posAt = theOne.trans.localPosition.x;

			//Debug.Log("Moving from " + posAt + " to " + posToBe);

			Move (posToBe - posAt);

			yield return null;

		}

		MSActionManager.Popup.OnPopup(reveal.GetComponent<MSPopup>());

		spinning = false;
	}

	[ContextMenu ("Organize")]
	void Oragnize()
	{
		for(int i = 0; i < items.Count; i++)
		{
			items[i].transform.localPosition = new Vector3(i * 140, items[i].transform.localPosition.y);
		}
	}

	[ContextMenu ("SetSpeed")]
	void SetSpeed()
	{
		currSpeed = spinSpeed;
	}

}
