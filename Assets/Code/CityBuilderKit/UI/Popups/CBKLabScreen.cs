using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

public class CBKLabScreen : MonoBehaviour {
	
	#region UI elements
	
	[SerializeField]
	UILabel titleLabel;
	
	[SerializeField]
	List<CBKGoonCard> cards;
	
	[SerializeField]
	CBKMiniHealingBox baseEnhanceBox;
	
	[SerializeField]
	UILabel baseInfoLabel;
	
	[SerializeField]
	List<CBKMiniHealingBox> boxes;
	
	[SerializeField]
	UILabel selectBaseLabel;
	
	[SerializeField]
	UILabel selectFeedersLabel;
	
	[SerializeField]
	Color currStateColor;
	
	[SerializeField]
	Color greyedOutColor;
	
	[SerializeField]
	CBKGoonCard goonCardPrefab;
	
	#endregion
	
	static readonly Vector3 cardOffset = new Vector3(240, 0, 0);
	
	void OrganizeCards(Dictionary<long, PZMonster> monsters)
	{
		while(cards.Count < monsters.Count)
		{
			CBKGoonCard card = Instantiate(goonCardPrefab) as CBKGoonCard;
			card.transform.parent = cards[cards.Count-1].transform.parent;
			card.transform.localScale = Vector3.one;
			card.transform.localPosition = cards[cards.Count-1].transform.localPosition + cardOffset;
			cards.Add(card);
		}
		
		int i = 0;
		foreach (PZMonster item in monsters.Values) 
		{
			cards[i].Init(item);
		}
	}
}
