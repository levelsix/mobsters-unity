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
		{EventProtocolResponse.S_SELL_NORM_STRUCTURE_EVENT, typeof(SellNormStructureResponseProto)},
		{EventProtocolResponse.S_UPGRADE_NORM_STRUCTURE_EVENT, typeof(UpgradeNormStructureResponseProto)},
		{EventProtocolResponse.S_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT, typeof(RetrieveCurrencyFromNormStructureResponseProto)},
		{EventProtocolResponse.S_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT, typeof(FinishNormStructWaittimeWithDiamondsResponseProto)},
		{EventProtocolResponse.S_NORM_STRUCT_WAIT_COMPLETE_EVENT, typeof(NormStructWaitCompleteResponseProto)},
		{EventProtocolResponse.S_UPDATE_CLIENT_USER_EVENT, typeof(UpdateClientUserResponseProto)},
		{EventProtocolResponse.S_SEND_GROUP_CHAT_EVENT, typeof(SendGroupChatResponseProto)},
		{EventProtocolResponse.S_LOAD_CITY_EVENT, typeof(LoadCityResponseProto)},
		{EventProtocolResponse.S_QUEST_PROGRESS_EVENT, typeof(QuestProgressResponseProto)},
		{EventProtocolResponse.S_QUEST_ACCEPT_EVENT, typeof(QuestAcceptResponseProto)},
		{EventProtocolResponse.S_RETRIEVE_STATIC_DATA_EVENT, typeof(RetrieveStaticDataResponseProto)},
		{EventProtocolResponse.S_QUEST_REDEEM_EVENT, typeof(QuestRedeemResponseProto)},
		{EventProtocolResponse.S_BEGIN_DUNGEON_EVENT, typeof(BeginDungeonResponseProto)},
		{EventProtocolResponse.S_UPDATE_MONSTER_HEALTH_EVENT, typeof(UpdateMonsterHealthResponseProto)},
		{EventProtocolResponse.S_END_DUNGEON_EVENT, typeof(EndDungeonResponseProto)},
		{EventProtocolResponse.S_ADD_MONSTER_TO_BATTLE_TEAM_EVENT, typeof(AddMonsterToBattleTeamResponseProto)},
		{EventProtocolResponse.S_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT, typeof(RemoveMonsterFromBattleTeamResponseProto)},
		{EventProtocolResponse.S_HEAL_MONSTER_EVENT, typeof(HealMonsterResponseProto)},
		{EventProtocolResponse.S_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT, typeof(HealMonsterWaitTimeCompleteResponseProto)},
		{EventProtocolResponse.S_COMBINE_USER_MONSTER_PIECES_EVENT, typeof(CombineUserMonsterPiecesResponseProto)},
		{EventProtocolResponse.S_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT, typeof(EnhancementWaitTimeCompleteResponseProto)},
		{EventProtocolResponse.S_SUBMIT_MONSTER_ENHANCEMENT_EVENT, typeof(SubmitMonsterEnhancementResponseProto)}
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
