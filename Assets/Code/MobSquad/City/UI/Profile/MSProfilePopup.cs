using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class MSProfilePopup : MonoBehaviour {

	#region UI Elements

	[SerializeField]
	GameObject statsElements;

	[SerializeField]
	GameObject teamElements;

	[SerializeField]
	MSTab statsTab;

	[SerializeField]
	MSTab teamTab;

	[SerializeField]
	MSChatAvatar avatar;

	[SerializeField]
	UILabel playerName;

	[SerializeField]
	MSProfileTeammate[] teammates;

	[SerializeField]
	MSProfileStats stats;

	#endregion

	public FullUserProto fullUser;
	
	public MinimumUserProtoWithLevel minUserWithLevel
	{
		get
		{
			MinimumUserProtoWithLevel minu = new MinimumUserProtoWithLevel();
			minu.level = fullUser.level;
			minu.minUserProto.avatarMonsterId = fullUser.avatarMonsterId;
			minu.minUserProto.clan = fullUser.clan;
			minu.minUserProto.name = fullUser.name;
			minu.minUserProto.userId = fullUser.userId;
			return minu;
		}
	}

	public void Popup(int userId)
	{
		MSActionManager.Popup.OnPopup(GetComponent<MSPopup>());
		Init (userId);
	}

	public void Init(int userId)
	{
		statsElements.SetActive(false);
		teamElements.SetActive(false);

		statsTab.InitActive();
		teamTab.InitInactive();

		playerName.text = "Loading...";
		avatar.alpha = 0;

		RetrieveUsersForUserIdsRequestProto request = new RetrieveUsersForUserIdsRequestProto();
		request.sender = MSWhiteboard.localMup;
		request.includeCurMonsterTeam = true;
		request.requestedUserIds.Add(userId);
		UMQNetworkManager.instance.SendRequest(request, (int)EventProtocolRequest.C_RETRIEVE_USERS_FOR_USER_IDS_EVENT, OnRetrieveResponse);
	}

	void OnRetrieveResponse(int tagNum)
	{
		RetrieveUsersForUserIdsResponseProto response = UMQNetworkManager.responseDict[tagNum] as RetrieveUsersForUserIdsResponseProto;
		UMQNetworkManager.responseDict.Remove (tagNum);

		fullUser = response.requestedUsers[0];
		avatar.alpha = 1;
		avatar.Init(fullUser.avatarMonsterId);
		playerName.text = fullUser.name;
		stats.Init(fullUser);
		statsElements.SetActive(true);

		for (int i = 0; i < teammates.Length; i++) 
		{
			teammates[i].Init(response.curTeam[0].currentTeam.Find(x=>x.teamSlotNum == i+1));
		}
		teammates[0].Select();
	}

	public void SelectStats()
	{
		statsElements.SetActive(true);
		teamElements.SetActive(false);
		statsTab.InitActive();
		teamTab.InitInactive ();
	}

	public void SelectTeam()
	{
		statsElements.SetActive(false);
		teamElements.SetActive (true);
		statsTab.InitInactive();
		teamTab.InitActive();
	}
}
