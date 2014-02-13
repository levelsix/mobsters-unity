//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventPvp.proto
// Note: requires additional types generated from: Battle.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QueueUpRequestProto")]
  public partial class QueueUpRequestProto : global::ProtoBuf.IExtensible
  {
    public QueueUpRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _attacker = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"attacker", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto attacker
    {
      get { return _attacker; }
      set { _attacker = value; }
    }

    private int _attackerElo = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"attackerElo", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int attackerElo
    {
      get { return _attackerElo; }
      set { _attackerElo = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _seenUserIds = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(5, Name=@"seenUserIds", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> seenUserIds
    {
      get { return _seenUserIds; }
    }
  

    private long _clientTime = default(long);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"clientTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long clientTime
    {
      get { return _clientTime; }
      set { _clientTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QueueUpResponseProto")]
  public partial class QueueUpResponseProto : global::ProtoBuf.IExtensible
  {
    public QueueUpResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _attacker = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"attacker", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto attacker
    {
      get { return _attacker; }
      set { _attacker = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.PvpProto> _defenderInfoList = new global::System.Collections.Generic.List<com.lvl6.proto.PvpProto>();
    [global::ProtoBuf.ProtoMember(2, Name=@"defenderInfoList", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.PvpProto> defenderInfoList
    {
      get { return _defenderInfoList; }
    }
  

    private com.lvl6.proto.QueueUpResponseProto.QueueUpStatus _status = com.lvl6.proto.QueueUpResponseProto.QueueUpStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.QueueUpResponseProto.QueueUpStatus.SUCCESS)]
    public com.lvl6.proto.QueueUpResponseProto.QueueUpStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"QueueUpStatus")]
    public enum QueueUpStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_NOT_ENOUGH_CASH", Value=2)]
      FAIL_NOT_ENOUGH_CASH = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=3)]
      FAIL_OTHER = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_NOT_ENOUGH_GEMS", Value=4)]
      FAIL_NOT_ENOUGH_GEMS = 4
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BeginPvpBattleRequestProto")]
  public partial class BeginPvpBattleRequestProto : global::ProtoBuf.IExtensible
  {
    public BeginPvpBattleRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _senderElo = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"senderElo", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int senderElo
    {
      get { return _senderElo; }
      set { _senderElo = value; }
    }

    private long _attackStartTime = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"attackStartTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long attackStartTime
    {
      get { return _attackStartTime; }
      set { _attackStartTime = value; }
    }

    private com.lvl6.proto.PvpProto _enemy = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"enemy", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.PvpProto enemy
    {
      get { return _enemy; }
      set { _enemy = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BeginPvpBattleResponseProto")]
  public partial class BeginPvpBattleResponseProto : global::ProtoBuf.IExtensible
  {
    public BeginPvpBattleResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.BeginPvpBattleResponseProto.BeginPvpBattleStatus _status = com.lvl6.proto.BeginPvpBattleResponseProto.BeginPvpBattleStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.BeginPvpBattleResponseProto.BeginPvpBattleStatus.SUCCESS)]
    public com.lvl6.proto.BeginPvpBattleResponseProto.BeginPvpBattleStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"BeginPvpBattleStatus")]
    public enum BeginPvpBattleStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_ENEMY_UNAVAILABLE", Value=2)]
      FAIL_ENEMY_UNAVAILABLE = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=3)]
      FAIL_OTHER = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EndPvpBattleRequestProto")]
  public partial class EndPvpBattleRequestProto : global::ProtoBuf.IExtensible
  {
    public EndPvpBattleRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProtoWithMaxResources _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithMaxResources sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _defenderId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"defenderId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int defenderId
    {
      get { return _defenderId; }
      set { _defenderId = value; }
    }

    private bool _userAttacked = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"userAttacked", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool userAttacked
    {
      get { return _userAttacked; }
      set { _userAttacked = value; }
    }

    private bool _userWon = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"userWon", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool userWon
    {
      get { return _userWon; }
      set { _userWon = value; }
    }

    private long _clientTime = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"clientTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long clientTime
    {
      get { return _clientTime; }
      set { _clientTime = value; }
    }

    private int _oilChange = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"oilChange", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int oilChange
    {
      get { return _oilChange; }
      set { _oilChange = value; }
    }

    private int _cashChange = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"cashChange", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cashChange
    {
      get { return _cashChange; }
      set { _cashChange = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EndPvpBattleResponseProto")]
  public partial class EndPvpBattleResponseProto : global::ProtoBuf.IExtensible
  {
    public EndPvpBattleResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProtoWithMaxResources _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithMaxResources sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _defenderId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"defenderId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int defenderId
    {
      get { return _defenderId; }
      set { _defenderId = value; }
    }

    private bool _attackerAttacked = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"attackerAttacked", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool attackerAttacked
    {
      get { return _attackerAttacked; }
      set { _attackerAttacked = value; }
    }

    private bool _attackerWon = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"attackerWon", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool attackerWon
    {
      get { return _attackerWon; }
      set { _attackerWon = value; }
    }

    private com.lvl6.proto.EndPvpBattleResponseProto.EndPvpBattleStatus _status = com.lvl6.proto.EndPvpBattleResponseProto.EndPvpBattleStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.EndPvpBattleResponseProto.EndPvpBattleStatus.SUCCESS)]
    public com.lvl6.proto.EndPvpBattleResponseProto.EndPvpBattleStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"EndPvpBattleStatus")]
    public enum EndPvpBattleStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=2)]
      FAIL_OTHER = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_BATTLE_TOOK_TOO_LONG", Value=3)]
      FAIL_BATTLE_TOOK_TOO_LONG = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}