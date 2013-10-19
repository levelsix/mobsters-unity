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

    private com.lvl6.proto.MinimumUserProto _owner = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"owner", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto owner
    {
      get { return _owner; }
      set { _owner = value; }
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

    private com.lvl6.proto.UserClanStatus _status = com.lvl6.proto.UserClanStatus.MEMBER;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UserClanStatus.MEMBER)]
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

    private com.lvl6.proto.UserClanStatus _clanStatus = com.lvl6.proto.UserClanStatus.MEMBER;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"clanStatus", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UserClanStatus.MEMBER)]
    public com.lvl6.proto.UserClanStatus clanStatus
    {
      get { return _clanStatus; }
      set { _clanStatus = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"UserClanStatus")]
    public enum UserClanStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"MEMBER", Value=1)]
      MEMBER = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"REQUESTING", Value=2)]
      REQUESTING = 2
    }
  
}