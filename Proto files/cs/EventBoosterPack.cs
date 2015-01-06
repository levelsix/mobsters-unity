//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventBoosterPack.proto
// Note: requires additional types generated from: BoosterPackStuff.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PurchaseBoosterPackRequestProto")]
  public partial class PurchaseBoosterPackRequestProto : global::ProtoBuf.IExtensible
  {
    public PurchaseBoosterPackRequestProto() {}
    
    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private int _boosterPackId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"boosterPackId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int boosterPackId
    {
      get { return _boosterPackId; }
      set { _boosterPackId = value; }
    }
    private long _clientTime = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"clientTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long clientTime
    {
      get { return _clientTime; }
      set { _clientTime = value; }
    }
    private bool _dailyFreeBoosterPack = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"dailyFreeBoosterPack", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool dailyFreeBoosterPack
    {
      get { return _dailyFreeBoosterPack; }
      set { _dailyFreeBoosterPack = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PurchaseBoosterPackResponseProto")]
  public partial class PurchaseBoosterPackResponseProto : global::ProtoBuf.IExtensible
  {
    public PurchaseBoosterPackResponseProto() {}
    
    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private com.lvl6.proto.PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus _status = com.lvl6.proto.PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus.SUCCESS)]
    public com.lvl6.proto.PurchaseBoosterPackResponseProto.PurchaseBoosterPackStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullUserMonsterProto> _updatedOrNew = new global::System.Collections.Generic.List<com.lvl6.proto.FullUserMonsterProto>();
    [global::ProtoBuf.ProtoMember(3, Name=@"updatedOrNew", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullUserMonsterProto> updatedOrNew
    {
      get { return _updatedOrNew; }
    }
  
    private com.lvl6.proto.BoosterItemProto _prize = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"prize", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.BoosterItemProto prize
    {
      get { return _prize; }
      set { _prize = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"PurchaseBoosterPackStatus")]
    public enum PurchaseBoosterPackStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_GEMS", Value=2)]
      FAIL_INSUFFICIENT_GEMS = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=3)]
      FAIL_OTHER = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReceivedRareBoosterPurchaseResponseProto")]
  public partial class ReceivedRareBoosterPurchaseResponseProto : global::ProtoBuf.IExtensible
  {
    public ReceivedRareBoosterPurchaseResponseProto() {}
    
    private com.lvl6.proto.RareBoosterPurchaseProto _rareBoosterPurchase = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"rareBoosterPurchase", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.RareBoosterPurchaseProto rareBoosterPurchase
    {
      get { return _rareBoosterPurchase; }
      set { _rareBoosterPurchase = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}