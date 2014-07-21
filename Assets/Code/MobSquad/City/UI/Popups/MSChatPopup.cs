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
	MSChatGrid chatGrid;

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

	const string CHEAT_PREFIX = "#~#";
	const string GEMS_CHEAT = "gemsgalore";
	const string UNLOCK_BUILDINGS_CHEAT = "unlockdown";
	public const string RESET_CHEAT = "cleanslate";
	const string CASH_CHEAT = "fastcash";
	const string OIL_CHEAT = "oilcheat";
	const string PURGE_CHEAT = "purgecash";
	const string SKIP_QUESTS_CHEAT = "quickquests";
	const string OIL_AND_CASH_CHEAT = "greedisgood";


	#endregion

	List<MSPrivateChatEntry> privateChats = new List<MSPrivateChatEntry>();

	public MinimumUserProtoWithLevel privateChatter;
	
	public void SendChatMessage()
	{

		if (inputField.label.text.EndsWith("|"))
		{
			inputField.label.text = inputField.label.text.Substring(0, inputField.label.text.Length - 1);
		}

		if (inputField.label.text.Length > 3 && inputField.label.text.Substring(0, 3) == "#~#")
		{
			if (inputField.label.text.StartsWith(CHEAT_PREFIX + OIL_CHEAT))
			{
				try
				{
					int amount = Convert.ToInt32(inputField.label.text.Substring((CHEAT_PREFIX + OIL_CHEAT).Length+1));
					MSResourceManager.instance.CheatMoney(0, amount, 0, OIL_CHEAT);
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
					MSResourceManager.instance.CheatMoney(amount, 0, 0, CASH_CHEAT);
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
					MSResourceManager.instance.CheatMoney(amount, amount, 0, OIL_AND_CASH_CHEAT);
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
					MSResourceManager.instance.CheatMoney(0, 0, amount, GEMS_CHEAT);
				}
				catch (Exception e)
				{
					Debug.Log("Exception: " + e);
				}
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + UNLOCK_BUILDINGS_CHEAT))
			{
				MSQuestManager.instance.CheatCompleteAllTasks();
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + SKIP_QUESTS_CHEAT))
			{
				MSQuestManager.instance.CheatCompleteAllTasks();
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + RESET_CHEAT))
			{
				
			}
			else if (inputField.label.text.StartsWith(CHEAT_PREFIX + PURGE_CHEAT))
			{
				
			}
		}
		else if (inputField.label.text.Length > 0)
		{
			if (MSChatManager.instance.currMode == MSValues.ChatMode.PRIVATE)
			{
				PrivateChatPostRequestProto request = new PrivateChatPostRequestProto();
				request.sender = MSWhiteboard.localMup;
				request.recipientId = privateChatter.minUserProto.userId;
				request.content = inputField.label.text;

				UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_PRIVATE_CHAT_POST_EVENT, CheckPrivateChatResponse);
			}
			else
			{
				SendGroupChatRequestProto request = new SendGroupChatRequestProto();
				
				request.sender = MSWhiteboard.localMup;
				if(MSChatManager.instance.currMode == MSValues.ChatMode.CLAN)
				{
					Debug.Log("Sending Clan Message");
					request.scope = GroupChatScope.CLAN;
				}
				else
				{
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
		SetGlobalChat();
	}

	public void Init(MSValues.ChatMode mode)
	{
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
		mover.Sample(1, true);
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.GLOBAL);

		globalChatButton.InitActive();
		clanChatButton.InitInactive();
		privateChatButton.InitInactive();
	}

	public void SetClanChat()
	{
		mover.Sample(1, true);
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.CLAN);
		
		globalChatButton.InitInactive();
		clanChatButton.InitActive();
		privateChatButton.InitInactive();
	}

	public void SetPrivateChat()
	{
		mover.Sample(0, true);
		MSChatManager.instance.SetChatMode(MSValues.ChatMode.PRIVATE);
		
		globalChatButton.InitInactive();
		clanChatButton.InitInactive();
		privateChatButton.InitActive();
	}

	public void GoToPrivateChat(MinimumUserProtoWithLevel otherPlayer,
	                            SortedList<long, PrivateChatPostProto> chat,
	                            bool slide = false)
	{
		if (slide)
		{
			mover.PlayForward();
		}
		else
		{
			mover.Sample(1, true);
		}

		privateChatter = otherPlayer;

		chatGrid.SpawnBubbles(chat);
		
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

	MSPrivateChatEntry Add(KeyValuePair<int, SortedList<long, PrivateChatPostProto>> pair)
	{
		MSPrivateChatEntry entry = (MSPoolManager.instance.Get(privateChatEntryPrefab.GetComponent<MSSimplePoolable>(), Vector3.zero, privateChatGrid.transform) 
		                            as MSSimplePoolable).GetComponent<MSPrivateChatEntry>();
		entry.transform.localScale = Vector3.one;
		privateChats.Add(entry);
		entry.Init(pair.Value.Values[0]);
		foreach (var item in entry.GetComponents<UIWidget>()) 
		{
			item.ParentHasChanged();
		}
		return entry;
	}

	public void SetupPrivateChatListing(Dictionary<int, SortedList<long, PrivateChatPostProto>> chats)
	{
		if (chats.Count == 0)
		{

		}
		else
		{
			RecyclePrivates();
			foreach (var pair in chats) 
			{
				MSPrivateChatEntry entry = Add(pair);
			}
			privateChatGrid.Reposition();
		}
	}
}
