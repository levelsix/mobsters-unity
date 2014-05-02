//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: BoosterPackStuff.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: SharedEnumConfig.proto
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

    private string _boosterPackName = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"boosterPackName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string boosterPackName
    {
      get { return _boosterPackName; }
      set { _boosterPackName = value; }
    }

    private int _gemPrice = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"gemPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gemPrice
    {
      get { return _gemPrice; }
      set { _gemPrice = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.BoosterItemProto> _specialItems = new global::System.Collections.Generic.List<com.lvl6.proto.BoosterItemProto>();
    [global::ProtoBuf.ProtoMember(4, Name=@"specialItems", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.BoosterItemProto> specialItems
    {
      get { return _specialItems; }
    }
  

    private string _listBackgroundImgName = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"listBackgroundImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string listBackgroundImgName
    {
      get { return _listBackgroundImgName; }
      set { _listBackgroundImgName = value; }
    }

    private string _listDescription = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"listDescription", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string listDescription
    {
      get { return _listDescription; }
      set { _listDescription = value; }
    }

    private string _navBarImgName = "";
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"navBarImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string navBarImgName
    {
      get { return _navBarImgName; }
      set { _navBarImgName = value; }
    }

    private string _navTitleImgName = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"navTitleImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string navTitleImgName
    {
      get { return _navTitleImgName; }
      set { _navTitleImgName = value; }
    }

    private string _machineImgName = "";
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"machineImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string machineImgName
    {
      get { return _machineImgName; }
      set { _machineImgName = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.BoosterDisplayItemProto> _displayItems = new global::System.Collections.Generic.List<com.lvl6.proto.BoosterDisplayItemProto>();
    [global::ProtoBuf.ProtoMember(10, Name=@"displayItems", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.BoosterDisplayItemProto> displayItems
    {
      get { return _displayItems; }
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

    private int _boosterPackId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"boosterPackId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int boosterPackId
    {
      get { return _boosterPackId; }
      set { _boosterPackId = value; }
    }

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private int _numPieces = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"numPieces", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numPieces
    {
      get { return _numPieces; }
      set { _numPieces = value; }
    }

    private bool _isComplete = default(bool);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"isComplete", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isComplete
    {
      get { return _isComplete; }
      set { _isComplete = value; }
    }

    private bool _isSpecial = default(bool);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"isSpecial", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isSpecial
    {
      get { return _isSpecial; }
      set { _isSpecial = value; }
    }

    private int _gemReward = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"gemReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gemReward
    {
      get { return _gemReward; }
      set { _gemReward = value; }
    }

    private int _cashReward = default(int);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"cashReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cashReward
    {
      get { return _cashReward; }
      set { _cashReward = value; }
    }

    private float _chanceToAppear = default(float);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"chanceToAppear", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float chanceToAppear
    {
      get { return _chanceToAppear; }
      set { _chanceToAppear = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BoosterDisplayItemProto")]
  public partial class BoosterDisplayItemProto : global::ProtoBuf.IExtensible
  {
    public BoosterDisplayItemProto() {}
    

    private int _boosterPackId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"boosterPackId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int boosterPackId
    {
      get { return _boosterPackId; }
      set { _boosterPackId = value; }
    }

    private bool _isMonster = default(bool);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"isMonster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isMonster
    {
      get { return _isMonster; }
      set { _isMonster = value; }
    }

    private bool _isComplete = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"isComplete", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isComplete
    {
      get { return _isComplete; }
      set { _isComplete = value; }
    }

    private com.lvl6.proto.Quality _quality = com.lvl6.proto.Quality.NO_QUALITY;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"quality", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.Quality.NO_QUALITY)]
    public com.lvl6.proto.Quality quality
    {
      get { return _quality; }
      set { _quality = value; }
    }

    private int _gemReward = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"gemReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gemReward
    {
      get { return _gemReward; }
      set { _gemReward = value; }
    }

    private int _quantity = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"quantity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int quantity
    {
      get { return _quantity; }
      set { _quantity = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}