using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// The controller for the chat popup
/// </summary>
using System;


public class MSChatPopup : MonoBehaviour {

	#region UI Parts
	
	[SerializeField]
	UIInput inputField;

	[SerializeField]
	MSTab globalChatButton;

	[SerializeField]
	MSTab clanChatButton;

	[SerializeField]
	MSTab privateChatButton;

	[SerializeField]
	MSChatGrid privateGrid;

	[SerializeField]
	UIGrid privateChatGrid;

	[SerializeField]
	MSPrivateChatEntry privateChatEntryPrefab;
	
	/// <summary>
	/// The mover.
	/// Private -> Chat
	/// </summary>
	[SerializeField]
	TweenPosition mover;

	[SerializeField]
	MSChatAvatar myAvatar;

	[SerializeField]
	GameObject notInClanParent;

	[SerializeField]
	GameObject noPrivateChatsParent;

	public MSBadge helpNotification;

	#endregion

	const string CHEAT_PREFIX = "#~#";

	//Implemented Cheats
	const string GEMS_CHEAT = "gemsgalore";
	const string CASH_CHEAT = "fastcash";
	const string OIL_CHEAT = "oilspill";
	const string OIL_AND_CASH_CHEAT = "greedisgood";
	public const string RESET_CHEAT = "cleanslate";
	const string PURGE_CHEAT = "purgecash";
	const string SKIP_QUESTS_CHEAT = "quickquests";

	//Skill cheats
	const string SET_SKILL_CHEAT = "skill";
	const string GOO_SKILL = "goo";
	const string CAKE_SKILL = "cake";
	const string QUICK_SKILL = "atk";
	const string BOMB_SKILL = "bomb";
	const string POISON_SKILL = "poison";
	const string MOMENTUM_SKILL = "momentum";
	const string RAGE_SKILL = "rage";
	const string SHIELD_SKILL = "shield";

	//Not implemented Cheats
	const string UNLOCK_BUILDINGS_CHEAT = "unlockdown"; //Currently unlocks tasks instead of buildings
	const string UNMUTE_CHEAT = "allears";
	const string FB_LOGOUT_CHEAT = "unfb";

	const string LVL6_CHEAT = "mister8conrad3chan9is1a2very4great5man";

	List<MSPrivateChatEntry> privateChats = new List<MSPrivateChatEntry>();

	public MinimumUserProtoWithLevel privateChatter;
	
