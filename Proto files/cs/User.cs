//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumClanProto")]
  public partial class MinimumClanProto : global::ProtoBuf.IExtensible
  {
    public MinimumClanProto() {}
    

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

    private int _clanIconId = default(int);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"clanIconId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int clanIconId
    {
      get { return _clanIconId; }
      set { _clanIconId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserProto")]
  public partial class MinimumUserProto : global::ProtoBuf.IExtensible
  {
    public MinimumUserProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private com.lvl6.proto.MinimumClanProto _clan = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"clan", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumClanProto clan
    {
      get { return _clan; }
      set { _clan = value; }
    }

    private int _avatarMonsterId = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"avatarMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int avatarMonsterId
    {
      get { return _avatarMonsterId; }
      set { _avatarMonsterId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserProtoWithLevel")]
  public partial class MinimumUserProtoWithLevel : global::ProtoBuf.IExtensible
  {
    public MinimumUserProtoWithLevel() {}
    

    private com.lvl6.proto.MinimumUserProto _minUserProto = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"minUserProto", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto minUserProto
    {
      get { return _minUserProto; }
      set { _minUserProto = value; }
    }

    private int _level = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int level
    {
      get { return _level; }
      set { _level = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserProtoWithFacebookId")]
  public partial class MinimumUserProtoWithFacebookId : global::ProtoBuf.IExtensible
  {
    public MinimumUserProtoWithFacebookId() {}
    

    private com.lvl6.proto.MinimumUserProto _minUserProto = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"minUserProto", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto minUserProto
    {
      get { return _minUserProto; }
      set { _minUserProto = value; }
    }

    private string _facebookId = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"facebookId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string facebookId
    {
      get { return _facebookId; }
      set { _facebookId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserProtoWithMaxResources")]
  public partial class MinimumUserProtoWithMaxResources : global::ProtoBuf.IExtensible
  {
    public MinimumUserProtoWithMaxResources() {}
    

    private com.lvl6.proto.MinimumUserProto _minUserProto = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"minUserProto", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto minUserProto
    {
      get { return _minUserProto; }
      set { _minUserProto = value; }
    }

    private int _maxCash = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"maxCash", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int maxCash
    {
      get { return _maxCash; }
      set { _maxCash = value; }
    }

    private int _maxOil = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"maxOil", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int maxOil
    {
      get { return _maxOil; }
      set { _maxOil = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserFacebookInviteForSlotProto")]
  public partial class UserFacebookInviteForSlotProto : global::ProtoBuf.IExtensible
  {
    public UserFacebookInviteForSlotProto() {}
    

    private int _inviteId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"inviteId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int inviteId
    {
      get { return _inviteId; }
      set { _inviteId = value; }
    }

    private com.lvl6.proto.MinimumUserProtoWithFacebookId _inviter = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"inviter", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithFacebookId inviter
    {
      get { return _inviter; }
      set { _inviter = value; }
    }

    private string _recipientFacebookId = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"recipientFacebookId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string recipientFacebookId
    {
      get { return _recipientFacebookId; }
      set { _recipientFacebookId = value; }
    }

    private long _timeOfInvite = default(long);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"timeOfInvite", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfInvite
    {
      get { return _timeOfInvite; }
      set { _timeOfInvite = value; }
    }

    private long _timeAccepted = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"timeAccepted", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeAccepted
    {
      get { return _timeAccepted; }
      set { _timeAccepted = value; }
    }

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }

    private int _structFbLvl = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"structFbLvl", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int structFbLvl
    {
      get { return _structFbLvl; }
      set { _structFbLvl = value; }
    }

    private long _redeemedTime = default(long);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"redeemedTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long redeemedTime
    {
      get { return _redeemedTime; }
      set { _redeemedTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullUserProto")]
  public partial class FullUserProto : global::ProtoBuf.IExtensible
  {
    public FullUserProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private int _level = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int level
    {
      get { return _level; }
      set { _level = value; }
    }

    private int _gems = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"gems", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gems
    {
      get { return _gems; }
      set { _gems = value; }
    }

    private int _cash = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"cash", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cash
    {
      get { return _cash; }
      set { _cash = value; }
    }

    private int _oil = default(int);
    [global::ProtoBuf.ProtoMember(42, IsRequired = false, Name=@"oil", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int oil
    {
      get { return _oil; }
      set { _oil = value; }
    }

    private int _experience = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"experience", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int experience
    {
      get { return _experience; }
      set { _experience = value; }
    }

    private int _tasksCompleted = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"tasksCompleted", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int tasksCompleted
    {
      get { return _tasksCompleted; }
      set { _tasksCompleted = value; }
    }

    private string _referralCode = "";
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"referralCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string referralCode
    {
      get { return _referralCode; }
      set { _referralCode = value; }
    }

    private int _numReferrals = default(int);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"numReferrals", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numReferrals
    {
      get { return _numReferrals; }
      set { _numReferrals = value; }
    }

    private long _lastLoginTime = default(long);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"lastLoginTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long lastLoginTime
    {
      get { return _lastLoginTime; }
      set { _lastLoginTime = value; }
    }

    private long _lastLogoutTime = default(long);
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"lastLogoutTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long lastLogoutTime
    {
      get { return _lastLogoutTime; }
      set { _lastLogoutTime = value; }
    }

    private bool _isFake = default(bool);
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"isFake", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isFake
    {
      get { return _isFake; }
      set { _isFake = value; }
    }

    private bool _isAdmin = default(bool);
    [global::ProtoBuf.ProtoMember(21, IsRequired = false, Name=@"isAdmin", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isAdmin
    {
      get { return _isAdmin; }
      set { _isAdmin = value; }
    }

    private int _numCoinsRetrievedFromStructs = default(int);
    [global::ProtoBuf.ProtoMember(23, IsRequired = false, Name=@"numCoinsRetrievedFromStructs", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numCoinsRetrievedFromStructs
    {
      get { return _numCoinsRetrievedFromStructs; }
      set { _numCoinsRetrievedFromStructs = value; }
    }

    private int _numOilRetrievedFromStructs = default(int);
    [global::ProtoBuf.ProtoMember(43, IsRequired = false, Name=@"numOilRetrievedFromStructs", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numOilRetrievedFromStructs
    {
      get { return _numOilRetrievedFromStructs; }
      set { _numOilRetrievedFromStructs = value; }
    }

    private com.lvl6.proto.MinimumClanProto _clan = null;
    [global::ProtoBuf.ProtoMember(25, IsRequired = false, Name=@"clan", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumClanProto clan
    {
      get { return _clan; }
      set { _clan = value; }
    }

    private bool _hasReceivedfbReward = default(bool);
    [global::ProtoBuf.ProtoMember(28, IsRequired = false, Name=@"hasReceivedfbReward", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool hasReceivedfbReward
    {
      get { return _hasReceivedfbReward; }
      set { _hasReceivedfbReward = value; }
    }

    private int _numBeginnerSalesPurchased = default(int);
    [global::ProtoBuf.ProtoMember(30, IsRequired = false, Name=@"numBeginnerSalesPurchased", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numBeginnerSalesPurchased
    {
      get { return _numBeginnerSalesPurchased; }
      set { _numBeginnerSalesPurchased = value; }
    }

    private string _facebookId = "";
    [global::ProtoBuf.ProtoMember(40, IsRequired = false, Name=@"facebookId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string facebookId
    {
      get { return _facebookId; }
      set { _facebookId = value; }
    }

    private string _gameCenterId = "";
    [global::ProtoBuf.ProtoMember(45, IsRequired = false, Name=@"gameCenterId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string gameCenterId
    {
      get { return _gameCenterId; }
      set { _gameCenterId = value; }
    }

    private long _lastObstacleSpawnedTime = default(long);
    [global::ProtoBuf.ProtoMember(47, IsRequired = false, Name=@"lastObstacleSpawnedTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long lastObstacleSpawnedTime
    {
      get { return _lastObstacleSpawnedTime; }
      set { _lastObstacleSpawnedTime = value; }
    }

    private int _numObstaclesRemoved = default(int);
    [global::ProtoBuf.ProtoMember(49, IsRequired = false, Name=@"numObstaclesRemoved", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numObstaclesRemoved
    {
      get { return _numObstaclesRemoved; }
      set { _numObstaclesRemoved = value; }
    }

    private int _avatarMonsterId = default(int);
    [global::ProtoBuf.ProtoMember(51, IsRequired = false, Name=@"avatarMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int avatarMonsterId
    {
      get { return _avatarMonsterId; }
      set { _avatarMonsterId = value; }
    }

    private com.lvl6.proto.UserPvpLeagueProto _pvpLeagueInfo = null;
    [global::ProtoBuf.ProtoMember(48, IsRequired = false, Name=@"pvpLeagueInfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.UserPvpLeagueProto pvpLeagueInfo
    {
      get { return _pvpLeagueInfo; }
      set { _pvpLeagueInfo = value; }
    }

    private long _lastMiniJobSpawnedTime = default(long);
    [global::ProtoBuf.ProtoMember(50, IsRequired = false, Name=@"lastMiniJobSpawnedTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long lastMiniJobSpawnedTime
    {
      get { return _lastMiniJobSpawnedTime; }
      set { _lastMiniJobSpawnedTime = value; }
    }

    private long _lastFreeBoosterPackTime = default(long);
    [global::ProtoBuf.ProtoMember(52, IsRequired = false, Name=@"lastFreeBoosterPackTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long lastFreeBoosterPackTime
    {
      get { return _lastFreeBoosterPackTime; }
      set { _lastFreeBoosterPackTime = value; }
    }

    private string _udidForHistory = "";
    [global::ProtoBuf.ProtoMember(46, IsRequired = false, Name=@"udidForHistory", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string udidForHistory
    {
      get { return _udidForHistory; }
      set { _udidForHistory = value; }
    }

    private string _deviceToken = "";
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"deviceToken", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string deviceToken
    {
      get { return _deviceToken; }
      set { _deviceToken = value; }
    }

    private int _numBadges = default(int);
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"numBadges", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numBadges
    {
      get { return _numBadges; }
      set { _numBadges = value; }
    }

    private long _createTime = default(long);
    [global::ProtoBuf.ProtoMember(20, IsRequired = false, Name=@"createTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long createTime
    {
      get { return _createTime; }
      set { _createTime = value; }
    }

    private int _apsalarId = default(int);
    [global::ProtoBuf.ProtoMember(22, IsRequired = false, Name=@"apsalarId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int apsalarId
    {
      get { return _apsalarId; }
      set { _apsalarId = value; }
    }

    private int _numConsecutiveDaysPlayed = default(int);
    [global::ProtoBuf.ProtoMember(24, IsRequired = false, Name=@"numConsecutiveDaysPlayed", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numConsecutiveDaysPlayed
    {
      get { return _numConsecutiveDaysPlayed; }
      set { _numConsecutiveDaysPlayed = value; }
    }

    private long _lastWallPostNotificationTime = default(long);
    [global::ProtoBuf.ProtoMember(26, IsRequired = false, Name=@"lastWallPostNotificationTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long lastWallPostNotificationTime
    {
      get { return _lastWallPostNotificationTime; }
      set { _lastWallPostNotificationTime = value; }
    }

    private string _kabamNaid = "";
    [global::ProtoBuf.ProtoMember(27, IsRequired = false, Name=@"kabamNaid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string kabamNaid
    {
      get { return _kabamNaid; }
      set { _kabamNaid = value; }
    }

    private bool _fbIdSetOnUserCreate = default(bool);
    [global::ProtoBuf.ProtoMember(44, IsRequired = false, Name=@"fbIdSetOnUserCreate", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool fbIdSetOnUserCreate
    {
      get { return _fbIdSetOnUserCreate; }
      set { _fbIdSetOnUserCreate = value; }
    }

    private string _udid = "";
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"udid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string udid
    {
      get { return _udid; }
      set { _udid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StaticUserLevelInfoProto")]
  public partial class StaticUserLevelInfoProto : global::ProtoBuf.IExtensible
  {
    public StaticUserLevelInfoProto() {}
    

    private int _level = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int level
    {
      get { return _level; }
      set { _level = value; }
    }

    private int _requiredExperience = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"requiredExperience", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int requiredExperience
    {
      get { return _requiredExperience; }
      set { _requiredExperience = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserPvpLeagueProto")]
  public partial class UserPvpLeagueProto : global::ProtoBuf.IExtensible
  {
    public UserPvpLeagueProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _leagueId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"leagueId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int leagueId
    {
      get { return _leagueId; }
      set { _leagueId = value; }
    }

    private int _rank = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"rank", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int rank
    {
      get { return _rank; }
      set { _rank = value; }
    }

    private int _elo = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"elo", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int elo
    {
      get { return _elo; }
      set { _elo = value; }
    }

    private int _battlesWon = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"battlesWon", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int battlesWon
    {
      get { return _battlesWon; }
      set { _battlesWon = value; }
    }

    private int _battlesLost = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"battlesLost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int battlesLost
    {
      get { return _battlesLost; }
      set { _battlesLost = value; }
    }

    private long _shieldEndTime = default(long);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"shieldEndTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long shieldEndTime
    {
      get { return _shieldEndTime; }
      set { _shieldEndTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}