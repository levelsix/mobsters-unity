//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventInAppPurchase.proto
// Note: requires additional types generated from: InAppPurchase.proto
// Note: requires additional types generated from: Structure.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InAppPurchaseRequestProto")]
  public partial class InAppPurchaseRequestProto : global::ProtoBuf.IExtensible
  {
    public InAppPurchaseRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private string _receipt = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"receipt", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string receipt
    {
      get { return _receipt; }
      set { _receipt = value; }
    }

    private string _localcents = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"localcents", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string localcents
    {
      get { return _localcents; }
      set { _localcents = value; }
    }

    private string _localcurrency = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"localcurrency", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string localcurrency
    {
      get { return _localcurrency; }
      set { _localcurrency = value; }
    }

    private string _locale = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"locale", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string locale
    {
      get { return _locale; }
      set { _locale = value; }
    }

    private string _ipaddr = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"ipaddr", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ipaddr
    {
      get { return _ipaddr; }
      set { _ipaddr = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InAppPurchaseResponseProto")]
  public partial class InAppPurchaseResponseProto : global::ProtoBuf.IExtensible
  {
    public InAppPurchaseResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.InAppPurchaseResponseProto.InAppPurchaseStatus _status = com.lvl6.proto.InAppPurchaseResponseProto.InAppPurchaseStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.InAppPurchaseResponseProto.InAppPurchaseStatus.SUCCESS)]
    public com.lvl6.proto.InAppPurchaseResponseProto.InAppPurchaseStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private int _diamondsGained = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"diamondsGained", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int diamondsGained
    {
      get { return _diamondsGained; }
      set { _diamondsGained = value; }
    }

    private int _coinsGained = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"coinsGained", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int coinsGained
    {
      get { return _coinsGained; }
      set { _coinsGained = value; }
    }

    private string _packageName = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"packageName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string packageName
    {
      get { return _packageName; }
      set { _packageName = value; }
    }

    private double _packagePrice = default(double);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"packagePrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(double))]
    public double packagePrice
    {
      get { return _packagePrice; }
      set { _packagePrice = value; }
    }

    private string _receipt = "";
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"receipt", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string receipt
    {
      get { return _receipt; }
      set { _receipt = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"InAppPurchaseStatus")]
    public enum InAppPurchaseStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL", Value=2)]
      FAIL = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DUPLICATE_RECEIPT", Value=3)]
      DUPLICATE_RECEIPT = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EarnFreeDiamondsRequestProto")]
  public partial class EarnFreeDiamondsRequestProto : global::ProtoBuf.IExtensible
  {
    public EarnFreeDiamondsRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.EarnFreeDiamondsType _freeDiamondsType = com.lvl6.proto.EarnFreeDiamondsType.FB_CONNECT;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"freeDiamondsType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.EarnFreeDiamondsType.FB_CONNECT)]
    public com.lvl6.proto.EarnFreeDiamondsType freeDiamondsType
    {
      get { return _freeDiamondsType; }
      set { _freeDiamondsType = value; }
    }

    private long _clientTime = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"clientTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EarnFreeDiamondsResponseProto")]
  public partial class EarnFreeDiamondsResponseProto : global::ProtoBuf.IExtensible
  {
    public EarnFreeDiamondsResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.EarnFreeDiamondsResponseProto.EarnFreeDiamondsStatus _status = com.lvl6.proto.EarnFreeDiamondsResponseProto.EarnFreeDiamondsStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.EarnFreeDiamondsResponseProto.EarnFreeDiamondsStatus.SUCCESS)]
    public com.lvl6.proto.EarnFreeDiamondsResponseProto.EarnFreeDiamondsStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private com.lvl6.proto.EarnFreeDiamondsType _freeDiamondsType = com.lvl6.proto.EarnFreeDiamondsType.FB_CONNECT;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"freeDiamondsType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.EarnFreeDiamondsType.FB_CONNECT)]
    public com.lvl6.proto.EarnFreeDiamondsType freeDiamondsType
    {
      get { return _freeDiamondsType; }
      set { _freeDiamondsType = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"EarnFreeDiamondsStatus")]
    public enum EarnFreeDiamondsStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CLIENT_TOO_APART_FROM_SERVER_TIME", Value=2)]
      CLIENT_TOO_APART_FROM_SERVER_TIME = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"METHOD_NOT_SUPPORTED", Value=3)]
      METHOD_NOT_SUPPORTED = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"OTHER_FAIL", Value=4)]
      OTHER_FAIL = 4
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ExchangeGemsForResourcesRequestProto")]
  public partial class ExchangeGemsForResourcesRequestProto : global::ProtoBuf.IExtensible
  {
    public ExchangeGemsForResourcesRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _numGems = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"numGems", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numGems
    {
      get { return _numGems; }
      set { _numGems = value; }
    }

    private int _numResources = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"numResources", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numResources
    {
      get { return _numResources; }
      set { _numResources = value; }
    }

    private com.lvl6.proto.ResourceType _resourceType = com.lvl6.proto.ResourceType.CASH;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"resourceType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.ResourceType.CASH)]
    public com.lvl6.proto.ResourceType resourceType
    {
      get { return _resourceType; }
      set { _resourceType = value; }
    }

    private long _clientTime = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"clientTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ExchangeGemsForResourcesResponseProto")]
  public partial class ExchangeGemsForResourcesResponseProto : global::ProtoBuf.IExtensible
  {
    public ExchangeGemsForResourcesResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.ExchangeGemsForResourcesResponseProto.ExchangeGemsForResourcesStatus _status = com.lvl6.proto.ExchangeGemsForResourcesResponseProto.ExchangeGemsForResourcesStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.ExchangeGemsForResourcesResponseProto.ExchangeGemsForResourcesStatus.SUCCESS)]
    public com.lvl6.proto.ExchangeGemsForResourcesResponseProto.ExchangeGemsForResourcesStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"ExchangeGemsForResourcesStatus")]
    public enum ExchangeGemsForResourcesStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=2)]
      FAIL_OTHER = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}