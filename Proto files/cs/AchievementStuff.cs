//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: AchievementStuff.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: SharedEnumConfig.proto
// Note: requires additional types generated from: Structure.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AchievementProto")]
  public partial class AchievementProto : global::ProtoBuf.IExtensible
  {
    public AchievementProto() {}
    

    private int _achievementId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"achievementId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int achievementId
    {
      get { return _achievementId; }
      set { _achievementId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _description = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string description
    {
      get { return _description; }
      set { _description = value; }
    }

    private int _gemReward = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"gemReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gemReward
    {
      get { return _gemReward; }
      set { _gemReward = value; }
    }

    private int _lvl = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"lvl", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int lvl
    {
      get { return _lvl; }
      set { _lvl = value; }
    }

    private com.lvl6.proto.AchievementProto.AchievementType _achievementType = com.lvl6.proto.AchievementProto.AchievementType.NO_ACHIEVEMENT;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"achievementType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.AchievementProto.AchievementType.NO_ACHIEVEMENT)]
    public com.lvl6.proto.AchievementProto.AchievementType achievementType
    {
      get { return _achievementType; }
      set { _achievementType = value; }
    }

    private com.lvl6.proto.ResourceType _resourceType = com.lvl6.proto.ResourceType.NO_RESOURCE;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"resourceType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.ResourceType.NO_RESOURCE)]
    public com.lvl6.proto.ResourceType resourceType
    {
      get { return _resourceType; }
      set { _resourceType = value; }
    }

    private com.lvl6.proto.Element _element = com.lvl6.proto.Element.NO_ELEMENT;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"element", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.Element.NO_ELEMENT)]
    public com.lvl6.proto.Element element
    {
      get { return _element; }
      set { _element = value; }
    }

    private com.lvl6.proto.Quality _quality = com.lvl6.proto.Quality.NO_QUALITY;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"quality", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.Quality.NO_QUALITY)]
    public com.lvl6.proto.Quality quality
    {
      get { return _quality; }
      set { _quality = value; }
    }

    private int _staticDataId = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"staticDataId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int staticDataId
    {
      get { return _staticDataId; }
      set { _staticDataId = value; }
    }

    private int _quantity = default(int);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"quantity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int quantity
    {
      get { return _quantity; }
      set { _quantity = value; }
    }

    private int _priority = default(int);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"priority", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int priority
    {
      get { return _priority; }
      set { _priority = value; }
    }

    private int _prerequisiteId = default(int);
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"prerequisiteId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int prerequisiteId
    {
      get { return _prerequisiteId; }
      set { _prerequisiteId = value; }
    }

    private int _successorId = default(int);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"successorId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int successorId
    {
      get { return _successorId; }
      set { _successorId = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"AchievementType")]
    public enum AchievementType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NO_ACHIEVEMENT", Value=17)]
      NO_ACHIEVEMENT = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"COLLECT_RESOURCE", Value=1)]
      COLLECT_RESOURCE = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CREATE_GRENADE", Value=2)]
      CREATE_GRENADE = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CREATE_RAINBOW", Value=3)]
      CREATE_RAINBOW = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CREATE_ROCKET", Value=4)]
      CREATE_ROCKET = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DEFEAT_MONSTERS", Value=5)]
      DEFEAT_MONSTERS = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DESTROY_ORBS", Value=6)]
      DESTROY_ORBS = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ENHANCE_POINTS", Value=7)]
      ENHANCE_POINTS = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"HEAL_MONSTERS", Value=8)]
      HEAL_MONSTERS = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"JOIN_LEAGUE", Value=9)]
      JOIN_LEAGUE = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MAKE_COMBO", Value=10)]
      MAKE_COMBO = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"REMOVE_OBSTACLE", Value=11)]
      REMOVE_OBSTACLE = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SELL_MONSTER", Value=12)]
      SELL_MONSTER = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"STEAL_RESOURCE", Value=13)]
      STEAL_RESOURCE = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"TAKE_DAMAGE", Value=14)]
      TAKE_DAMAGE = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"UPGRADE_BUILDING", Value=15)]
      UPGRADE_BUILDING = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"WIN_PVP_BATTLE", Value=16)]
      WIN_PVP_BATTLE = 16
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserAchievementProto")]
  public partial class UserAchievementProto : global::ProtoBuf.IExtensible
  {
    public UserAchievementProto() {}
    

    private int _achievementId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"achievementId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int achievementId
    {
      get { return _achievementId; }
      set { _achievementId = value; }
    }

    private int _progress = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"progress", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int progress
    {
      get { return _progress; }
      set { _progress = value; }
    }

    private bool _isComplete = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"isComplete", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isComplete
    {
      get { return _isComplete; }
      set { _isComplete = value; }
    }

    private bool _isRedeemed = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"isRedeemed", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isRedeemed
    {
      get { return _isRedeemed; }
      set { _isRedeemed = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}