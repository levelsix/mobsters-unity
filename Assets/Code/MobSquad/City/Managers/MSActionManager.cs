using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;

/// <summary>
/// Event Manager
/// All events that need to trigger actions on 
/// </summary>
public static class MSActionManager 
{
	
	/// <summary>
	/// All Control events. 
	/// These will only be called by AOC2ControlManager.
	/// Other managers and systems which tie into controls should
	/// read from these events.
	/// </summary>
	public static class Controls
	{
		//All touch events are indexed using how many touches were involved in their action
		//This way, listening to multi-touch events is done by listening to the index of touches-1
		//Example, OnTap[0] fires on a 1-finger tap; OnDoubleTap[2] fires on a three-fingered double-tap
		public static Action<TCKTouchData>[] OnTap = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnStartHold = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnKeepHold = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnReleaseHold = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnStartDrag = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnKeepDrag = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnReleaseDrag = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnFlick = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		public static Action<TCKTouchData>[] OnDoubleTap = new Action<TCKTouchData>[TCKControlManager.MAX_TOUCHES];
		
		/// <summary>
		/// The on pinch event. The float passed along with it
		/// reflects the change in pinch distance in this frame.
		/// </summary>
		public static Action<float> OnPinch;
	}
	
	public static class Town
	{
		/// <summary>
		/// The on building select event.
		/// Notifies UI that we've selected a new building
		/// </summary>
		public static Action<MSBuilding> OnBuildingSelect;
		
		/// <summary>
		/// The on unit select event.
		/// Notifies the UI that we've selected a unit.
		/// </summary>
		public static Action<MSUnit> OnUnitSelect;
		
		/// <summary>
		/// The place building event, which gives the signal to the
		/// selected building to place itself on the grid.
		/// </summary>
		public static Action PlaceBuilding;
		
		public static Action<MSBuilding> OnCollectFromBuilding;

		/// <summary>
		/// Activates UI changes for when a building is dragged
		/// </summary>
		public static Action OnBuildingDragStart;

		/// <summary>
		/// Activates UI changes for when a building is done being dragged
		/// </summary>
		public static Action OnBuildingDragEnd;

		public static Action<MSObstacle> OnObstacleRemoved;
	}
	
	public static class Loading
	{
		public static Action<StartupResponseProto> OnStartup;

		public static Action LoadBuildings;
		public static Action OnBuildingsLoaded;
	}
	
	public static class Popup
	{
		public static Action<MSPopup> OnPopup;
		public static Action<int> ClosePopupLayer;
		public static Action CloseTopPopupLayer;
		public static Action CloseAllPopups;
		public static Action<string> CreatePopup;
		public static Action<string, string[], Action[]> CreateButtonPopup;
	}
	
	public static class UI
	{
		public static Action<Camera> OnCameraResize;
		public static Action<int>[] OnChangeResource = new Action<int>[3];
		public static Action<int[]> OnSetResourceMaxima;
        public static Action OnCameraLockButton;
        public static Action OnCameraSnapButton;
		
		public static Action<MSFullQuest> OnQuestEntryClicked;
		public static Action OnReorganizeHealQueue;

		public static Action<ReceivedGroupChatResponseProto> OnGroupChatReceived;
		public static Action<PrivateChatPostResponseProto> OnPrivateChatReceived;

		public static Action OnRequestsAcceptOrReject;

		public static Action OnDialogueClicked;
	}
	
	public static class Goon
	{
		public static Action<PZMonster> OnMonsterAddTeam;
		public static Action<PZMonster> OnMonsterRemoveTeam;
		public static Action<PZMonster> OnMonsterAddQueue;
		public static Action<PZMonster> OnMonsterRemoveQueue;
		public static Action OnTeamChanged;
		public static Action OnHealQueueChanged;
		public static Action OnEnhanceQueueChanged;
		public static Action<PZMonster> OnMonsterFinishHeal;
		public static Action<long> OnMonsterRemovedFromPlayerInventory;

		public static Action OnMonsterListChanged;
	}
	
	public static class Cam
	{
		public static Action OnCameraChangeOrientation;
	}
	
	public static class Quest
	{
		public static Action<int> OnStructureUpgraded;
		public static Action<BeginDungeonResponseProto> OnTaskCompleted;
		public static Action<ResourceType, int> OnMoneyCollected;

		public static Action<BattleStats> OnBattleFinish;

		/// <summary>
		/// Called whenever a monster is finished being enhanced
		/// Parameter: Number of Enhance Points gained
		/// </summary>
		public static Action<int> OnMonsterEnhanced;

		/// <summary>
		/// Called whenever the player moves up a league in PvP rankings
		/// Parameter: League number (id?)
		/// </summary>
		public static Action<int> OnLeagueJoined;

		/// <summary>
		/// Called whenever monsters are sold
		/// Parameter: The number of monsters sold
		/// </summary>
		public static Action<int> OnMonstersSold;
	}

	public static class Pvp
	{
		public static Action<int, int> OnPvpVictory;
	}
	
	public static class Puzzle
	{
		public static Action<PZMonster> OnDeploy;
		public static Action<int> OnTurnChange;
		public static Action<int> OnComboChange;
		public static Action OnNewPlayerTurn;
		public static Action OnGemMatch;
		public static Action OnGemPressed;

		public static Action ForceHideSwap;
		public static Action ForceShowSwap;
	}
	
	public static class Scene
	{
		public static Action OnCity;
		public static Action OnPuzzle;
	}

	public static class Clan
	{
		public static Action<int, UserClanStatus> OnPlayerClanChange;

		/// <summary>
		/// The on create clan event.
		/// Integer passed through is the int for the player's clan.
		/// If int is zero, then a create request failed.
		/// </summary>
		public static Action<int> OnCreateClan;

		public static Action OnRaidBegin;
		public static Action<AttackClanRaidMonsterResponseProto> OnRaidMonsterAttacked;
		public static Action<AttackClanRaidMonsterResponseProto> OnRaidMonsterDied;
	}

	public static class Tutorial
	{
		public static Action OnTutorialContinue;
	}

	public static class Chat
	{

	}
}
