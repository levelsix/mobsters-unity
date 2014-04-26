using UnityEngine;
using System.Collections;
using com.lvl6.proto;

/// <summary>
/// CBK booster buyer.
/// </summary>
public class MSBoosterBuyer : MonoBehaviour {

	IEnumerator BuyBooster(int packId)
	{
		PurchaseBoosterPackRequestProto request = new PurchaseBoosterPackRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.boosterPackId = packId;
		request.clientTime = MSUtil.timeNowMillis;

		int tagNum = UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PURCHASE_BOOSTER_PACK_EVENT, null);

		while (!UMQNetworkManager.responseDict.ContainsKey (tagNum))
		{
			yield return null;
		}

		PurchaseBoosterPackResponseProto response = UMQNetworkManager.responseDict[tagNum] as PurchaseBoosterPackResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus.SUCCESS)
		{
			MSMonsterManager.instance.UpdateOrAddAll(response.updatedOrNew);

			//TODO: Send response.prize to the UI for display
		}
		else
		{
			Debug.LogWarning("Purchase booster fail: " + response.status.ToString());
		}
	}
}
