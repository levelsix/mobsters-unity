//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Task.proto
// Note: requires additional types generated from: MonsterStuff.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TaskStageProto")]
  public partial class TaskStageProto : global::ProtoBuf.IExtensible
  {
    public TaskStageProto() {}
    

    private int _stageId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"stageId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int stageId
    {
      get { return _stageId; }
      set { _stageId = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.TaskStageMonsterProto> _stageMonsters = new global::System.Collections.Generic.List<com.lvl6.proto.TaskStageMonsterProto>();
    [global::ProtoBuf.ProtoMember(2, Name=@"stageMonsters", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.TaskStageMonsterProto> stageMonsters
    {
      get { return _stageMonsters; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FullTaskProto")]
  public partial class FullTaskProto : global::ProtoBuf.IExtensible
  {
    public FullTaskProto() {}
    

    private int _taskId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"taskId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int taskId
    {
      get { return _taskId; }
      set { _taskId = value; }
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

    private int _cityId = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"cityId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cityId
    {
      get { return _cityId; }
      set { _cityId = value; }
    }

    private int _assetNumWithinCity = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"assetNumWithinCity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int assetNumWithinCity
    {
      get { return _assetNumWithinCity; }
      set { _assetNumWithinCity = value; }
    }

    private int _prerequisiteTaskId = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"prerequisiteTaskId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int prerequisiteTaskId
    {
      get { return _prerequisiteTaskId; }
      set { _prerequisiteTaskId = value; }
    }

    private int _prerequisiteQuestId = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"prerequisiteQuestId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int prerequisiteQuestId
    {
      get { return _prerequisiteQuestId; }
      set { _prerequisiteQuestId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MinimumUserTaskProto")]
  public partial class MinimumUserTaskProto : global::ProtoBuf.IExtensible
  {
    public MinimumUserTaskProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _taskId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"taskId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int taskId
    {
      get { return _taskId; }
      set { _taskId = value; }
    }

    private int _numTimesActed = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"numTimesActed", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int numTimesActed
    {
      get { return _numTimesActed; }
      set { _numTimesActed = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TaskStageMonsterProto")]
  public partial class TaskStageMonsterProto : global::ProtoBuf.IExtensible
  {
    public TaskStageMonsterProto() {}
    

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int monsterId
    {
      get { return _monsterId; }
      set { _monsterId = value; }
    }

    private com.lvl6.proto.TaskStageMonsterProto.MonsterType _monsterType = com.lvl6.proto.TaskStageMonsterProto.MonsterType.REGULAR;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"monsterType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.TaskStageMonsterProto.MonsterType.REGULAR)]
    public com.lvl6.proto.TaskStageMonsterProto.MonsterType monsterType
    {
      get { return _monsterType; }
      set { _monsterType = value; }
    }

    private int _expReward = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"expReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int expReward
    {
      get { return _expReward; }
      set { _expReward = value; }
    }

    private int _cashReward = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"cashReward", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cashReward
    {
      get { return _cashReward; }
      set { _cashReward = value; }
    }

    private bool _puzzlePieceDropped = default(bool);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"puzzlePieceDropped", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool puzzlePieceDropped
    {
      get { return _puzzlePieceDropped; }
      set { _puzzlePieceDropped = value; }
    }

    private int _level = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int level
    {
      get { return _level; }
      set { _level = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"MonsterType")]
    public enum MonsterType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"REGULAR", Value=1)]
      REGULAR = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MINI_BOSS", Value=2)]
      MINI_BOSS = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BOSS", Value=3)]
      BOSS = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersistentEventProto")]
  public partial class PersistentEventProto : global::ProtoBuf.IExtensible
  {
    public PersistentEventProto() {}
    

    private int _eventId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"eventId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eventId
    {
      get { return _eventId; }
      set { _eventId = value; }
    }

    private com.lvl6.proto.PersistentEventProto.DayOfWeek _dayOfWeek = com.lvl6.proto.PersistentEventProto.DayOfWeek.SUNDAY;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"dayOfWeek", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.PersistentEventProto.DayOfWeek.SUNDAY)]
    public com.lvl6.proto.PersistentEventProto.DayOfWeek dayOfWeek
    {
      get { return _dayOfWeek; }
      set { _dayOfWeek = value; }
    }

    private int _startHour = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"startHour", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int startHour
    {
      get { return _startHour; }
      set { _startHour = value; }
    }

    private int _eventDurationMinutes = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"eventDurationMinutes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eventDurationMinutes
    {
      get { return _eventDurationMinutes; }
      set { _eventDurationMinutes = value; }
    }

    private int _taskId = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"taskId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int taskId
    {
      get { return _taskId; }
      set { _taskId = value; }
    }

    private int _cooldownMinutes = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"cooldownMinutes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cooldownMinutes
    {
      get { return _cooldownMinutes; }
      set { _cooldownMinutes = value; }
    }

    private com.lvl6.proto.PersistentEventProto.EventType _type = com.lvl6.proto.PersistentEventProto.EventType.ENHANCE;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.PersistentEventProto.EventType.ENHANCE)]
    public com.lvl6.proto.PersistentEventProto.EventType type
    {
      get { return _type; }
      set { _type = value; }
    }

    private com.lvl6.proto.MonsterProto.MonsterElement _monsterElement = com.lvl6.proto.MonsterProto.MonsterElement.FIRE;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"monsterElement", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.MonsterProto.MonsterElement.FIRE)]
    public com.lvl6.proto.MonsterProto.MonsterElement monsterElement
    {
      get { return _monsterElement; }
      set { _monsterElement = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"DayOfWeek")]
    public enum DayOfWeek
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUNDAY", Value=1)]
      SUNDAY = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MONDAY", Value=2)]
      MONDAY = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"TUESDAY", Value=3)]
      TUESDAY = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"WEDNESDAY", Value=4)]
      WEDNESDAY = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"THURSDAY", Value=5)]
      THURSDAY = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FRIDAY", Value=6)]
      FRIDAY = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SATURDAY", Value=7)]
      SATURDAY = 7
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"EventType")]
    public enum EventType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"ENHANCE", Value=1)]
      ENHANCE = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"EVOLUTION", Value=2)]
      EVOLUTION = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserPersistentEventProto")]
  public partial class UserPersistentEventProto : global::ProtoBuf.IExtensible
  {
    public UserPersistentEventProto() {}
    

    private int _userId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int userId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    private int _eventId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"eventId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eventId
    {
      get { return _eventId; }
      set { _eventId = value; }
    }

    private long _coolDownStartTime = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"coolDownStartTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long coolDownStartTime
    {
      get { return _coolDownStartTime; }
      set { _coolDownStartTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}