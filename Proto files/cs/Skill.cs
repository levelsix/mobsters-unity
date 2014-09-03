//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Skill.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SkillProto")]
  public partial class SkillProto : global::ProtoBuf.IExtensible
  {
    public SkillProto() {}
    

    private int _skillId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"skillId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int skillId
    {
      get { return _skillId; }
      set { _skillId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private int _orbCost = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"orbCost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int orbCost
    {
      get { return _orbCost; }
      set { _orbCost = value; }
    }

    private com.lvl6.proto.SkillType _type = com.lvl6.proto.SkillType.NO_SKILL;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.SkillType.NO_SKILL)]
    public com.lvl6.proto.SkillType type
    {
      get { return _type; }
      set { _type = value; }
    }

    private com.lvl6.proto.SkillActivationType _activationType = com.lvl6.proto.SkillActivationType.USER_ACTIVATED;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"activationType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.SkillActivationType.USER_ACTIVATED)]
    public com.lvl6.proto.SkillActivationType activationType
    {
      get { return _activationType; }
      set { _activationType = value; }
    }

    private int _predecId = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"predecId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int predecId
    {
      get { return _predecId; }
      set { _predecId = value; }
    }

    private int _sucId = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"sucId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int sucId
    {
      get { return _sucId; }
      set { _sucId = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.SkillPropertyProto> _properties = new global::System.Collections.Generic.List<com.lvl6.proto.SkillPropertyProto>();
    [global::ProtoBuf.ProtoMember(8, Name=@"properties", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.SkillPropertyProto> properties
    {
      get { return _properties; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SkillPropertyProto")]
  public partial class SkillPropertyProto : global::ProtoBuf.IExtensible
  {
    public SkillPropertyProto() {}
    

    private int _skillPropertyId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"skillPropertyId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int skillPropertyId
    {
      get { return _skillPropertyId; }
      set { _skillPropertyId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private float _skillValue = default(float);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"skillValue", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float skillValue
    {
      get { return _skillValue; }
      set { _skillValue = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"SkillType")]
    public enum SkillType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NO_SKILL", Value=1)]
      NO_SKILL = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CAKE_DROP", Value=2)]
      CAKE_DROP = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"JELLY", Value=3)]
      JELLY = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QUICK_ATTACK", Value=4)]
      QUICK_ATTACK = 4
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"SkillActivationType")]
    public enum SkillActivationType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER_ACTIVATED", Value=1)]
      USER_ACTIVATED = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AUTO_ACTIVATED", Value=2)]
      AUTO_ACTIVATED = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PASSIVE", Value=3)]
      PASSIVE = 3
    }
  
}