//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Protocols.proto
// Note: requires additional types generated from: Battle.proto
// Note: requires additional types generated from: BoosterPackStuff.proto
// Note: requires additional types generated from: Chat.proto
// Note: requires additional types generated from: City.proto
// Note: requires additional types generated from: Clan.proto
// Note: requires additional types generated from: EventApns.proto
// Note: requires additional types generated from: EventBoosterPack.proto
// Note: requires additional types generated from: EventChat.proto
// Note: requires additional types generated from: EventCity.proto
// Note: requires additional types generated from: EventClan.proto
// Note: requires additional types generated from: EventDungeon.proto
// Note: requires additional types generated from: EventInAppPurchase.proto
// Note: requires additional types generated from: EventMonster.proto
// Note: requires additional types generated from: EventPvp.proto
// Note: requires additional types generated from: EventQuest.proto
// Note: requires additional types generated from: EventReferral.proto
// Note: requires additional types generated from: EventStartup.proto
// Note: requires additional types generated from: EventStaticData.proto
// Note: requires additional types generated from: EventStructure.proto
// Note: requires additional types generated from: EventTournament.proto
// Note: requires additional types generated from: EventUser.proto
// Note: requires additional types generated from: InAppPurchase.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: Quest.proto
// Note: requires additional types generated from: StaticData.proto
// Note: requires additional types generated from: Structure.proto
// Note: requires additional types generated from: Task.proto
// Note: requires additional types generated from: TournamentStuff.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
    [global::ProtoBuf.ProtoContract(Name=@"EventProtocolRequest")]
    public enum EventProtocolRequest
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_STARTUP_EVENT", Value=1)]
      C_STARTUP_EVENT = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_IN_APP_PURCHASE_EVENT", Value=2)]
      C_IN_APP_PURCHASE_EVENT = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_PURCHASE_NORM_STRUCTURE_EVENT", Value=3)]
      C_PURCHASE_NORM_STRUCTURE_EVENT = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_MOVE_OR_ROTATE_NORM_STRUCTURE_EVENT", Value=4)]
      C_MOVE_OR_ROTATE_NORM_STRUCTURE_EVENT = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_SET_FACEBOOK_ID_EVENT", Value=5)]
      C_SET_FACEBOOK_ID_EVENT = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_UPGRADE_NORM_STRUCTURE_EVENT", Value=6)]
      C_UPGRADE_NORM_STRUCTURE_EVENT = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT", Value=7)]
      C_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT", Value=8)]
      C_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_NORM_STRUCT_WAIT_COMPLETE_EVENT", Value=9)]
      C_NORM_STRUCT_WAIT_COMPLETE_EVENT = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_LOAD_PLAYER_CITY_EVENT", Value=10)]
      C_LOAD_PLAYER_CITY_EVENT = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_EXCHANGE_GEMS_FOR_RESOURCES_EVENT", Value=11)]
      C_EXCHANGE_GEMS_FOR_RESOURCES_EVENT = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_QUEST_ACCEPT_EVENT", Value=12)]
      C_QUEST_ACCEPT_EVENT = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_QUEST_PROGRESS_EVENT", Value=13)]
      C_QUEST_PROGRESS_EVENT = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_QUEST_REDEEM_EVENT", Value=14)]
      C_QUEST_REDEEM_EVENT = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_PURCHASE_CITY_EXPANSION_EVENT", Value=15)]
      C_PURCHASE_CITY_EXPANSION_EVENT = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_EXPANSION_WAIT_COMPLETE_EVENT", Value=16)]
      C_EXPANSION_WAIT_COMPLETE_EVENT = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_LEVEL_UP_EVENT", Value=17)]
      C_LEVEL_UP_EVENT = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_ENABLE_APNS_EVENT", Value=18)]
      C_ENABLE_APNS_EVENT = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_USER_CREATE_EVENT", Value=19)]
      C_USER_CREATE_EVENT = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_LOAD_CITY_EVENT", Value=20)]
      C_LOAD_CITY_EVENT = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_RETRIEVE_USERS_FOR_USER_IDS_EVENT", Value=21)]
      C_RETRIEVE_USERS_FOR_USER_IDS_EVENT = 21,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_EARN_FREE_DIAMONDS_EVENT", Value=22)]
      C_EARN_FREE_DIAMONDS_EVENT = 22,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_SEND_GROUP_CHAT_EVENT", Value=23)]
      C_SEND_GROUP_CHAT_EVENT = 23,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_CREATE_CLAN_EVENT", Value=24)]
      C_CREATE_CLAN_EVENT = 24,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_LEAVE_CLAN_EVENT", Value=25)]
      C_LEAVE_CLAN_EVENT = 25,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_REQUEST_JOIN_CLAN_EVENT", Value=26)]
      C_REQUEST_JOIN_CLAN_EVENT = 26,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_RETRACT_REQUEST_JOIN_CLAN_EVENT", Value=27)]
      C_RETRACT_REQUEST_JOIN_CLAN_EVENT = 27,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_APPROVE_OR_REJECT_REQUEST_TO_JOIN_CLAN_EVENT", Value=28)]
      C_APPROVE_OR_REJECT_REQUEST_TO_JOIN_CLAN_EVENT = 28,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_TRANSFER_CLAN_OWNERSHIP", Value=29)]
      C_TRANSFER_CLAN_OWNERSHIP = 29,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_RETRIEVE_CLAN_INFO_EVENT", Value=30)]
      C_RETRIEVE_CLAN_INFO_EVENT = 30,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_CHANGE_CLAN_DESCRIPTION_EVENT", Value=31)]
      C_CHANGE_CLAN_DESCRIPTION_EVENT = 31,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_BOOT_PLAYER_FROM_CLAN_EVENT", Value=32)]
      C_BOOT_PLAYER_FROM_CLAN_EVENT = 32,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_PICK_LOCK_BOX_EVENT", Value=33)]
      C_PICK_LOCK_BOX_EVENT = 33,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_RETRIEVE_TOURNAMENT_RANKINGS_EVENT", Value=34)]
      C_RETRIEVE_TOURNAMENT_RANKINGS_EVENT = 34,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_SUBMIT_MONSTER_ENHANCEMENT_EVENT", Value=35)]
      C_SUBMIT_MONSTER_ENHANCEMENT_EVENT = 35,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_PURCHASE_BOOSTER_PACK_EVENT", Value=37)]
      C_PURCHASE_BOOSTER_PACK_EVENT = 37,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_CHANGE_CLAN_JOIN_TYPE_EVENT", Value=39)]
      C_CHANGE_CLAN_JOIN_TYPE_EVENT = 39,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_PRIVATE_CHAT_POST_EVENT", Value=40)]
      C_PRIVATE_CHAT_POST_EVENT = 40,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_RETRIEVE_PRIVATE_CHAT_POST_EVENT", Value=41)]
      C_RETRIEVE_PRIVATE_CHAT_POST_EVENT = 41,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_REDEEM_USER_LOCK_BOX_ITEMS_EVENT", Value=42)]
      C_REDEEM_USER_LOCK_BOX_ITEMS_EVENT = 42,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_BEGIN_DUNGEON_EVENT", Value=43)]
      C_BEGIN_DUNGEON_EVENT = 43,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_END_DUNGEON_EVENT", Value=44)]
      C_END_DUNGEON_EVENT = 44,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_REVIVE_IN_DUNGEON_EVENT", Value=45)]
      C_REVIVE_IN_DUNGEON_EVENT = 45,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_QUEUE_UP_EVENT", Value=46)]
      C_QUEUE_UP_EVENT = 46,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_UPDATE_MONSTER_HEALTH_EVENT", Value=47)]
      C_UPDATE_MONSTER_HEALTH_EVENT = 47,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_HEAL_MONSTER_EVENT", Value=48)]
      C_HEAL_MONSTER_EVENT = 48,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT", Value=49)]
      C_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT = 49,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_ADD_MONSTER_TO_BATTLE_TEAM_EVENT", Value=50)]
      C_ADD_MONSTER_TO_BATTLE_TEAM_EVENT = 50,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT", Value=51)]
      C_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT = 51,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_INCREASE_MONSTER_INVENTORY_SLOT_EVENT", Value=52)]
      C_INCREASE_MONSTER_INVENTORY_SLOT_EVENT = 52,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT", Value=53)]
      C_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT = 53,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_COMBINE_USER_MONSTER_PIECES_EVENT", Value=54)]
      C_COMBINE_USER_MONSTER_PIECES_EVENT = 54,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_SELL_USER_MONSTER_EVENT", Value=55)]
      C_SELL_USER_MONSTER_EVENT = 55,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_INVITE_FB_FRIENDS_FOR_SLOTS_EVENT", Value=56)]
      C_INVITE_FB_FRIENDS_FOR_SLOTS_EVENT = 56,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_ACCEPT_AND_REJECT_FB_INVITE_FOR_SLOTS_EVENT", Value=57)]
      C_ACCEPT_AND_REJECT_FB_INVITE_FOR_SLOTS_EVENT = 57,
            
      [global::ProtoBuf.ProtoEnum(Name=@"C_LOGOUT_EVENT", Value=101)]
      C_LOGOUT_EVENT = 101
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"EventProtocolResponse")]
    public enum EventProtocolResponse
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_STARTUP_EVENT", Value=1)]
      S_STARTUP_EVENT = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_IN_APP_PURCHASE_EVENT", Value=2)]
      S_IN_APP_PURCHASE_EVENT = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_PURCHASE_NORM_STRUCTURE_EVENT", Value=3)]
      S_PURCHASE_NORM_STRUCTURE_EVENT = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_MOVE_OR_ROTATE_NORM_STRUCTURE_EVENT", Value=4)]
      S_MOVE_OR_ROTATE_NORM_STRUCTURE_EVENT = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_SET_FACEBOOK_ID_EVENT", Value=5)]
      S_SET_FACEBOOK_ID_EVENT = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_UPGRADE_NORM_STRUCTURE_EVENT", Value=6)]
      S_UPGRADE_NORM_STRUCTURE_EVENT = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT", Value=7)]
      S_RETRIEVE_CURRENCY_FROM_NORM_STRUCTURE_EVENT = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT", Value=8)]
      S_FINISH_NORM_STRUCT_WAITTIME_WITH_DIAMONDS_EVENT = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_NORM_STRUCT_WAIT_COMPLETE_EVENT", Value=9)]
      S_NORM_STRUCT_WAIT_COMPLETE_EVENT = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_LOAD_PLAYER_CITY_EVENT", Value=10)]
      S_LOAD_PLAYER_CITY_EVENT = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_EXCHANGE_GEMS_FOR_RESOURCES_EVENT", Value=11)]
      S_EXCHANGE_GEMS_FOR_RESOURCES_EVENT = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_QUEST_ACCEPT_EVENT", Value=12)]
      S_QUEST_ACCEPT_EVENT = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_QUEST_PROGRESS_EVENT", Value=13)]
      S_QUEST_PROGRESS_EVENT = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_QUEST_REDEEM_EVENT", Value=14)]
      S_QUEST_REDEEM_EVENT = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_PURCHASE_CITY_EXPANSION_EVENT", Value=15)]
      S_PURCHASE_CITY_EXPANSION_EVENT = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_EXPANSION_WAIT_COMPLETE_EVENT", Value=16)]
      S_EXPANSION_WAIT_COMPLETE_EVENT = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_LEVEL_UP_EVENT", Value=17)]
      S_LEVEL_UP_EVENT = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_ENABLE_APNS_EVENT", Value=18)]
      S_ENABLE_APNS_EVENT = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_USER_CREATE_EVENT", Value=19)]
      S_USER_CREATE_EVENT = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_LOAD_CITY_EVENT", Value=20)]
      S_LOAD_CITY_EVENT = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RETRIEVE_USERS_FOR_USER_IDS_EVENT", Value=21)]
      S_RETRIEVE_USERS_FOR_USER_IDS_EVENT = 21,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_EARN_FREE_DIAMONDS_EVENT", Value=22)]
      S_EARN_FREE_DIAMONDS_EVENT = 22,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_SEND_GROUP_CHAT_EVENT", Value=23)]
      S_SEND_GROUP_CHAT_EVENT = 23,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_CREATE_CLAN_EVENT", Value=24)]
      S_CREATE_CLAN_EVENT = 24,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_LEAVE_CLAN_EVENT", Value=25)]
      S_LEAVE_CLAN_EVENT = 25,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_REQUEST_JOIN_CLAN_EVENT", Value=26)]
      S_REQUEST_JOIN_CLAN_EVENT = 26,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RETRACT_REQUEST_JOIN_CLAN_EVENT", Value=27)]
      S_RETRACT_REQUEST_JOIN_CLAN_EVENT = 27,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_APPROVE_OR_REJECT_REQUEST_TO_JOIN_CLAN_EVENT", Value=28)]
      S_APPROVE_OR_REJECT_REQUEST_TO_JOIN_CLAN_EVENT = 28,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_TRANSFER_CLAN_OWNERSHIP", Value=29)]
      S_TRANSFER_CLAN_OWNERSHIP = 29,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RETRIEVE_CLAN_INFO_EVENT", Value=30)]
      S_RETRIEVE_CLAN_INFO_EVENT = 30,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_CHANGE_CLAN_DESCRIPTION_EVENT", Value=31)]
      S_CHANGE_CLAN_DESCRIPTION_EVENT = 31,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_BOOT_PLAYER_FROM_CLAN_EVENT", Value=32)]
      S_BOOT_PLAYER_FROM_CLAN_EVENT = 32,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_PICK_LOCK_BOX_EVENT", Value=33)]
      S_PICK_LOCK_BOX_EVENT = 33,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RETRIEVE_TOURNAMENT_RANKINGS_EVENT", Value=34)]
      S_RETRIEVE_TOURNAMENT_RANKINGS_EVENT = 34,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_SUBMIT_MONSTER_ENHANCEMENT_EVENT", Value=35)]
      S_SUBMIT_MONSTER_ENHANCEMENT_EVENT = 35,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_PURCHASE_BOOSTER_PACK_EVENT", Value=37)]
      S_PURCHASE_BOOSTER_PACK_EVENT = 37,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_CHANGE_CLAN_JOIN_TYPE_EVENT", Value=39)]
      S_CHANGE_CLAN_JOIN_TYPE_EVENT = 39,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_PRIVATE_CHAT_POST_EVENT", Value=40)]
      S_PRIVATE_CHAT_POST_EVENT = 40,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RETRIEVE_PRIVATE_CHAT_POST_EVENT", Value=41)]
      S_RETRIEVE_PRIVATE_CHAT_POST_EVENT = 41,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_REDEEM_USER_LOCK_BOX_ITEMS_EVENT", Value=42)]
      S_REDEEM_USER_LOCK_BOX_ITEMS_EVENT = 42,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_BEGIN_DUNGEON_EVENT", Value=43)]
      S_BEGIN_DUNGEON_EVENT = 43,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_END_DUNGEON_EVENT", Value=44)]
      S_END_DUNGEON_EVENT = 44,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_REVIVE_IN_DUNGEON_EVENT", Value=45)]
      S_REVIVE_IN_DUNGEON_EVENT = 45,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_QUEUE_UP_EVENT", Value=46)]
      S_QUEUE_UP_EVENT = 46,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_UPDATE_MONSTER_HEALTH_EVENT", Value=47)]
      S_UPDATE_MONSTER_HEALTH_EVENT = 47,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_HEAL_MONSTER_EVENT", Value=48)]
      S_HEAL_MONSTER_EVENT = 48,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT", Value=49)]
      S_HEAL_MONSTER_WAIT_TIME_COMPLETE_EVENT = 49,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_ADD_MONSTER_TO_BATTLE_TEAM_EVENT", Value=50)]
      S_ADD_MONSTER_TO_BATTLE_TEAM_EVENT = 50,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT", Value=51)]
      S_REMOVE_MONSTER_FROM_BATTLE_TEAM_EVENT = 51,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_INCREASE_MONSTER_INVENTORY_SLOT_EVENT", Value=52)]
      S_INCREASE_MONSTER_INVENTORY_SLOT_EVENT = 52,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT", Value=53)]
      S_ENHANCEMENT_WAIT_TIME_COMPLETE_EVENT = 53,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_COMBINE_USER_MONSTER_PIECES_EVENT", Value=54)]
      S_COMBINE_USER_MONSTER_PIECES_EVENT = 54,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_SELL_USER_MONSTER_EVENT", Value=55)]
      S_SELL_USER_MONSTER_EVENT = 55,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_INVITE_FB_FRIENDS_FOR_SLOTS_EVENT", Value=56)]
      S_INVITE_FB_FRIENDS_FOR_SLOTS_EVENT = 56,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_ACCEPT_AND_REJECT_FB_INVITE_FOR_SLOTS_EVENT", Value=57)]
      S_ACCEPT_AND_REJECT_FB_INVITE_FOR_SLOTS_EVENT = 57,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_UPDATE_CLIENT_USER_EVENT", Value=101)]
      S_UPDATE_CLIENT_USER_EVENT = 101,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_REFERRAL_CODE_USED_EVENT", Value=102)]
      S_REFERRAL_CODE_USED_EVENT = 102,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_PURGE_STATIC_DATA_EVENT", Value=103)]
      S_PURGE_STATIC_DATA_EVENT = 103,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RECEIVED_GROUP_CHAT_EVENT", Value=104)]
      S_RECEIVED_GROUP_CHAT_EVENT = 104,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_SEND_ADMIN_MESSAGE_EVENT", Value=105)]
      S_SEND_ADMIN_MESSAGE_EVENT = 105,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_GENERAL_NOTIFICATION_EVENT", Value=106)]
      S_GENERAL_NOTIFICATION_EVENT = 106,
            
      [global::ProtoBuf.ProtoEnum(Name=@"S_RECEIVED_RARE_BOOSTER_PURCHASE_EVENT", Value=107)]
      S_RECEIVED_RARE_BOOSTER_PURCHASE_EVENT = 107
    }
  
}