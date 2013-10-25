using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CBKGoonScreen : MonoBehaviour {

	[SerializeField]
	CBKGoonCard[] teamCards;
	
	[SerializeField]
	List<CBKGoonCard> reserveCards;
	
	[SerializeField]
	CBKGoonCard goonCardPrefab;
	
	int lastIndex
	{
		get
		{
			return reserveCards.Count - 1;
		}
	}
	
	Vector3 cardOffset = new Vector3(240, 0, 0);
	
	public void Init(List<PZMonster> teamGoons, Dictionary<long, PZMonster> playerGoons)
	{
		#region Set Up Cards
		
		int playerSlots = CBKWhiteboard.constants.userMonsterConstants.maxNumTeamSlots + CBKWhiteboard.localUser.numAdditionalMonsterSlots;
		
		int i;
		for (i = 0; i < teamGoons.Count; i++) 
		{
			teamCards[i].Init(teamGoons[i]);
		}
		for (;i < 3; i++)
		{
			teamCards[i].InitEmptyTeam();
		}
		
		i = 0;
		foreach (KeyValuePair<long, PZMonster> item in playerGoons) 
		{
			if (item.Value.userMonster.teamSlotNum > 0)
			{
				continue;
			}
			if (lastIndex < i)
			{
				AddReserveCardSlot();
			}
			reserveCards[i++].Init (item.Value);
		}
		
		for (i = playerGoons.Count - teamGoons.Count; i < playerSlots - teamGoons.Count; i++)
		{
			if (lastIndex < i)
			{
				AddReserveCardSlot();
			}
			reserveCards[i].InitEmptyReserve();
		}
		
		if (lastIndex < i)
		{
			AddReserveCardSlot();
		}
		reserveCards[lastIndex].InitSlotForPurchase();
		
		#endregion
		
		#region Bottom Healing Queue
		
		
		
		#endregion
	}
	
	void AddReserveCardSlot()
	{
		CBKGoonCard card = Instantiate(goonCardPrefab) as CBKGoonCard;
		card.transform.parent = reserveCards[lastIndex].transform.parent;
		card.transform.localScale = Vector3.one;
		card.transform.localPosition = reserveCards[lastIndex].transform.localPosition + cardOffset;
		reserveCards.Add (card);
	}
}
