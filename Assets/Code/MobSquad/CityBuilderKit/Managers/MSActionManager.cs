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
		public static Action<CBKUnit> OnUnitSelect;
		
		/// <summary>
		/// The place building event, which gives the signal to the
		/// selected building to place itself on the grid.
		/// </summary>
		public static Action PlaceBuilding;
		
		public static Action<MSBuilding> OnCollectFromBuilding;
	}
	
	public static class Loading
	{
		public static Action<StartupResponseProto> OnStartup;

		public static Action LoadBuildings;
		public static Action OnBuildingsLoaded;
	}
	
	public static class Popup
	{
		public static Action<GameObject> OnPopup;
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
		
		public static Action<CBKFullQuest> OnQuestEntryClicked;
		public static Action OnReorganizeHealQueue;

		public static Action<ReceivedGroupChatResponseProto> OnGroupChatReceived;

		public static Action OnRequestsAcceptOrReject;
	}
	
	public static class Goon
	{
		public static Action<PZMonster> OnMonsterAddTeam;
		public static Action<PZMonster> OnMonsterRemoveTeam;
		public static Action OnTeamChanged;
		public static Action OnHealQueueChanged;
		public static Action OnEnhanceQueueChanged;
		public static Action<PZMonster> OnMonsterFinishHeal;
		public static Action<long> OnMonsterRemoved;

		public static Action OnMonsterListChanged;
	}
	
	public static class Cam
	{
		public static Action OnCameraChangeOrientation;
	}
	
	public static class Quest
	{
		public static Action<int> OnStructureBuilt;
		public static Action<int, int> OnStructureUpgraded;
		public static Action<int> OnMonsterDefeated;
		public static Action<int> OnMonsterDonated;
		public static Action<int> OnEquipObtained;
		public static Action<int> OnTaskCompleted;
		public static Action<int> OnMoneyCollected;
	}
	
	public static class Puzzle
	{
		public static Action<PZMonster> OnDeploy;
		public static Action<int> OnTurnChange;
		public static Action<int> OnComboChange;
		public static Action OnNewPlayerTurn;
		public static Action OnGemMatch;

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
}
