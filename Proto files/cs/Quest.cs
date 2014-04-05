//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Quest.proto
// Note: requires additional types generated from: Chat.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: Structure.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullQuestProto")]
  public partial class FullQuestProto : global::ProtoBuf.IExtensible
  {
    public FullQuestProto() {}
    

    private int _questId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"questId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int questId
    {
      get { return _questId; }
      set { _questId = value; }
    }

    private int _cityId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"cityId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cityId
    {
      get { return _cityId; }
      set { _cityId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _description = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string description
    {
      get { return _description; }
      set { _description = value; }
    }

    private string _doneResponse = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"doneResponse", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string doneResponse
    {
      get { return _doneResponse; }
      set { _doneResponse = value; }
    }

    private com.lvl6.proto.DialogueProto _acceptDialogue = null;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"acceptDialogue", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.DialogueProto acceptDialogue
    {
      get { return _acceptDialogue; }
      set { _acceptDialogue = value; }
    }

    private com.lvl6.proto.FullQuestProto.QuestType _questType = com.lvl6.proto.FullQuestProto.QuestType.KILL_MONSTER;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"questType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.FullQuestProto.QuestType.KILL_MONSTER)]
    public com.lvl6.proto.FullQuestProto.QuestType questType
    {
      get { return _questType; }
      set { _questType = value; }
    }

    private string _jobDescription = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"jobDescription", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string jobDescription
    {
      get { return _jobDescription; }
      set { _jobDescription = value; }
    }

    private int _staticDataId = default(int);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"staticDataId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int staticDataId
    {
      get { return _staticDataId; }
      set { _staticDataId = value; }
    }

    private int _quantity = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"quantity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int quantity
    {
      get { return _quantity; }
      set { _quantity = value; }
    }

    private int _cashReward = default(int);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"cashReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cashReward
    {
      get { return _cashReward; }
      set { _cashReward = value; }
    }

    private int _oilReward = default(int);
    [global::ProtoBuf.ProtoMember(22, IsRequired = false, Name=@"oilReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int oilReward
    {
      get { return _oilReward; }
      set { _oilReward = value; }
    }

    private int _gemReward = default(int);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"gemReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gemReward
    {
      get { return _gemReward; }
      set { _gemReward = value; }
    }

    private int _expReward = default(int);
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"expReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int expReward
    {
      get { return _expReward; }
      set { _expReward = value; }
    }

    private int _monsterIdReward = default(int);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"monsterIdReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterIdReward
    {
      get { return _monsterIdReward; }
      set { _monsterIdReward = value; }
    }

    private bool _isCompleteMonster = default(bool);
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"isCompleteMonster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isCompleteMonster
    {
      get { return _isCompleteMonster; }
      set { _isCompleteMonster = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _questsRequiredForThis = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(16, Name=@"questsRequiredForThis", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> questsRequiredForThis
    {
      get { return _questsRequiredForThis; }
    }
  

    private string _questGiverName = "";
    [global::ProtoBuf.ProtoMember(24, IsRequired = false, Name=@"questGiverName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string questGiverName
    {
      get { return _questGiverName; }
      set { _questGiverName = value; }
    }

    private string _questGiverImagePrefix = "";
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"questGiverImagePrefix", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string questGiverImagePrefix
    {
      get { return _questGiverImagePrefix; }
      set { _questGiverImagePrefix = value; }
    }

    private int _priority = default(int);
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"priority", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int priority
    {
      get { return _priority; }
      set { _priority = value; }
    }

    private string _carrotId = "";
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"carrotId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string carrotId
    {
      get { return _carrotId; }
      set { _carrotId = value; }
    }

    private bool _isAchievement = default(bool);
    [global::ProtoBuf.ProtoMember(20, IsRequired = false, Name=@"isAchievement", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isAchievement
    {
      get { return _isAchievement; }
      set { _isAchievement = value; }
    }

    private com.lvl6.proto.CoordinateProto _questGiverImgOffset = null;
    [global::ProtoBuf.ProtoMember(21, IsRequired = false, Name=@"questGiverImgOffset", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.CoordinateProto questGiverImgOffset
    {
      get { return _questGiverImgOffset; }
      set { _questGiverImgOffset = value; }
    }

    private com.lvl6.proto.MonsterProto.MonsterElement _monsterElement = com.lvl6.proto.MonsterProto.MonsterElement.FIRE;
    [global::ProtoBuf.ProtoMember(23, IsRequired = false, Name=@"monsterElement", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.MonsterProto.MonsterElement.FIRE)]
    public com.lvl6.proto.MonsterProto.MonsterElement monsterElement
    {
      get { return _monsterElement; }
      set { _monsterElement = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"QuestType")]
    public enum QuestType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"KILL_MONSTER", Value=1)]
      KILL_MONSTER = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DONATE_MONSTER", Value=2)]
      DONATE_MONSTER = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"COMPLETE_TASK", Value=3)]
      COMPLETE_TASK = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"COLLECT_COINS_FROM_HOME", Value=4)]
      COLLECT_COINS_FROM_HOME = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BUILD_STRUCT", Value=5)]
      BUILD_STRUCT = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"UPGRADE_STRUCT", Value=6)]
      UPGRADE_STRUCT = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MONSTER_APPEAR", Value=7)]
      MONSTER_APPEAR = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"COLLECT_SPECIAL_ITEM", Value=8)]
      COLLECT_SPECIAL_ITEM = 8
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DialogueProto")]
  public partial class DialogueProto : global::ProtoBuf.IExtensible
  {
    public DialogueProto() {}
    
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.DialogueProto.SpeechSegmentProto> _speechSegment = new global::System.Collections.Generic.List<com.lvl6.proto.DialogueProto.SpeechSegmentProto>();
    [global::ProtoBuf.ProtoMember(1, Name=@"speechSegment", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.DialogueProto.SpeechSegmentProto> speechSegment
    {
      get { return _speechSegment; }
    }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SpeechSegmentProto")]
  public partial class SpeechSegmentProto : global::ProtoBuf.IExtensible
  {
    public SpeechSegmentProto() {}
    

    private string _speaker = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"speaker", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string speaker
    {
      get { return _speaker; }
      set { _speaker = value; }
    }

    private string _speakerImage = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"speakerImage", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string speakerImage
    {
      get { return _speakerImage; }
      set { _speakerImage = value; }
    }

    private string _speakerText = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"speakerText", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string speakerText
    {
      get { return _speakerText; }
      set { _speakerText = value; }
    }

    private bool _isLeftSide = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"isLeftSide", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isLeftSide
    {
      get { return _isLeftSide; }
      set { _isLeftSide = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullUserQuestProto")]
  public partial class FullUserQuestProto : global::ProtoBuf.IExtensible
  {
    public FullUserQuestProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _questId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"questId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int questId
    {
      get { return _questId; }
      set { _questId = value; }
    }

    private bool _isRedeemed = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"isRedeemed", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isRedeemed
    {
      get { return _isRedeemed; }
      set { _isRedeemed = value; }
    }

    private bool _isComplete = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"isComplete", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isComplete
    {
      get { return _isComplete; }
      set { _isComplete = value; }
    }

    private int _progress = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"progress", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int progress
    {
      get { return _progress; }
      set { _progress = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ItemProto")]
  public partial class ItemProto : global::ProtoBuf.IExtensible
  {
    public ItemProto() {}
    

    private int _itemId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _imgName = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"imgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string imgName
    {
      get { return _imgName; }
      set { _imgName = value; }
    }

    private string _borderImgName = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"borderImgName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string borderImgName
    {
      get { return _borderImgName; }
      set { _borderImgName = value; }
    }

    private com.lvl6.proto.ColorProto _color = null;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"color", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.ColorProto color
    {
      get { return _color; }
      set { _color = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}