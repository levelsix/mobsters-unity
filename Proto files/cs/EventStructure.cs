//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventStructure.proto
// Note: requires additional types generated from: City.proto
// Note: requires additional types generated from: Structure.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PurchaseNormStructureRequestProto")]
  public partial class PurchaseNormStructureRequestProto : global::ProtoBuf.IExtensible
  {
    public PurchaseNormStructureRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.CoordinateProto _structCoordinates = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"structCoordinates", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.CoordinateProto structCoordinates
    {
      get { return _structCoordinates; }
      set { _structCoordinates = value; }
    }

    private int _structId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"structId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int structId
    {
      get { return _structId; }
      set { _structId = value; }
    }

    private long _timeOfPurchase = default(long);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"timeOfPurchase", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfPurchase
    {
      get { return _timeOfPurchase; }
      set { _timeOfPurchase = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PurchaseNormStructureResponseProto")]
  public partial class PurchaseNormStructureResponseProto : global::ProtoBuf.IExtensible
  {
    public PurchaseNormStructureResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.PurchaseNormStructureResponseProto.PurchaseNormStructureStatus _status = com.lvl6.proto.PurchaseNormStructureResponseProto.PurchaseNormStructureStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.PurchaseNormStructureResponseProto.PurchaseNormStructureStatus.SUCCESS)]
    public com.lvl6.proto.PurchaseNormStructureResponseProto.PurchaseNormStructureStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"PurchaseNormStructureStatus")]
    public enum PurchaseNormStructureStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_CASH", Value=2)]
      FAIL_INSUFFICIENT_CASH = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_GEMS", Value=3)]
      FAIL_INSUFFICIENT_GEMS = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_LEVEL_TOO_LOW", Value=4)]
      FAIL_LEVEL_TOO_LOW = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_ANOTHER_STRUCT_STILL_BUILDING", Value=5)]
      FAIL_ANOTHER_STRUCT_STILL_BUILDING = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_ALREADY_HAVE_MAX_OF_THIS_STRUCT", Value=6)]
      FAIL_ALREADY_HAVE_MAX_OF_THIS_STRUCT = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=7)]
      FAIL_OTHER = 7
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MoveOrRotateNormStructureRequestProto")]
  public partial class MoveOrRotateNormStructureRequestProto : global::ProtoBuf.IExtensible
  {
    public MoveOrRotateNormStructureRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }

    private com.lvl6.proto.MoveOrRotateNormStructureRequestProto.MoveOrRotateNormStructType _type = com.lvl6.proto.MoveOrRotateNormStructureRequestProto.MoveOrRotateNormStructType.MOVE;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.MoveOrRotateNormStructureRequestProto.MoveOrRotateNormStructType.MOVE)]
    public com.lvl6.proto.MoveOrRotateNormStructureRequestProto.MoveOrRotateNormStructType type
    {
      get { return _type; }
      set { _type = value; }
    }

    private com.lvl6.proto.CoordinateProto _curStructCoordinates = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"curStructCoordinates", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.CoordinateProto curStructCoordinates
    {
      get { return _curStructCoordinates; }
      set { _curStructCoordinates = value; }
    }

    private com.lvl6.proto.StructOrientation _newOrientation = com.lvl6.proto.StructOrientation.POSITION_1;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"newOrientation", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.StructOrientation.POSITION_1)]
    public com.lvl6.proto.StructOrientation newOrientation
    {
      get { return _newOrientation; }
      set { _newOrientation = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"MoveOrRotateNormStructType")]
    public enum MoveOrRotateNormStructType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOVE", Value=1)]
      MOVE = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROTATE", Value=2)]
      ROTATE = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MoveOrRotateNormStructureResponseProto")]
  public partial class MoveOrRotateNormStructureResponseProto : global::ProtoBuf.IExtensible
  {
    public MoveOrRotateNormStructureResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.MoveOrRotateNormStructureResponseProto.MoveOrRotateNormStructureStatus _status = com.lvl6.proto.MoveOrRotateNormStructureResponseProto.MoveOrRotateNormStructureStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.MoveOrRotateNormStructureResponseProto.MoveOrRotateNormStructureStatus.SUCCESS)]
    public com.lvl6.proto.MoveOrRotateNormStructureResponseProto.MoveOrRotateNormStructureStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"MoveOrRotateNormStructureStatus")]
    public enum MoveOrRotateNormStructureStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"OTHER_FAIL", Value=2)]
      OTHER_FAIL = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SellNormStructureRequestProto")]
  public partial class SellNormStructureRequestProto : global::ProtoBuf.IExtensible
  {
    public SellNormStructureRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SellNormStructureResponseProto")]
  public partial class SellNormStructureResponseProto : global::ProtoBuf.IExtensible
  {
    public SellNormStructureResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.SellNormStructureResponseProto.SellNormStructureStatus _status = com.lvl6.proto.SellNormStructureResponseProto.SellNormStructureStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.SellNormStructureResponseProto.SellNormStructureStatus.SUCCESS)]
    public com.lvl6.proto.SellNormStructureResponseProto.SellNormStructureStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"SellNormStructureStatus")]
    public enum SellNormStructureStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL", Value=2)]
      FAIL = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpgradeNormStructureRequestProto")]
  public partial class UpgradeNormStructureRequestProto : global::ProtoBuf.IExtensible
  {
    public UpgradeNormStructureRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }

    private long _timeOfUpgrade = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"timeOfUpgrade", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfUpgrade
    {
      get { return _timeOfUpgrade; }
      set { _timeOfUpgrade = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpgradeNormStructureResponseProto")]
  public partial class UpgradeNormStructureResponseProto : global::ProtoBuf.IExtensible
  {
    public UpgradeNormStructureResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.UpgradeNormStructureResponseProto.UpgradeNormStructureStatus _status = com.lvl6.proto.UpgradeNormStructureResponseProto.UpgradeNormStructureStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UpgradeNormStructureResponseProto.UpgradeNormStructureStatus.SUCCESS)]
    public com.lvl6.proto.UpgradeNormStructureResponseProto.UpgradeNormStructureStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"UpgradeNormStructureStatus")]
    public enum UpgradeNormStructureStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOT_ENOUGH_MATERIALS", Value=2)]
      NOT_ENOUGH_MATERIALS = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOT_BUILT_YET", Value=3)]
      NOT_BUILT_YET = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOT_USERS_STRUCT", Value=4)]
      NOT_USERS_STRUCT = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ANOTHER_STRUCT_STILL_UPGRADING", Value=5)]
      ANOTHER_STRUCT_STILL_UPGRADING = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"OTHER_FAIL", Value=6)]
      OTHER_FAIL = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CLIENT_TOO_APART_FROM_SERVER_TIME", Value=7)]
      CLIENT_TOO_APART_FROM_SERVER_TIME = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AT_MAX_LEVEL_ALREADY", Value=8)]
      AT_MAX_LEVEL_ALREADY = 8
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FinishNormStructWaittimeWithDiamondsRequestProto")]
  public partial class FinishNormStructWaittimeWithDiamondsRequestProto : global::ProtoBuf.IExtensible
  {
    public FinishNormStructWaittimeWithDiamondsRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }

    private long _timeOfSpeedup = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"timeOfSpeedup", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfSpeedup
    {
      get { return _timeOfSpeedup; }
      set { _timeOfSpeedup = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FinishNormStructWaittimeWithDiamondsResponseProto")]
  public partial class FinishNormStructWaittimeWithDiamondsResponseProto : global::ProtoBuf.IExtensible
  {
    public FinishNormStructWaittimeWithDiamondsResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.FinishNormStructWaittimeWithDiamondsResponseProto.FinishNormStructWaittimeStatus _status = com.lvl6.proto.FinishNormStructWaittimeWithDiamondsResponseProto.FinishNormStructWaittimeStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.FinishNormStructWaittimeWithDiamondsResponseProto.FinishNormStructWaittimeStatus.SUCCESS)]
    public com.lvl6.proto.FinishNormStructWaittimeWithDiamondsResponseProto.FinishNormStructWaittimeStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"FinishNormStructWaittimeStatus")]
    public enum FinishNormStructWaittimeStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_NOT_ENOUGH_GEMS", Value=2)]
      FAIL_NOT_ENOUGH_GEMS = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=3)]
      FAIL_OTHER = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"NormStructWaitCompleteRequestProto")]
  public partial class NormStructWaitCompleteRequestProto : global::ProtoBuf.IExtensible
  {
    public NormStructWaitCompleteRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _userStructId = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(2, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> userStructId
    {
      get { return _userStructId; }
    }
  

    private long _curTime = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"curTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long curTime
    {
      get { return _curTime; }
      set { _curTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"NormStructWaitCompleteResponseProto")]
  public partial class NormStructWaitCompleteResponseProto : global::ProtoBuf.IExtensible
  {
    public NormStructWaitCompleteResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.NormStructWaitCompleteResponseProto.NormStructWaitCompleteStatus _status = com.lvl6.proto.NormStructWaitCompleteResponseProto.NormStructWaitCompleteStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.NormStructWaitCompleteResponseProto.NormStructWaitCompleteStatus.SUCCESS)]
    public com.lvl6.proto.NormStructWaitCompleteResponseProto.NormStructWaitCompleteStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullUserStructureProto> _userStruct = new global::System.Collections.Generic.List<com.lvl6.proto.FullUserStructureProto>();
    [global::ProtoBuf.ProtoMember(3, Name=@"userStruct", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullUserStructureProto> userStruct
    {
      get { return _userStruct; }
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"NormStructWaitCompleteStatus")]
    public enum NormStructWaitCompleteStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_NOT_DONE_YET", Value=2)]
      FAIL_NOT_DONE_YET = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=3)]
      FAIL_OTHER = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RetrieveCurrencyFromNormStructureRequestProto")]
  public partial class RetrieveCurrencyFromNormStructureRequestProto : global::ProtoBuf.IExtensible
  {
    public RetrieveCurrencyFromNormStructureRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval> _structRetrievals = new global::System.Collections.Generic.List<com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval>();
    [global::ProtoBuf.ProtoMember(2, Name=@"structRetrievals", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.RetrieveCurrencyFromNormStructureRequestProto.StructRetrieval> structRetrievals
    {
      get { return _structRetrievals; }
    }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StructRetrieval")]
  public partial class StructRetrieval : global::ProtoBuf.IExtensible
  {
    public StructRetrieval() {}
    

    private int _userStructId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userStructId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userStructId
    {
      get { return _userStructId; }
      set { _userStructId = value; }
    }

    private long _timeOfRetrieval = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"timeOfRetrieval", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfRetrieval
    {
      get { return _timeOfRetrieval; }
      set { _timeOfRetrieval = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RetrieveCurrencyFromNormStructureResponseProto")]
  public partial class RetrieveCurrencyFromNormStructureResponseProto : global::ProtoBuf.IExtensible
  {
    public RetrieveCurrencyFromNormStructureResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus _status = com.lvl6.proto.RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus.OTHER_FAIL;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus.OTHER_FAIL)]
    public com.lvl6.proto.RetrieveCurrencyFromNormStructureResponseProto.RetrieveCurrencyFromNormStructureStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"RetrieveCurrencyFromNormStructureStatus")]
    public enum RetrieveCurrencyFromNormStructureStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"OTHER_FAIL", Value=1)]
      OTHER_FAIL = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=2)]
      SUCCESS = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CLIENT_TOO_APART_FROM_SERVER_TIME", Value=3)]
      CLIENT_TOO_APART_FROM_SERVER_TIME = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOT_LONG_ENOUGH", Value=4)]
      NOT_LONG_ENOUGH = 4
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ExpansionWaitCompleteRequestProto")]
  public partial class ExpansionWaitCompleteRequestProto : global::ProtoBuf.IExtensible
  {
    public ExpansionWaitCompleteRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private long _curTime = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"curTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long curTime
    {
      get { return _curTime; }
      set { _curTime = value; }
    }

    private bool _speedUp = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"speedUp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool speedUp
    {
      get { return _speedUp; }
      set { _speedUp = value; }
    }

    private int _xPosition = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"xPosition", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int xPosition
    {
      get { return _xPosition; }
      set { _xPosition = value; }
    }

    private int _yPosition = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"yPosition", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int yPosition
    {
      get { return _yPosition; }
      set { _yPosition = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ExpansionWaitCompleteResponseProto")]
  public partial class ExpansionWaitCompleteResponseProto : global::ProtoBuf.IExtensible
  {
    public ExpansionWaitCompleteResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.ExpansionWaitCompleteResponseProto.ExpansionWaitCompleteStatus _status = com.lvl6.proto.ExpansionWaitCompleteResponseProto.ExpansionWaitCompleteStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.ExpansionWaitCompleteResponseProto.ExpansionWaitCompleteStatus.SUCCESS)]
    public com.lvl6.proto.ExpansionWaitCompleteResponseProto.ExpansionWaitCompleteStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private com.lvl6.proto.UserCityExpansionDataProto _ucedp = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"ucedp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.UserCityExpansionDataProto ucedp
    {
      get { return _ucedp; }
      set { _ucedp = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"ExpansionWaitCompleteStatus")]
    public enum ExpansionWaitCompleteStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_WAS_NOT_EXPANDING", Value=2)]
      FAIL_WAS_NOT_EXPANDING = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_NOT_DONE_YET", Value=3)]
      FAIL_NOT_DONE_YET = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_GEMS", Value=4)]
      FAIL_INSUFFICIENT_GEMS = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=5)]
      FAIL_OTHER = 5
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}