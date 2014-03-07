//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Clan.proto
// Note: requires additional types generated from: Battle.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: Structure.proto
// Note: requires additional types generated from: Task.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullClanProto")]
  public partial class FullClanProto : global::ProtoBuf.IExtensible
  {
    public FullClanProto() {}
    

    private int _clanId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"clanId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanId
    {
      get { return _clanId; }
      set { _clanId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private long _createTime = default(long);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"createTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long createTime
    {
      get { return _createTime; }
      set { _createTime = value; }
    }

    private string _description = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string description
    {
      get { return _description; }
      set { _description = value; }
    }

    private string _tag = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"tag", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string tag
    {
      get { return _tag; }
      set { _tag = value; }
    }

    private bool _requestToJoinRequired = default(bool);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"requestToJoinRequired", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool requestToJoinRequired
    {
      get { return _requestToJoinRequired; }
      set { _requestToJoinRequired = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullUserClanProto")]
  public partial class FullUserClanProto : global::ProtoBuf.IExtensible
  {
    public FullUserClanProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _clanId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanId
    {
      get { return _clanId; }
      set { _clanId = value; }
    }

    private com.lvl6.proto.UserClanStatus _status = com.lvl6.proto.UserClanStatus.LEADER;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UserClanStatus.LEADER)]
    public com.lvl6.proto.UserClanStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private long _requestTime = default(long);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"requestTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long requestTime
    {
      get { return _requestTime; }
      set { _requestTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullClanProtoWithClanSize")]
  public partial class FullClanProtoWithClanSize : global::ProtoBuf.IExtensible
  {
    public FullClanProtoWithClanSize() {}
    

    private com.lvl6.proto.FullClanProto _clan = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"clan", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.FullClanProto clan
    {
      get { return _clan; }
      set { _clan = value; }
    }

    private int _clanSize = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanSize", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanSize
    {
      get { return _clanSize; }
      set { _clanSize = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserProtoForClans")]
  public partial class MinimumUserProtoForClans : global::ProtoBuf.IExtensible
  {
    public MinimumUserProtoForClans() {}
    

    private com.lvl6.proto.MinimumUserProtoWithBattleHistory _minUserProto = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"minUserProto", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithBattleHistory minUserProto
    {
      get { return _minUserProto; }
      set { _minUserProto = value; }
    }

    private com.lvl6.proto.UserClanStatus _clanStatus = com.lvl6.proto.UserClanStatus.LEADER;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanStatus", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UserClanStatus.LEADER)]
    public com.lvl6.proto.UserClanStatus clanStatus
    {
      get { return _clanStatus; }
      set { _clanStatus = value; }
    }

    private float _raidContribution = default(float);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"raidContribution", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float raidContribution
    {
      get { return _raidContribution; }
      set { _raidContribution = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ClanRaidProto")]
  public partial class ClanRaidProto : global::ProtoBuf.IExtensible
  {
    public ClanRaidProto() {}
    

    private int _clanRaidId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"clanRaidId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidId
    {
      get { return _clanRaidId; }
      set { _clanRaidId = value; }
    }

    private string _clanRaidName = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanRaidName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string clanRaidName
    {
      get { return _clanRaidName; }
      set { _clanRaidName = value; }
    }

    private string _activeTitleImgName = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"activeTitleImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string activeTitleImgName
    {
      get { return _activeTitleImgName; }
      set { _activeTitleImgName = value; }
    }

    private string _activeBackgroundImgName = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"activeBackgroundImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string activeBackgroundImgName
    {
      get { return _activeBackgroundImgName; }
      set { _activeBackgroundImgName = value; }
    }

    private string _activeDescription = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"activeDescription", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string activeDescription
    {
      get { return _activeDescription; }
      set { _activeDescription = value; }
    }

    private string _inactiveMonsterImgName = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"inactiveMonsterImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string inactiveMonsterImgName
    {
      get { return _inactiveMonsterImgName; }
      set { _inactiveMonsterImgName = value; }
    }

    private string _inactiveDescription = "";
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"inactiveDescription", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string inactiveDescription
    {
      get { return _inactiveDescription; }
      set { _inactiveDescription = value; }
    }

    private string _dialogueText = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"dialogueText", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string dialogueText
    {
      get { return _dialogueText; }
      set { _dialogueText = value; }
    }

    private string _spotlightMonsterImgName = "";
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"spotlightMonsterImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string spotlightMonsterImgName
    {
      get { return _spotlightMonsterImgName; }
      set { _spotlightMonsterImgName = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageProto> _raidStages = new global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageProto>();
    [global::ProtoBuf.ProtoMember(10, Name=@"raidStages", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageProto> raidStages
    {
      get { return _raidStages; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ClanRaidStageProto")]
  public partial class ClanRaidStageProto : global::ProtoBuf.IExtensible
  {
    public ClanRaidStageProto() {}
    

    private int _clanRaidStageId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"clanRaidStageId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidStageId
    {
      get { return _clanRaidStageId; }
      set { _clanRaidStageId = value; }
    }

    private int _clanRaidId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanRaidId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidId
    {
      get { return _clanRaidId; }
      set { _clanRaidId = value; }
    }

    private int _durationMinutes = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"durationMinutes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int durationMinutes
    {
      get { return _durationMinutes; }
      set { _durationMinutes = value; }
    }

    private int _stageNum = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"stageNum", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int stageNum
    {
      get { return _stageNum; }
      set { _stageNum = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageMonsterProto> _monsters = new global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageMonsterProto>();
    [global::ProtoBuf.ProtoMember(11, Name=@"monsters", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageMonsterProto> monsters
    {
      get { return _monsters; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageRewardProto> _possibleRewards = new global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageRewardProto>();
    [global::ProtoBuf.ProtoMember(12, Name=@"possibleRewards", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidStageRewardProto> possibleRewards
    {
      get { return _possibleRewards; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ClanRaidStageMonsterProto")]
  public partial class ClanRaidStageMonsterProto : global::ProtoBuf.IExtensible
  {
    public ClanRaidStageMonsterProto() {}
    

    private int _crsmId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"crsmId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crsmId
    {
      get { return _crsmId; }
      set { _crsmId = value; }
    }

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private int _monsterHp = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"monsterHp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterHp
    {
      get { return _monsterHp; }
      set { _monsterHp = value; }
    }

    private int _minDmg = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"minDmg", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int minDmg
    {
      get { return _minDmg; }
      set { _minDmg = value; }
    }

    private int _maxDmg = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"maxDmg", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int maxDmg
    {
      get { return _maxDmg; }
      set { _maxDmg = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ClanRaidStageRewardProto")]
  public partial class ClanRaidStageRewardProto : global::ProtoBuf.IExtensible
  {
    public ClanRaidStageRewardProto() {}
    

    private int _crsrId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"crsrId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crsrId
    {
      get { return _crsrId; }
      set { _crsrId = value; }
    }

    private int _minOilReward = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"minOilReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int minOilReward
    {
      get { return _minOilReward; }
      set { _minOilReward = value; }
    }

    private int _maxOilReward = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"maxOilReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int maxOilReward
    {
      get { return _maxOilReward; }
      set { _maxOilReward = value; }
    }

    private int _minCashReward = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"minCashReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int minCashReward
    {
      get { return _minCashReward; }
      set { _minCashReward = value; }
    }

    private int _maxCashReward = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"maxCashReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int maxCashReward
    {
      get { return _maxCashReward; }
      set { _maxCashReward = value; }
    }

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentClanEventProto")]
  public partial class PersistentClanEventProto : global::ProtoBuf.IExtensible
  {
    public PersistentClanEventProto() {}
    

    private int _clanEventId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"clanEventId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanEventId
    {
      get { return _clanEventId; }
      set { _clanEventId = value; }
    }

    private com.lvl6.proto.DayOfWeek _dayOfWeek = com.lvl6.proto.DayOfWeek.MONDAY;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"dayOfWeek", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.DayOfWeek.MONDAY)]
    public com.lvl6.proto.DayOfWeek dayOfWeek
    {
      get { return _dayOfWeek; }
      set { _dayOfWeek = value; }
    }

    private int _startHour = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"startHour", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int startHour
    {
      get { return _startHour; }
      set { _startHour = value; }
    }

    private int _eventDurationMinutes = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"eventDurationMinutes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eventDurationMinutes
    {
      get { return _eventDurationMinutes; }
      set { _eventDurationMinutes = value; }
    }

    private int _clanRaidId = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"clanRaidId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidId
    {
      get { return _clanRaidId; }
      set { _clanRaidId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentClanEventClanInfoProto")]
  public partial class PersistentClanEventClanInfoProto : global::ProtoBuf.IExtensible
  {
    public PersistentClanEventClanInfoProto() {}
    

    private int _clanId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"clanId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanId
    {
      get { return _clanId; }
      set { _clanId = value; }
    }

    private int _clanEventId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanEventId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanEventId
    {
      get { return _clanEventId; }
      set { _clanEventId = value; }
    }

    private int _clanRaidId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"clanRaidId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidId
    {
      get { return _clanRaidId; }
      set { _clanRaidId = value; }
    }

    private int _clanRaidStageId = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"clanRaidStageId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidStageId
    {
      get { return _clanRaidStageId; }
      set { _clanRaidStageId = value; }
    }

    private long _stageStartTime = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"stageStartTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long stageStartTime
    {
      get { return _stageStartTime; }
      set { _stageStartTime = value; }
    }

    private int _crsmId = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"crsmId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crsmId
    {
      get { return _crsmId; }
      set { _crsmId = value; }
    }

    private long _stageMonsterStartTime = default(long);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"stageMonsterStartTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long stageMonsterStartTime
    {
      get { return _stageMonsterStartTime; }
      set { _stageMonsterStartTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentClanEventUserInfoProto")]
  public partial class PersistentClanEventUserInfoProto : global::ProtoBuf.IExtensible
  {
    public PersistentClanEventUserInfoProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _clanId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanId
    {
      get { return _clanId; }
      set { _clanId = value; }
    }

    private int _crId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"crId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crId
    {
      get { return _crId; }
      set { _crId = value; }
    }

    private int _crDmgDone = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"crDmgDone", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crDmgDone
    {
      get { return _crDmgDone; }
      set { _crDmgDone = value; }
    }

    private int _crsDmgDone = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"crsDmgDone", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crsDmgDone
    {
      get { return _crsDmgDone; }
      set { _crsDmgDone = value; }
    }

    private int _crsmDmgDone = default(int);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"crsmDmgDone", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crsmDmgDone
    {
      get { return _crsmDmgDone; }
      set { _crsmDmgDone = value; }
    }

    private com.lvl6.proto.UserCurrentMonsterTeamProto _userMonsters = null;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"userMonsters", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.UserCurrentMonsterTeamProto userMonsters
    {
      get { return _userMonsters; }
      set { _userMonsters = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentClanEventUserRewardProto")]
  public partial class PersistentClanEventUserRewardProto : global::ProtoBuf.IExtensible
  {
    public PersistentClanEventUserRewardProto() {}
    

    private int _rewardId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"rewardId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int rewardId
    {
      get { return _rewardId; }
      set { _rewardId = value; }
    }

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private long _crsEndTime = default(long);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"crsEndTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long crsEndTime
    {
      get { return _crsEndTime; }
      set { _crsEndTime = value; }
    }

    private com.lvl6.proto.ResourceType _resourceType = com.lvl6.proto.ResourceType.CASH;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"resourceType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.ResourceType.CASH)]
    public com.lvl6.proto.ResourceType resourceType
    {
      get { return _resourceType; }
      set { _resourceType = value; }
    }

    private int _staticDataId = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"staticDataId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int staticDataId
    {
      get { return _staticDataId; }
      set { _staticDataId = value; }
    }

    private int _quantity = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"quantity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int quantity
    {
      get { return _quantity; }
      set { _quantity = value; }
    }

    private long _timeRedeemed = default(long);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"timeRedeemed", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeRedeemed
    {
      get { return _timeRedeemed; }
      set { _timeRedeemed = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentClanEventRaidStageHistoryProto")]
  public partial class PersistentClanEventRaidStageHistoryProto : global::ProtoBuf.IExtensible
  {
    public PersistentClanEventRaidStageHistoryProto() {}
    
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.PersistentClanEventUserRewardProto> _rewards = new global::System.Collections.Generic.List<com.lvl6.proto.PersistentClanEventUserRewardProto>();
    [global::ProtoBuf.ProtoMember(1, Name=@"rewards", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.PersistentClanEventUserRewardProto> rewards
    {
      get { return _rewards; }
    }
  

    private int _eventId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"eventId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eventId
    {
      get { return _eventId; }
      set { _eventId = value; }
    }

    private int _clanRaidId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"clanRaidId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidId
    {
      get { return _clanRaidId; }
      set { _clanRaidId = value; }
    }

    private int _clanRaidStageId = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"clanRaidStageId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanRaidStageId
    {
      get { return _clanRaidStageId; }
      set { _clanRaidStageId = value; }
    }

    private long _crsEndTime = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"crsEndTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long crsEndTime
    {
      get { return _crsEndTime; }
      set { _crsEndTime = value; }
    }

    private int _crsDmgDone = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"crsDmgDone", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crsDmgDone
    {
      get { return _crsDmgDone; }
      set { _crsDmgDone = value; }
    }

    private int _stageHp = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"stageHp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int stageHp
    {
      get { return _stageHp; }
      set { _stageHp = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentClanEventRaidHistoryProto")]
  public partial class PersistentClanEventRaidHistoryProto : global::ProtoBuf.IExtensible
  {
    public PersistentClanEventRaidHistoryProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _crDmg = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"crDmg", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int crDmg
    {
      get { return _crDmg; }
      set { _crDmg = value; }
    }

    private int _clanCrDmg = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"clanCrDmg", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanCrDmg
    {
      get { return _clanCrDmg; }
      set { _clanCrDmg = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"UserClanStatus")]
    public enum UserClanStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"LEADER", Value=1)]
      LEADER = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"JUNIOR_LEADER", Value=2)]
      JUNIOR_LEADER = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CAPTAIN", Value=3)]
      CAPTAIN = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MEMBER", Value=4)]
      MEMBER = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"REQUESTING", Value=10)]
      REQUESTING = 10
    }
  
}