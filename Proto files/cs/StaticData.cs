//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: StaticData.proto
// Note: requires additional types generated from: BoosterPackStuff.proto
// Note: requires additional types generated from: City.proto
// Note: requires additional types generated from: Clan.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: Quest.proto
// Note: requires additional types generated from: Structure.proto
// Note: requires additional types generated from: Task.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StaticDataProto")]
  public partial class StaticDataProto : global::ProtoBuf.IExtensible
  {
    public StaticDataProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.CityExpansionCostProto> _expansionCosts = new global::System.Collections.Generic.List<com.lvl6.proto.CityExpansionCostProto>();
    [global::ProtoBuf.ProtoMember(2, Name=@"expansionCosts", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.CityExpansionCostProto> expansionCosts
    {
      get { return _expansionCosts; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullCityProto> _allCities = new global::System.Collections.Generic.List<com.lvl6.proto.FullCityProto>();
    [global::ProtoBuf.ProtoMember(3, Name=@"allCities", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullCityProto> allCities
    {
      get { return _allCities; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullTaskProto> _allTasks = new global::System.Collections.Generic.List<com.lvl6.proto.FullTaskProto>();
    [global::ProtoBuf.ProtoMember(4, Name=@"allTasks", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullTaskProto> allTasks
    {
      get { return _allTasks; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.MonsterProto> _allMonsters = new global::System.Collections.Generic.List<com.lvl6.proto.MonsterProto>();
    [global::ProtoBuf.ProtoMember(5, Name=@"allMonsters", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.MonsterProto> allMonsters
    {
      get { return _allMonsters; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.StaticUserLevelInfoProto> _slip = new global::System.Collections.Generic.List<com.lvl6.proto.StaticUserLevelInfoProto>();
    [global::ProtoBuf.ProtoMember(6, Name=@"slip", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.StaticUserLevelInfoProto> slip
    {
      get { return _slip; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto> _inProgressQuests = new global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto>();
    [global::ProtoBuf.ProtoMember(7, Name=@"inProgressQuests", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto> inProgressQuests
    {
      get { return _inProgressQuests; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto> _unredeemedQuests = new global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto>();
    [global::ProtoBuf.ProtoMember(8, Name=@"unredeemedQuests", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto> unredeemedQuests
    {
      get { return _unredeemedQuests; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto> _availableQuests = new global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto>();
    [global::ProtoBuf.ProtoMember(9, Name=@"availableQuests", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullQuestProto> availableQuests
    {
      get { return _availableQuests; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.BoosterPackProto> _boosterPacks = new global::System.Collections.Generic.List<com.lvl6.proto.BoosterPackProto>();
    [global::ProtoBuf.ProtoMember(11, Name=@"boosterPacks", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.BoosterPackProto> boosterPacks
    {
      get { return _boosterPacks; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ResourceGeneratorProto> _allGenerators = new global::System.Collections.Generic.List<com.lvl6.proto.ResourceGeneratorProto>();
    [global::ProtoBuf.ProtoMember(12, Name=@"allGenerators", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ResourceGeneratorProto> allGenerators
    {
      get { return _allGenerators; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ResourceStorageProto> _allStorages = new global::System.Collections.Generic.List<com.lvl6.proto.ResourceStorageProto>();
    [global::ProtoBuf.ProtoMember(13, Name=@"allStorages", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ResourceStorageProto> allStorages
    {
      get { return _allStorages; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.HospitalProto> _allHospitals = new global::System.Collections.Generic.List<com.lvl6.proto.HospitalProto>();
    [global::ProtoBuf.ProtoMember(14, Name=@"allHospitals", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.HospitalProto> allHospitals
    {
      get { return _allHospitals; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ResidenceProto> _allResidences = new global::System.Collections.Generic.List<com.lvl6.proto.ResidenceProto>();
    [global::ProtoBuf.ProtoMember(15, Name=@"allResidences", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ResidenceProto> allResidences
    {
      get { return _allResidences; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.LabProto> _allLabs = new global::System.Collections.Generic.List<com.lvl6.proto.LabProto>();
    [global::ProtoBuf.ProtoMember(17, Name=@"allLabs", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.LabProto> allLabs
    {
      get { return _allLabs; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.TownHallProto> _allTownHalls = new global::System.Collections.Generic.List<com.lvl6.proto.TownHallProto>();
    [global::ProtoBuf.ProtoMember(16, Name=@"allTownHalls", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.TownHallProto> allTownHalls
    {
      get { return _allTownHalls; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.PersistentEventProto> _persistentEvents = new global::System.Collections.Generic.List<com.lvl6.proto.PersistentEventProto>();
    [global::ProtoBuf.ProtoMember(18, Name=@"persistentEvents", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.PersistentEventProto> persistentEvents
    {
      get { return _persistentEvents; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.MonsterBattleDialogueProto> _mbds = new global::System.Collections.Generic.List<com.lvl6.proto.MonsterBattleDialogueProto>();
    [global::ProtoBuf.ProtoMember(19, Name=@"mbds", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.MonsterBattleDialogueProto> mbds
    {
      get { return _mbds; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidProto> _raids = new global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidProto>();
    [global::ProtoBuf.ProtoMember(20, Name=@"raids", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ClanRaidProto> raids
    {
      get { return _raids; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.PersistentClanEventProto> _persistentClanEvents = new global::System.Collections.Generic.List<com.lvl6.proto.PersistentClanEventProto>();
    [global::ProtoBuf.ProtoMember(21, Name=@"persistentClanEvents", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.PersistentClanEventProto> persistentClanEvents
    {
      get { return _persistentClanEvents; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ItemProto> _items = new global::System.Collections.Generic.List<com.lvl6.proto.ItemProto>();
    [global::ProtoBuf.ProtoMember(22, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ItemProto> items
    {
      get { return _items; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ObstacleProto> _obstacles = new global::System.Collections.Generic.List<com.lvl6.proto.ObstacleProto>();
    [global::ProtoBuf.ProtoMember(23, Name=@"obstacles", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ObstacleProto> obstacles
    {
      get { return _obstacles; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.ClanIconProto> _clanIcons = new global::System.Collections.Generic.List<com.lvl6.proto.ClanIconProto>();
    [global::ProtoBuf.ProtoMember(24, Name=@"clanIcons", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.ClanIconProto> clanIcons
    {
      get { return _clanIcons; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}