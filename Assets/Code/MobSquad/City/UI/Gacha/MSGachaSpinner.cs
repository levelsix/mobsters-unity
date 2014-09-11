using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class MSGachaSpinner : MonoBehaviour {
	
	[SerializeField]
	List<MSGachaItem> items;
	
	[SerializeField]
	float minSpinTime = 5;
	
	bool spinning = false;
	
	bool canStop = true;	
	
	[SerializeField]
	float currSpeed;
	
	[SerializeField]
	float timeToLand;
	
	[SerializeField]
	MSGachaReveal reveal;
	
	BoosterPackProto boosterPack;
	
	[SerializeField]
	UICenterOnChild spinnerCenter;
	
	[SerializeField]
	SpringPanel spinnerSpring;
	
	public MSGachaItem lastToLoop = null;
	
	public void Init(BoosterPackProto pack)
	{
		boosterPack = pack;
		
		foreach (var item in items) 
		{
			item.Init(pack);
		}
	}
	
	IEnumerator SpinTimes(int times)
	{
		for (int i = 0; i < times; i++) 
		{
			yield return StartCoroutine(Spin());
		}
		
		spinnerSpring.onFinished = delegate { MSActionManager.Popup.OnPopup(reveal.GetComponent<MSPopup>()); };
		
	}
	
	public void SpinOnce()
	{
		if( MSUtil.timeSince(MSWhiteboard.localUser.lastFreeBoosterPackTime) > 24 * 60 * 60 * 1000)
		{
			StartCoroutine(SpinTimes(1));
		}
		else if (MSResourceManager.instance.Spend(ResourceType.GEMS, boosterPack.gemPrice))
		{
			StartCoroutine(SpinTimes(1));
		}
	}
	
	public void ThisGoesToEleven()
	{
		if (MSResourceManager.instance.Spend(ResourceType.GEMS, boosterPack.gemPrice*10))
		{
			StartCoroutine(SpinOutOfControl(5));
		}
	}
	
	public IEnumerator SpinOutOfControl(int rolls)
	{
		List<BoosterItemProto> prizes = new List<BoosterItemProto>();
		
		PurchaseBoosterPackRequestProto request = new PurchaseBoosterPackRequestProto();
		for (int i = 0; i < rolls; i++)
		{
			request.sender = MSWhiteboard.localMup;
			request.boosterPackId = boosterPack.boosterPackId;
			request.clientTime = MSUtil.timeNowMillis;
			
			int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PURCHASE_BOOSTER_PACK_EVENT, null);
			
			spinnerSpring.enabled = true;
			while (!UMQNetworkManager.responseDict.ContainsKey (tagNum))
			{
				spinnerSpring.target = spinnerSpring.transform.localPosition + new Vector3(1000, 0, 0);
				spinnerSpring.strength = currSpeed;
				yield return null;
			}
			
			PurchaseBoosterPackResponseProto response = UMQNetworkManager.responseDict[tagNum] as PurchaseBoosterPackResponseProto;
			UMQNetworkManager.responseDict.Remove(tagNum);
			
			if (response.status == PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus.SUCCESS)
			{
				MSMonsterManager.instance.UpdateOrAddAll(response.updatedOrNew);
				
				prizes.Add(response.prize);
				Debug.Log("Prize: " + response.prize.boosterItemId + ", " + response.prize.isComplete + ", " + response.prize.monsterId);
			}
			else
			{
				Debug.LogWarning("Purchase booster fail: " + response.status.ToString());
			}
		}
		
		float currTime = 0;
		while (currTime < minSpinTime)
		{
			currTime += Time.deltaTime;
			spinnerSpring.target = spinnerSpring.transform.localPosition + new Vector3(1000, 0, 0);
			spinnerSpring.strength = currSpeed + 3 * currSpeed * currTime/minSpinTime;
			yield return null;
		}
		
		reveal.Init(prizes);
		MSActionManager.Popup.OnPopup(reveal.GetComponent<MSPopup>());
	}
	
	public IEnumerator Spin()
	{
		
		PurchaseBoosterPackRequestProto request = new PurchaseBoosterPackRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.boosterPackId = boosterPack.boosterPackId;
		request.clientTime = MSUtil.timeNowMillis;
		//request.freeBoosterPack = MSUtil.timeSince(MSWhiteboard.localUser.lastFreeBoosterPackTime) > 24 * 60 * 60 * 1000;
		
		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PURCHASE_BOOSTER_PACK_EVENT, null);
		
		StartCoroutine(SpinForTime(minSpinTime));
		
		canStop = false;
		
		while (!UMQNetworkManager.responseDict.ContainsKey (tagNum))
		{
			yield return null;
		}
		
		PurchaseBoosterPackResponseProto response = UMQNetworkManager.responseDict[tagNum] as PurchaseBoosterPackResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		canStop = true;
		
		while(spinning)
		{
			yield return null;
		}
		
		if (response.status == PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus.SUCCESS)
		{
			MSMonsterManager.instance.UpdateOrAddAll(response.updatedOrNew);
			
			reveal.Init(response.prize);
			Debug.Log("Prize: " + response.prize.boosterItemId + ", " + response.prize.isComplete + ", " + response.prize.monsterId);

			if(MSActionManager.Gacha.OnPurchaseBoosterSucces != null)
			{
				MSActionManager.Gacha.OnPurchaseBoosterSucces();
			}
		}
		else
		{
			Debug.LogWarning("Purchase booster fail: " + response.status.ToString());
			if(MSActionManager.Gacha.OnPurchaseBoosterFail != null)
			{
				MSActionManager.Gacha.OnPurchaseBoosterFail();
			}
		}
		
		MSGachaItem theOne = lastToLoop;
		
		theOne.Setup(response.prize);
		
		spinnerCenter.CenterOn(theOne.transform);
		
		//spinnerSpring.strength = currSpeed;
		//theOne.label.text = "THE ONE";
	}
	
	IEnumerator SpinForTime(float seconds)
	{
		spinning = true;
		float currTime = 0;
		spinnerSpring.enabled = true;
		while (currTime < seconds || !canStop)
		{
			//Debug.Log("Spinning...");
			currTime += Time.deltaTime;
			spinnerSpring.target = spinnerSpring.transform.localPosition + new Vector3(1000, 0, 0);
			spinnerSpring.strength = currSpeed;
			yield return null;
		}
		spinning = false;
	}
}