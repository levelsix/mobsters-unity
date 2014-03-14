#define DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.lvl6.proto;
using System;
using ProtoBuf;
using System.IO;

/// <summary>
/// @author Rob Giusti
/// UMQ deserializer.
/// Hacky, but it's how we've gotta make it work
/// </summary>
public static class UMQDeserializer {
	
	static MySerializer ser = new MySerializer();
	
	static Dictionary<EventProtocolResponse, Type> dict = new Dictionary<EventProtocolResponse, Type>()
	{
		{EventProtocolResponse.S_STARTUP_EVENT, typeof(StartupResponseProto)},
		{EventProtocolResponse.S_USER_CREATE_EVENT, typeof(UserCreateResponseProto)},
		{EventProtocolResponse.S_LOAD_PLAYER_CITY_EVENT, typeof(LoadPlayerCityResponseProto)},
		{EventProtocolResponse.S_MOVE_OR_ROTATE_NORM_STRUCTURE_EVENT, typeof(MoveOrRotateNormStructureResponseProto)},
		{EventProtocolResponse.S_PURCHASE_NORM_STRUCTURE_EVENT, typeof(PurchaseNormStructureResponseProto)},
		//	{EventProtocolResponse.S_SELL_NORM_STRUCTURE_EVENT, typeof(SellNormStructureResponseProto)},
		{EventProtocolResponse.S_UPGRADE_NORM_STRUCTURE_EVENT, typeof(UpgradeNormStructureResponseProto)},
		{EventProtocolResponse.S_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT, typeof(RetrieveCurrencyFromNormStructureResponseProto)},
		{EventProtocolResponse.S_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT, typeof(FinishNormStructWaittimeWithDiamondsResponseProto)},
		{EventProtocolResponse.S_NORM_STRUCT_WAIT_COMPLETE_EVENT, typeof(NormStructWaitCompleteResponseProto)},
		{EventProtocolResponse.S_UPDATE_CLIENT_USER_EVENT, typeof(UpdateClientUserResponseProto)},
		{EventProtocolResponse.S_SEND_GROUP_CHAT_EVENT, typeof(SendGroupChatResponseProto)},
		{EventProtocolResponse.S_LOAD_CITY_EVENT, typeof(LoadCityResponseProto)},
		{EventProtocolResponse.S_QUEST_PROGRESS_EVENT, typeof(QuestProgressResponseProto)},
		{EventProtocolResponse.S_QUEST_ACCEPT_EVENT, typeof(QuestAcceptResponseProto)},
		{EventProtocolResponse.S_QUEST_REDEEM_EVENT, typeof(QuestRedeemResponseProto)},
		{EventProtocolResponse.S_BEGIN_DUNGEON_EVENT, typeof(BeginDungeonResponseProto)},
		{EventProtocolResponse.S_UPDATE_MONSTER_HEALTH_EVENT, typeof(UpdateMonsterHealthResponseProto)},
		{EventProtocolResponse.S_END_DUNGEON_EVENT, typeof(EndDungeonResponseProto)},
		{EventProtocolResponse.S_ADD_MONSTER_TO_BATTLE_TEAM_EVENT, typeof(AddMonsterToBattleTeamResponseProto)},
		{EventProtocolResponse.S_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT, typeof(RemoveMonsterFromBattleTeamResponseProto)},
		{EventProtocolResponse.S_HEAL_MONSTER_EVENT, typeof(HealMonsterResponseProto)},
		{EventProtocolResponse.S_COMBINE_USER_MONSTER_PIECES_EVENT, typeof(CombineUserMonsterPiecesResponseProto)},
		{EventProtocolResponse.S_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT, typeof(EnhancementWaitTimeCompleteResponseProto)},
		{EventProtocolResponse.S_SUBMIT_MONSTER_ENHANCEMENT_EVENT, typeof(SubmitMonsterEnhancementResponseProto)},
		{EventProtocolResponse.S_PURGE_STATIC_DATA_EVENT, typeof(PurgeClientStaticDataResponseProto)},
		{EventProtocolResponse.S_CREATE_CLAN_EVENT, typeof(CreateClanResponseProto)},
		{EventProtocolResponse.S_LEAVE_CLAN_EVENT, typeof(LeaveClanResponseProto)},
		{EventProtocolResponse.S_REQUEST_JOIN_CLAN_EVENT, typeof(RequestJoinClanResponseProto)},
		{EventProtocolResponse.S_RETRACT_REQUEST_JOIN_CLAN_EVENT, typeof(RetractRequestJoinClanResponseProto)},
		{EventProtocolResponse.S_APPROVE_OR_REJECT_REQUEST_TO_JOIN_CLAN_EVENT, typeof(ApproveOrRejectRequestToJoinClanResponseProto)},
		{EventProtocolResponse.S_RETRIEVE_CLAN_INFO_EVENT, typeof(RetrieveClanInfoResponseProto)},
		{EventProtocolResponse.S_TRANSFER_CLAN_OWNERSHIP, typeof(TransferClanOwnershipResponseProto)},
		{EventProtocolResponse.S_CHANGE_CLAN_DESCRIPTION_EVENT, typeof(ChangeClanDescriptionResponseProto)},
		{EventProtocolResponse.S_BOOT_PLAYER_FROM_CLAN_EVENT, typeof(BootPlayerFromClanResponseProto)},
		{EventProtocolResponse.S_CHANGE_CLAN_JOIN_TYPE_EVENT, typeof(ChangeClanJoinTypeResponseProto)},
		{EventProtocolResponse.S_LEVEL_UP_EVENT, typeof(LevelUpResponseProto)},
		{EventProtocolResponse.S_RECEIVED_GROUP_CHAT_EVENT, typeof(ReceivedGroupChatResponseProto)},
		{EventProtocolResponse.S_PURCHASE_BOOSTER_PACK_EVENT, typeof(PurchaseBoosterPackResponseProto)},
		{EventProtocolResponse.S_INVITE_FB_FRIENDS_FOR_SLOTS_EVENT, typeof(InviteFbFriendsForSlotsResponseProto)},
		{EventProtocolResponse.S_ACCEPT_AND_REJECT_FB_INVITE_FOR_SLOTS_EVENT, typeof(AcceptAndRejectFbInviteForSlotsResponseProto)},
		{EventProtocolResponse.S_INCREASE_MONSTER_INVENTORY_SLOT_EVENT, typeof(IncreaseMonsterInventorySlotResponseProto)},
		{EventProtocolResponse.S_SELL_USER_MONSTER_EVENT, typeof(SellUserMonsterResponseProto)},
		{EventProtocolResponse.S_EXCHANGE_GEMS_FOR_RESOURCES_EVENT, typeof(ExchangeGemsForResourcesResponseProto)},
		{EventProtocolResponse.S_EVOLVE_MONSTER_EVENT, typeof(EvolveMonsterResponseProto)},
		{EventProtocolResponse.S_EVOLUTION_FINISHED_EVENT, typeof(EvolutionFinishedResponseProto)},
		{EventProtocolResponse.S_QUEUE_UP_EVENT, typeof(QueueUpResponseProto)},
		{EventProtocolResponse.S_BEGIN_PVP_BATTLE_EVENT, typeof(BeginPvpBattleResponseProto)},
		{EventProtocolResponse.S_END_PVP_BATTLE_EVENT, typeof(EndPvpBattleResponseProto)},
		{EventProtocolResponse.S_UPDATE_USER_CURRENCY_EVENT, typeof(UpdateUserCurrencyResponseProto)},
		{EventProtocolResponse.S_FORCE_LOGOUT_EVENT, typeof(ForceLogoutResponseProto)},
		{EventProtocolResponse.S_BEGIN_CLAN_RAID_EVENT, typeof(BeginClanRaidResponseProto)},
		{EventProtocolResponse.S_ATTACK_CLAN_RAID_MONSTER_EVENT, typeof(AttackClanRaidMonsterResponseProto)},
		{EventProtocolResponse.S_AWARD_CLAN_RAID_STAGE_REWARD_EVENT, typeof(AwardClanRaidStageRewardResponseProto)},
		{EventProtocolResponse.S_RECORD_CLAN_RAID_STATS_EVENT, typeof(RecordClanRaidStatsResponseProto)}
	};
	
	public static object Deserialize(Stream stream, EventProtocolResponse type)
	{
		object result = null;
		
#if DEBUG
		Debug.Log("Deserializing type: " + type.ToString());
#endif
		
		result = ser.Deserialize(stream, result, dict[type]);
		
		return result;
	}
	
}
