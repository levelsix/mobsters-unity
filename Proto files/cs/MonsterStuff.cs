//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: MonsterStuff.proto
// Note: requires additional types generated from: SharedEnumConfig.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MonsterProto")]
  public partial class MonsterProto : global::ProtoBuf.IExtensible
  {
    public MonsterProto() {}
    

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private string _evolutionGroup = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"evolutionGroup", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string evolutionGroup
    {
      get { return _evolutionGroup; }
      set { _evolutionGroup = value; }
    }

    private string _shorterName = "";
    [global::ProtoBuf.ProtoMember(28, IsRequired = false, Name=@"shorterName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string shorterName
    {
      get { return _shorterName; }
      set { _shorterName = value; }
    }

    private string _monsterGroup = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"monsterGroup", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string monsterGroup
    {
      get { return _monsterGroup; }
      set { _monsterGroup = value; }
    }

    private com.lvl6.proto.Quality _quality = com.lvl6.proto.Quality.NO_QUALITY;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"quality", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.Quality.NO_QUALITY)]
    public com.lvl6.proto.Quality quality
    {
      get { return _quality; }
      set { _quality = value; }
    }

    private int _evolutionLevel = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"evolutionLevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int evolutionLevel
    {
      get { return _evolutionLevel; }
      set { _evolutionLevel = value; }
    }

    private string _displayName = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"displayName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string displayName
    {
      get { return _displayName; }
      set { _displayName = value; }
    }

    private com.lvl6.proto.Element _monsterElement = com.lvl6.proto.Element.NO_ELEMENT;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"monsterElement", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.Element.NO_ELEMENT)]
    public com.lvl6.proto.Element monsterElement
    {
      get { return _monsterElement; }
      set { _monsterElement = value; }
    }

    private string _imagePrefix = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"imagePrefix", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string imagePrefix
    {
      get { return _imagePrefix; }
      set { _imagePrefix = value; }
    }

    private int _numPuzzlePieces = default(int);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"numPuzzlePieces", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numPuzzlePieces
    {
      get { return _numPuzzlePieces; }
      set { _numPuzzlePieces = value; }
    }

    private int _minutesToCombinePieces = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"minutesToCombinePieces", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int minutesToCombinePieces
    {
      get { return _minutesToCombinePieces; }
      set { _minutesToCombinePieces = value; }
    }

    private int _maxLevel = default(int);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"maxLevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int maxLevel
    {
      get { return _maxLevel; }
      set { _maxLevel = value; }
    }

    private int _evolutionMonsterId = default(int);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"evolutionMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int evolutionMonsterId
    {
      get { return _evolutionMonsterId; }
      set { _evolutionMonsterId = value; }
    }

    private int _evolutionCatalystMonsterId = default(int);
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"evolutionCatalystMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int evolutionCatalystMonsterId
    {
      get { return _evolutionCatalystMonsterId; }
      set { _evolutionCatalystMonsterId = value; }
    }

    private int _minutesToEvolve = default(int);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"minutesToEvolve", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int minutesToEvolve
    {
      get { return _minutesToEvolve; }
      set { _minutesToEvolve = value; }
    }

    private int _numCatalystMonstersRequired = default(int);
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"numCatalystMonstersRequired", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numCatalystMonstersRequired
    {
      get { return _numCatalystMonstersRequired; }
      set { _numCatalystMonstersRequired = value; }
    }

    private string _carrotRecruited = "";
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"carrotRecruited", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string carrotRecruited
    {
      get { return _carrotRecruited; }
      set { _carrotRecruited = value; }
    }

    private string _carrotDefeated = "";
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"carrotDefeated", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string carrotDefeated
    {
      get { return _carrotDefeated; }
      set { _carrotDefeated = value; }
    }

    private string _carrotEvolved = "";
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"carrotEvolved", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string carrotEvolved
    {
      get { return _carrotEvolved; }
      set { _carrotEvolved = value; }
    }

    private string _description = "";
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string description
    {
      get { return _description; }
      set { _description = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.MonsterLevelInfoProto> _lvlInfo = new global::System.Collections.Generic.List<com.lvl6.proto.MonsterLevelInfoProto>();
    [global::ProtoBuf.ProtoMember(20, Name=@"lvlInfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.MonsterLevelInfoProto> lvlInfo
    {
      get { return _lvlInfo; }
    }
  

    private int _evolutionCost = default(int);
    [global::ProtoBuf.ProtoMember(21, IsRequired = false, Name=@"evolutionCost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int evolutionCost
    {
      get { return _evolutionCost; }
      set { _evolutionCost = value; }
    }

    private com.lvl6.proto.MonsterProto.AnimationType _attackAnimationType = com.lvl6.proto.MonsterProto.AnimationType.NO_ANIMATION;
    [global::ProtoBuf.ProtoMember(22, IsRequired = false, Name=@"attackAnimationType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.MonsterProto.AnimationType.NO_ANIMATION)]
    public com.lvl6.proto.MonsterProto.AnimationType attackAnimationType
    {
      get { return _attackAnimationType; }
      set { _attackAnimationType = value; }
    }

    private int _verticalPixelOffset = default(int);
    [global::ProtoBuf.ProtoMember(23, IsRequired = false, Name=@"verticalPixelOffset", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int verticalPixelOffset
    {
      get { return _verticalPixelOffset; }
      set { _verticalPixelOffset = value; }
    }

    private string _atkSoundFile = "";
    [global::ProtoBuf.ProtoMember(24, IsRequired = false, Name=@"atkSoundFile", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string atkSoundFile
    {
      get { return _atkSoundFile; }
      set { _atkSoundFile = value; }
    }

    private int _atkSoundAnimationFrame = default(int);
    [global::ProtoBuf.ProtoMember(25, IsRequired = false, Name=@"atkSoundAnimationFrame", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int atkSoundAnimationFrame
    {
      get { return _atkSoundAnimationFrame; }
      set { _atkSoundAnimationFrame = value; }
    }

    private int _atkAnimationRepeatedFramesStart = default(int);
    [global::ProtoBuf.ProtoMember(26, IsRequired = false, Name=@"atkAnimationRepeatedFramesStart", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int atkAnimationRepeatedFramesStart
    {
      get { return _atkAnimationRepeatedFramesStart; }
      set { _atkAnimationRepeatedFramesStart = value; }
    }

    private int _atkAnimationRepeatedFramesEnd = default(int);
    [global::ProtoBuf.ProtoMember(27, IsRequired = false, Name=@"atkAnimationRepeatedFramesEnd", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int atkAnimationRepeatedFramesEnd
    {
      get { return _atkAnimationRepeatedFramesEnd; }
      set { _atkAnimationRepeatedFramesEnd = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"AnimationType")]
    public enum AnimationType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NO_ANIMATION", Value=3)]
      NO_ANIMATION = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MELEE", Value=1)]
      MELEE = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"RANGED", Value=2)]
      RANGED = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MonsterLevelInfoProto")]
  public partial class MonsterLevelInfoProto : global::ProtoBuf.IExtensible
  {
    public MonsterLevelInfoProto() {}
    

    private int _lvl = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"lvl", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int lvl
    {
      get { return _lvl; }
      set { _lvl = value; }
    }

    private int _hp = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"hp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int hp
    {
      get { return _hp; }
      set { _hp = value; }
    }

    private int _curLvlRequiredExp = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"curLvlRequiredExp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int curLvlRequiredExp
    {
      get { return _curLvlRequiredExp; }
      set { _curLvlRequiredExp = value; }
    }

    private int _feederExp = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"feederExp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int feederExp
    {
      get { return _feederExp; }
      set { _feederExp = value; }
    }

    private int _fireDmg = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"fireDmg", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int fireDmg
    {
      get { return _fireDmg; }
      set { _fireDmg = value; }
    }

    private int _grassDmg = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"grassDmg", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int grassDmg
    {
      get { return _grassDmg; }
      set { _grassDmg = value; }
    }

    private int _waterDmg = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"waterDmg", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int waterDmg
    {
      get { return _waterDmg; }
      set { _waterDmg = value; }
    }

    private int _lightningDmg = default(int);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"lightningDmg", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int lightningDmg
    {
      get { return _lightningDmg; }
      set { _lightningDmg = value; }
    }

    private int _darknessDmg = default(int);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"darknessDmg", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int darknessDmg
    {
      get { return _darknessDmg; }
      set { _darknessDmg = value; }
    }

    private int _rockDmg = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"rockDmg", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int rockDmg
    {
      get { return _rockDmg; }
      set { _rockDmg = value; }
    }

    private int _speed = default(int);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"speed", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int speed
    {
      get { return _speed; }
      set { _speed = value; }
    }

    private float _hpExponentBase = default(float);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"hpExponentBase", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float hpExponentBase
    {
      get { return _hpExponentBase; }
      set { _hpExponentBase = value; }
    }

    private float _dmgExponentBase = default(float);
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"dmgExponentBase", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float dmgExponentBase
    {
      get { return _dmgExponentBase; }
      set { _dmgExponentBase = value; }
    }

    private float _expLvlDivisor = default(float);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"expLvlDivisor", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float expLvlDivisor
    {
      get { return _expLvlDivisor; }
      set { _expLvlDivisor = value; }
    }

    private float _expLvlExponent = default(float);
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"expLvlExponent", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float expLvlExponent
    {
      get { return _expLvlExponent; }
      set { _expLvlExponent = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullUserMonsterProto")]
  public partial class FullUserMonsterProto : global::ProtoBuf.IExtensible
  {
    public FullUserMonsterProto() {}
    

    private long _userMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userMonsterId
    {
      get { return _userMonsterId; }
      set { _userMonsterId = value; }
    }

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private int _currentExp = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"currentExp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int currentExp
    {
      get { return _currentExp; }
      set { _currentExp = value; }
    }

    private int _currentLvl = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"currentLvl", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int currentLvl
    {
      get { return _currentLvl; }
      set { _currentLvl = value; }
    }

    private int _currentHealth = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"currentHealth", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int currentHealth
    {
      get { return _currentHealth; }
      set { _currentHealth = value; }
    }

    private int _numPieces = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"numPieces", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numPieces
    {
      get { return _numPieces; }
      set { _numPieces = value; }
    }

    private bool _isComplete = default(bool);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"isComplete", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isComplete
    {
      get { return _isComplete; }
      set { _isComplete = value; }
    }

    private long _combineStartTime = default(long);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"combineStartTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long combineStartTime
    {
      get { return _combineStartTime; }
      set { _combineStartTime = value; }
    }

    private int _teamSlotNum = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"teamSlotNum", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int teamSlotNum
    {
      get { return _teamSlotNum; }
      set { _teamSlotNum = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserMonsterProto")]
  public partial class MinimumUserMonsterProto : global::ProtoBuf.IExtensible
  {
    public MinimumUserMonsterProto() {}
    

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private int _monsterLvl = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"monsterLvl", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterLvl
    {
      get { return _monsterLvl; }
      set { _monsterLvl = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserMonsterHealingProto")]
  public partial class UserMonsterHealingProto : global::ProtoBuf.IExtensible
  {
    public UserMonsterHealingProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private long _userMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userMonsterId
    {
      get { return _userMonsterId; }
      set { _userMonsterId = value; }
    }

    private long _queuedTimeMillis = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"queuedTimeMillis", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long queuedTimeMillis
    {
      get { return _queuedTimeMillis; }
      set { _queuedTimeMillis = value; }
    }

    private float _healthProgress = default(float);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"healthProgress", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float healthProgress
    {
      get { return _healthProgress; }
      set { _healthProgress = value; }
    }

    private int _priority = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"priority", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int priority
    {
      get { return _priority; }
      set { _priority = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserMonsterCurrentHealthProto")]
  public partial class UserMonsterCurrentHealthProto : global::ProtoBuf.IExtensible
  {
    public UserMonsterCurrentHealthProto() {}
    

    private long _userMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userMonsterId
    {
      get { return _userMonsterId; }
      set { _userMonsterId = value; }
    }

    private int _currentHealth = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"currentHealth", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int currentHealth
    {
      get { return _currentHealth; }
      set { _currentHealth = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserEnhancementProto")]
  public partial class UserEnhancementProto : global::ProtoBuf.IExtensible
  {
    public UserEnhancementProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private com.lvl6.proto.UserEnhancementItemProto _baseMonster = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"baseMonster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.UserEnhancementItemProto baseMonster
    {
      get { return _baseMonster; }
      set { _baseMonster = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.UserEnhancementItemProto> _feeders = new global::System.Collections.Generic.List<com.lvl6.proto.UserEnhancementItemProto>();
    [global::ProtoBuf.ProtoMember(3, Name=@"feeders", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.UserEnhancementItemProto> feeders
    {
      get { return _feeders; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserEnhancementItemProto")]
  public partial class UserEnhancementItemProto : global::ProtoBuf.IExtensible
  {
    public UserEnhancementItemProto() {}
    

    private long _userMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userMonsterId
    {
      get { return _userMonsterId; }
      set { _userMonsterId = value; }
    }

    private long _expectedStartTimeMillis = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"expectedStartTimeMillis", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long expectedStartTimeMillis
    {
      get { return _expectedStartTimeMillis; }
      set { _expectedStartTimeMillis = value; }
    }

    private int _enhancingCost = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"enhancingCost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int enhancingCost
    {
      get { return _enhancingCost; }
      set { _enhancingCost = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserMonsterCurrentExpProto")]
  public partial class UserMonsterCurrentExpProto : global::ProtoBuf.IExtensible
  {
    public UserMonsterCurrentExpProto() {}
    

    private long _userMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userMonsterId
    {
      get { return _userMonsterId; }
      set { _userMonsterId = value; }
    }

    private int _expectedExperience = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"expectedExperience", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int expectedExperience
    {
      get { return _expectedExperience; }
      set { _expectedExperience = value; }
    }

    private int _expectedLevel = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"expectedLevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int expectedLevel
    {
      get { return _expectedLevel; }
      set { _expectedLevel = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserMonsterSellProto")]
  public partial class MinimumUserMonsterSellProto : global::ProtoBuf.IExtensible
  {
    public MinimumUserMonsterSellProto() {}
    

    private long _userMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userMonsterId
    {
      get { return _userMonsterId; }
      set { _userMonsterId = value; }
    }

    private int _cashAmount = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"cashAmount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cashAmount
    {
      get { return _cashAmount; }
      set { _cashAmount = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserCurrentMonsterTeamProto")]
  public partial class UserCurrentMonsterTeamProto : global::ProtoBuf.IExtensible
  {
    public UserCurrentMonsterTeamProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullUserMonsterProto> _currentTeam = new global::System.Collections.Generic.List<com.lvl6.proto.FullUserMonsterProto>();
    [global::ProtoBuf.ProtoMember(2, Name=@"currentTeam", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullUserMonsterProto> currentTeam
    {
      get { return _currentTeam; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserMonsterEvolutionProto")]
  public partial class UserMonsterEvolutionProto : global::ProtoBuf.IExtensible
  {
    public UserMonsterEvolutionProto() {}
    

    private long _catalystUserMonsterId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"catalystUserMonsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long catalystUserMonsterId
    {
      get { return _catalystUserMonsterId; }
      set { _catalystUserMonsterId = value; }
    }
    private readonly global::System.Collections.Generic.List<long> _userMonsterIds = new global::System.Collections.Generic.List<long>();
    [global::ProtoBuf.ProtoMember(2, Name=@"userMonsterIds", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<long> userMonsterIds
    {
      get { return _userMonsterIds; }
    }
  

    private long _startTime = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"startTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long startTime
    {
      get { return _startTime; }
      set { _startTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MonsterBattleDialogueProto")]
  public partial class MonsterBattleDialogueProto : global::ProtoBuf.IExtensible
  {
    public MonsterBattleDialogueProto() {}
    

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private com.lvl6.proto.MonsterBattleDialogueProto.DialogueType _dialogueType = com.lvl6.proto.MonsterBattleDialogueProto.DialogueType.NO_DIALOGUE;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"dialogueType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.MonsterBattleDialogueProto.DialogueType.NO_DIALOGUE)]
    public com.lvl6.proto.MonsterBattleDialogueProto.DialogueType dialogueType
    {
      get { return _dialogueType; }
      set { _dialogueType = value; }
    }

    private string _dialogue = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"dialogue", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string dialogue
    {
      get { return _dialogue; }
      set { _dialogue = value; }
    }

    private float _probabilityUttered = default(float);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"probabilityUttered", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float probabilityUttered
    {
      get { return _probabilityUttered; }
      set { _probabilityUttered = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"DialogueType")]
    public enum DialogueType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NO_DIALOGUE", Value=2)]
      NO_DIALOGUE = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ENTER_BATTLE", Value=1)]
      ENTER_BATTLE = 1
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}