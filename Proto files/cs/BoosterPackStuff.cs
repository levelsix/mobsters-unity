//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: BoosterPackStuff.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RareBoosterPurchaseProto")]
  public partial class RareBoosterPurchaseProto : global::ProtoBuf.IExtensible
  {
    public RareBoosterPurchaseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _user = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"user", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto user
    {
      get { return _user; }
      set { _user = value; }
    }

    private com.lvl6.proto.BoosterPackProto _booster = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"booster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.BoosterPackProto booster
    {
      get { return _booster; }
      set { _booster = value; }
    }

    private ulong _timeOfPurchase = default(ulong);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"timeOfPurchase", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(ulong))]
    public ulong timeOfPurchase
    {
      get { return _timeOfPurchase; }
      set { _timeOfPurchase = value; }
    }

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BoosterPackProto")]
  public partial class BoosterPackProto : global::ProtoBuf.IExtensible
  {
    public BoosterPackProto() {}
    

    private int _boosterPackId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"boosterPackId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int boosterPackId
    {
      get { return _boosterPackId; }
      set { _boosterPackId = value; }
    }

    private bool _costsCoins = default(bool);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"costsCoins", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool costsCoins
    {
      get { return _costsCoins; }
      set { _costsCoins = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.BoosterItemProto> _boosterItems = new global::System.Collections.Generic.List<com.lvl6.proto.BoosterItemProto>();
    [global::ProtoBuf.ProtoMember(4, Name=@"boosterItems", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.BoosterItemProto> boosterItems
    {
      get { return _boosterItems; }
    }
  

    private int _price = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"price", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int price
    {
      get { return _price; }
      set { _price = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BoosterItemProto")]
  public partial class BoosterItemProto : global::ProtoBuf.IExtensible
  {
    public BoosterItemProto() {}
    

    private int _boosterItemId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"boosterItemId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int boosterItemId
    {
      get { return _boosterItemId; }
      set { _boosterItemId = value; }
    }

    private int _equipId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"equipId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int equipId
    {
      get { return _equipId; }
      set { _equipId = value; }
    }

    private int _quantity = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"quantity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int quantity
    {
      get { return _quantity; }
      set { _quantity = value; }
    }

    private bool _isSpecial = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"isSpecial", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isSpecial
    {
      get { return _isSpecial; }
      set { _isSpecial = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}