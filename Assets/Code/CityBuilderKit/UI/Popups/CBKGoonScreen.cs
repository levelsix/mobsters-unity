using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;

public class CBKGoonScreen : MonoBehaviour {
	
	#region UI Elements
	
	[SerializeField]
	CBKGoonCard[] teamCards;
	
	[SerializeField]
	List<CBKGoonCard> reserveCards;
	
	[SerializeField]
	List<CBKMiniHealingBox> healBoxes;
	
	[SerializeField]
	CBKGoonCard goonCardPrefab;
	
	[SerializeField]
	CBKMiniHealingBox healBoxPrefab;
	
	[SerializeField]
	GameObject healingQueueParent;
	
	[SerializeField]
	CBKActionButton healingQueueSpeedUpButton;
	
	[SerializeField]
	UILabel healingQueueTotalTime;
	
	#endregion
	
	#region Properties
	
	int playerSlots
	{
		get
		{
			return CBKWhiteboard.constants.userMonsterConstants.maxNumTeamSlots + CBKWhiteboard.localUser.numAdditionalMonsterSlots;
		}
	}
			
	int lastIndex
	{
		get
		{
			return reserveCards.Count - 1;
		}
	}
	
	int lastBox
	{
		get
		{
			return healBoxes.Count - 1;
		}
	}
	
	#endregion
	
	#region Constants
	
	static readonly Vector3 cardOffset = new Vector3(240, 0, 0);
	
	static readonly Vector3 boxOffset = new Vector3(-142, 0, 0);
	
	const string removeDialogueBeforeCost = "Are you sure you want to remove this goon from the healing queue? You will be refunded $";
	const string removeDialogueAfterCost = " of the healing cost";
	
	#endregion
	
	public void Init(PZMonster[] teamGoons, Dictionary<long, PZMonster> playerGoons, List<PZMonster> healings)
	{
		#region Set Up Cards
		
		int i;
		for (i = 0; i < teamGoons.Length; i++) 
		{
			
			if (teamGoons[i] == null || teamGoons[i].monster.monsterId <= 0)
			{
				Debug.Log("Init empty");
				teamCards[i].InitEmptyTeam();
			}
			else
			{
				Debug.Log("Team slot not empty");
				teamCards[i].Init(teamGoons[i]);
			}
		}
		
		OrganizeReserveCards (teamGoons, playerGoons);
		
		#endregion
		
		#region Bottom Healing Queue
		
		OrganizeHealingQueue (healings);
		
		#endregion
	}
	
	void OnEnable()
	{
		CBKEventManager.Goon.OnMonsterAddTeam += OnAddTeamMember;
		CBKEventManager.Goon.OnMonsterRemoveTeam += OnRemoveTeamMember;
		CBKEventManager.Goon.OnHealQueueChanged += OnHealQueueChanged;
		healingQueueSpeedUpButton.onClick += TrySpeedUpHeal;
		
		Init (CBKMonsterManager.instance.userTeam, CBKMonsterManager.instance.userMonsters, CBKMonsterManager.instance.healingMonsters);
	}
	
	void OnDisable()
	{
		CBKEventManager.Goon.OnMonsterAddTeam -= OnAddTeamMember;
		CBKEventManager.Goon.OnMonsterRemoveTeam -= OnRemoveTeamMember;
		CBKEventManager.Goon.OnHealQueueChanged -= OnHealQueueChanged;
		healingQueueSpeedUpButton.onClick -= TrySpeedUpHeal;
	}
	
	void OrganizeReserveCards()
	{
		OrganizeReserveCards(CBKMonsterManager.instance.userTeam, CBKMonsterManager.instance.userMonsters);
	}
	
	void OrganizeReserveCards (PZMonster[] teamGoons, Dictionary<long, PZMonster> playerGoons)
	{
		int i = 0;
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
		
		for (i = playerGoons.Count - CBKMonsterManager.instance.monstersOnTeam; i < playerSlots - CBKMonsterManager.instance.monstersOnTeam; i++)
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
		reserveCards[i].InitSlotForPurchase();
		
		while (++i < reserveCards.Count)
		{
			reserveCards[i].gameObject.SetActive(false);
		}
	}
	
	void OrganizeHealingQueue()
	{
		OrganizeHealingQueue(CBKMonsterManager.instance.healingMonsters);
	}
	
	void OrganizeHealingQueue(List<PZMonster> healingMonsters)
	{
		if (healingMonsters.Count == 0)
		{
			healingQueueParent.SetActive(false);
			return;
		}
		
		healingQueueParent.SetActive(true);
		
		while(healBoxes.Count < healingMonsters.Count)
		{
			AddHealBox();
		}
		int i;
		for (i = 0; i < healingMonsters.Count; i++) 
		{
			healBoxes[i].Init (healingMonsters[i]);
			healBoxes[i].SetBar(i==0);
		}
		for (; i < healBoxes.Count; i++) 
		{
			healBoxes[i].gameObject.SetActive(false);
		}
	}
	
	void AddReserveCardSlot()
	{
		CBKGoonCard card = Instantiate(goonCardPrefab) as CBKGoonCard;
		card.transform.parent = reserveCards[lastIndex].transform.parent;
		card.transform.localScale = Vector3.one;
		card.transform.localPosition = reserveCards[lastIndex].transform.localPosition + cardOffset;
		reserveCards.Add (card);
	}
	
	void AddHealBox()
	{
		CBKMiniHealingBox box = Instantiate(healBoxPrefab) as CBKMiniHealingBox;
		box.transform.parent = healBoxes[lastBox].transform.parent;
		box.transform.localScale = Vector3.one;
		box.transform.localPosition = healBoxes[lastBox].transform.localPosition + boxOffset;
		healBoxes.Add (box);
	}
	
	void Update()
	{
		if (healingQueueParent.activeSelf)
		{
			long timeLeft = CBKMonsterManager.instance.healingMonsters[CBKMonsterManager.instance.healingMonsters.Count-1].healTimeLeft;
			healingQueueTotalTime.text = CBKUtil.TimeStringShort(timeLeft);
			healingQueueSpeedUpButton.label.text = Mathf.Ceil((float)timeLeft/6000).ToString(); //TODO: Proper formula here
		}
	}
	
	void TrySpeedUpHeal()
	{
		int gemCost = Mathf.CeilToInt((CBKMonsterManager.instance.healingMonsters[CBKMonsterManager.instance.healingMonsters.Count-1].timeToHealMillis) * 1f/6000);
		if (CBKResourceManager.instance.resources[(int)CBKResourceManager.ResourceType.PREMIUM] >= gemCost)
		{
			CBKResourceManager.instance.Spend(CBKResourceManager.ResourceType.PREMIUM, gemCost);
			CBKMonsterManager.instance.SpeedUpHeal(gemCost);
		}
		else
		{
			CBKEventManager.Popup.CreatePopup("Not enough Gems");
		}
	}
	
	void OnHealQueueChanged()
	{
		OrganizeHealingQueue();
		OrganizeReserveCards();
	}
	
	void OnRemoveTeamMember(PZMonster monster)
	{
		OrganizeReserveCards();
	}
	
	void OnAddTeamMember(PZMonster monster)
	{
		teamCards[monster.userMonster.teamSlotNum-1].Init(monster);
		OrganizeReserveCards();
	}
}