	public void SendChatMessage()
	{

		if (inputField.label.text.EndsWith("|"))
		{
			inputField.label.text = inputField.label.text.Substring(0, inputField.label.text.Length - 1);
		}

		if (inputField.label.text.Length > 3 && inputField.label.text.Substring(0, 3) == CHEAT_PREFIX)
		{
			if (inputField.label.text.StartsWith(CHEAT_PREFIX + OIL_CHEAT))
			{
				try
				{
					int amount = Convert.ToInt32(inputField.label.text.Substring((CHEAT_PREFIX + OIL_CHEAT).Length+1));
					MSResourceManager.instance.CheatMoney(amount, DevRequest.F_B_GET_OIL);
					MSResourceManager.instance.Collect(ResourceType.OIL, amount);
//					Debug.Log("Gaining " + amount + " Oil");
				}
				catch (Exception e)
				{
					Debug.Log("Exception: " + e);
				}
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + CASH_CHEAT))
			{
				try
				{
					int amount = Convert.ToInt32(inputField.label.text.Substring((CHEAT_PREFIX + CASH_CHEAT).Length+1));
					MSResourceManager.instance.CheatMoney(amount, DevRequest.F_B_GET_CASH);
					MSResourceManager.instance.Collect(ResourceType.CASH, amount);
//					Debug.Log("Gaining " + amount + " Cash");
				}
				catch (Exception e)
				{
					Debug.Log("Exception: " + e);
				}
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + OIL_AND_CASH_CHEAT))
			{
				try
				{
					int amount = Convert.ToInt32(inputField.label.text.Substring((CHEAT_PREFIX + OIL_AND_CASH_CHEAT).Length+1));
					MSResourceManager.instance.CheatMoney(amount, DevRequest.F_B_GET_CASH_OIL_GEMS);
					MSResourceManager.instance.Collect(ResourceType.CASH, amount);
					MSResourceManager.instance.Collect(ResourceType.OIL, amount);
					MSResourceManager.instance.Collect(ResourceType.GEMS, amount);
//					Debug.Log("Gaining " + amount + " Cash and Oil and Gems");
				}
				catch (Exception e)
				{
					Debug.Log("Exception: " + e);
				}
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + GEMS_CHEAT))
			{
				try
				{
					int amount = Convert.ToInt32(inputField.label.text.Substring((CHEAT_PREFIX + GEMS_CHEAT).Length+1));
					MSResourceManager.instance.CheatMoney(amount, DevRequest.F_B_GET_GEMS);
					MSResourceManager.instance.Collect(ResourceType.GEMS, amount);
//					Debug.Log("Gaining " + amount + " Gems");
				}
				catch (Exception e)
				{
					Debug.Log("Exception: " + e);
				}
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + UNLOCK_BUILDINGS_CHEAT))
			{
				MSQuestManager.instance.CheatCompleteAllTasks();
//				Debug.Log("Unlocking buildings (well, tasks...)");
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + SKIP_QUESTS_CHEAT))
			{
				MSQuestManager.instance.CheatCompleteAllTasks();
//				Debug.Log("Unlocking Tasks");
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + RESET_CHEAT))
			{
				MSResourceManager.instance.CheatReset();
//				Debug.Log("Resetting account");
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + PURGE_CHEAT))
			{
				Caching.CleanCache();
//				Debug.Log("Cleaning Cache");
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + SET_SKILL_CHEAT))
			{
				int skill = 0;

				List<SkillProto> allSkills = MSDataManager.instance.GetList<SkillProto>();

				if (inputField.label.text.EndsWith(GOO_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.JELLY).skillId;
				}
				else if (inputField.label.text.EndsWith(CAKE_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.CAKE_DROP).skillId;
				}
				else if (inputField.label.text.EndsWith(QUICK_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.QUICK_ATTACK).skillId;
				}
				else if (inputField.label.text.EndsWith(BOMB_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.BOMBS).skillId;
				}
				else if (inputField.label.text.EndsWith(POISON_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.POISON).skillId;
				}
				else if (inputField.label.text.EndsWith(MOMENTUM_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.MOMENTUM).skillId;
				}
				else if (inputField.label.text.EndsWith(SHIELD_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.SHIELD).skillId;
				}
				else if (inputField.label.text.EndsWith(RAGE_SKILL))
				{
					skill = allSkills.Find(x=>x.type==SkillType.ROID_RAGE).skillId;
				}

				if (inputField.label.text.StartsWith(CHEAT_PREFIX + SET_SKILL_CHEAT + "e_"))
				{
					PZCombatManager.instance.forceEnemySkill = skill;
				}
				else
				{
					PZCombatManager.instance.forcePlayerSkill = skill;
				}
			}
		}
		else if (inputField.label.text.Length > 0)
		{
			if (MSChatManager.instance.currMode == MSValues.ChatMode.PRIVATE)
			{
				PrivateChatPostRequestProto request = new PrivateChatPostRequestProto();
				request.sender = MSWhiteboard.localMup;
				request.recipientUuid = privateChatter.minUserProto.userUuid;
				request.content = inputField.label.text;

				UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PRIVATE_CHAT_POST_EVENT, CheckPrivateChatResponse);
			}
			else
			{
				SendGroupChatRequestProto request = new SendGroupChatRequestProto();
				
				request.sender = MSWhiteboard.localMup;
				if(MSChatManager.instance.currMode == MSValues.ChatMode.CLAN)
				{
//					Debug.Log("Sending Clan Message");
					request.scope = GroupChatScope.CLAN;
				}
				else
				{
//					Debug.Log("Sending global message\n"
//					          + "Name: " + request.sender.name
//					          + "Avatar: " + request.sender.avatarMonsterId);
					request.scope = GroupChatScope.GLOBAL;
				}
				request.chatMessage = inputField.label.text;
				request.clientTime = MSUtil.timeNowMillis;
				
				UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolResponse.S_SEND_GROUP_CHAT_EVENT, CheckServerChatResponse);
			}
		}

		inputField.value = "";
	}

	void CheckPrivateChatResponse(int tagNum)
	{
		PrivateChatPostResponseProto response = UMQNetworkManager.responseDict[tagNum] as PrivateChatPostResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);

		if (response.status == PrivateChatPostResponseProto.PrivateChatPostStatus.SUCCESS)
		{
			MSChatManager.instance.ReceivePrivateChatMessage(response);
		}
		else
		{
			Debug.LogError("Problem sending private message: " + response.status.ToString());
		}
	}

	/// <summary>
	/// Makes sure that our chat went through and it's not innapropro or nuttin'
	/// </summary>
	void CheckServerChatResponse(int tagNum)
	{
		SendGroupChatResponseProto response = UMQNetworkManager.responseDict[tagNum] as SendGroupChatResponseProto;
		UMQNetworkManager.responseDict.Remove(tagNum);
		
		if (response.status != SendGroupChatResponseProto.SendGroupChatStatus.SUCCESS)
		{
			Debug.LogError("Problem sending chat message: " + response.status.ToString());
		}
	}
	
	void OnEnable()
	{
		MSChatBubbleOptions.instance.Close(true);
		SetGlobalChat();
	}

	public void Init(MSValues.ChatMode mode)
	{
		myAvatar.Init(MSWhiteboard.localUser.avatarMonsterId);

		inputField.label.text = "Type your message";

		if (MSChatBubbleOptions.instance != null)
		{
			MSChatBubbleOptions.instance.Close(true);
		}

		switch(mode)
		{
		case MSValues.ChatMode.GLOBAL:
			SetGlobalChat();
			break;
		case MSValues.ChatMode.CLAN:
			SetClanChat();
			break;
		case MSValues.ChatMode.PRIVATE:
			SetPrivateChat();
			break;
		}
	}

	public void SetGlobalChat()
	{

		MSChatManager.instance.clanGrid.gameObject.SetActive(false);
		MSChatManager.instance.globalGrid.gameObject.SetActive(true);
		MSChatManager.instance.privateGrid.gameObject.SetActive(false);

		notInClanParent.SetActive(false);
		noPrivateChatsParent.SetActive(false);
		mover.gameObject.SetActive(true);
		mover.Sample(1, true);
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.GLOBAL);

		globalChatButton.InitActive();
		clanChatButton.InitInactive();
		privateChatButton.InitInactive();
	}

	public void SetClanChat()
	{

		MSChatManager.instance.clanGrid.gameObject.SetActive(true);
		MSChatManager.instance.globalGrid.gameObject.SetActive(false);
		MSChatManager.instance.privateGrid.gameObject.SetActive(false);

		globalChatButton.InitInactive();
		clanChatButton.InitActive();
		privateChatButton.InitInactive();

		noPrivateChatsParent.SetActive(false);
		if (MSClanManager.instance.isInClan)
		{
			notInClanParent.SetActive(false);
			mover.gameObject.SetActive(true);
			mover.Sample(1, true);
			MSChatManager.instance.SetChatMode(MSValues.ChatMode.CLAN);
		}
		else
		{
			mover.gameObject.SetActive(false);
			notInClanParent.SetActive(true);
		}
	}

	public void SetPrivateChat()
	{

		MSChatManager.instance.clanGrid.gameObject.SetActive(false);
		MSChatManager.instance.globalGrid.gameObject.SetActive(false);
		MSChatManager.instance.privateGrid.gameObject.SetActive(true);

		globalChatButton.InitInactive();
		clanChatButton.InitInactive();
		privateChatButton.InitActive();

		notInClanParent.SetActive(false);
		if (MSChatManager.instance.hasPrivateChats)
		{
			noPrivateChatsParent.SetActive(false);
			mover.gameObject.SetActive(true);
			mover.Sample(0, true);
			MSChatManager.instance.SetChatMode(MSValues.ChatMode.PRIVATE);
		}
		else
		{
			noPrivateChatsParent.SetActive(true);
			mover.Sample(0, true);
		}
	}

	public void AdminChat()
	{
//		Debug.Log("Admin schmat");
		MSChatManager.instance.GoToPrivateChat(MSChatManager.instance.adminUser);
	}

	public void GoToPrivateChat(MinimumUserProtoWithLevel otherPlayer,
	                            List<PrivateChatPostProto> chat,
	                            bool slide = false)
	{
		MSChatBubbleOptions.instance.Close(true);
		if (slide)
		{
			mover.PlayForward();
		}
		else
		{
			mover.Sample(1, true);
		}

		privateChatter = otherPlayer;

		privateGrid.SpawnBubbles(chat);
		
		globalChatButton.InitInactive();
		clanChatButton.InitInactive();
		privateChatButton.InitInactive();
	}

	void RecyclePrivates()
	{
		foreach (var item in privateChats) 
		{
			item.GetComponent<MSSimplePoolable>().Pool();
		}

		privateChats.Clear();
	}

	MSPrivateChatEntry Add(KeyValuePair<string, List<PrivateChatPostProto>> pair)
	{
		MSPrivateChatEntry entry = (MSPoolManager.instance.Get(privateChatEntryPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, privateChatGrid.transform) 
		                            as MSSimplePoolable).GetComponent<MSPrivateChatEntry>();
		entry.transform.localScale = Vector3.one;
		privateChats.Add(entry);
		entry.Init(pair.Value[0]);
		foreach (var item in entry.GetComponents<UIWidget>()) 
		{
			item.ParentHasChanged();
		}
		return entry;
	}

	public void SetupPrivateChatListing(Dictionary<string, List<PrivateChatPostProto>> chats)
	{
		if (chats.Count == 0)
		{

		}
		else
		{
			RecyclePrivates();
			foreach (var item in chats) 
			{
				MSPrivateChatEntry entry = Add(item);
			}
			privateChatGrid.Reposition();
		}
	}

	public void DoRepositionAllGrids()
	{
		StartCoroutine(RepositionAllGrids());
	}

	IEnumerator RepositionAllGrids()
	{
		yield return null;

		MSChatManager.instance.globalGrid.table.animateSmoothly = false;
		MSChatManager.instance.globalGrid.table.Reposition();
		
		MSChatManager.instance.clanGrid.table.animateSmoothly = false;
		MSChatManager.instance.clanGrid.table.Reposition();
		
		MSChatManager.instance.privateGrid.table.animateSmoothly = false;
		MSChatManager.instance.privateGrid.table.Reposition();

		privateChatGrid.animateSmoothly = false;
		privateChatGrid.Reposition();
	}
}
