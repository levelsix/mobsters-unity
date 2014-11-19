//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventUser.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: Structure.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserCreateRequestProto")]
  public partial class UserCreateRequestProto : global::ProtoBuf.IExtensible
  {
    public UserCreateRequestProto() {}
    

    private string _udid = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"udid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string udid
    {
      get { return _udid; }
      set { _udid = value; }
    }

    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _deviceToken = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"deviceToken", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string deviceToken
    {
      get { return _deviceToken; }
      set { _deviceToken = value; }
    }

    private string _facebookId = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"facebookId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string facebookId
    {
      get { return _facebookId; }
      set { _facebookId = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.TutorialStructProto> _structsJustBuilt = new global::System.Collections.Generic.List<com.lvl6.proto.TutorialStructProto>();
    [global::ProtoBuf.ProtoMember(5, Name=@"structsJustBuilt", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.TutorialStructProto> structsJustBuilt
    {
      get { return _structsJustBuilt; }
    }
  

    private int _cash = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"cash", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cash
    {
      get { return _cash; }
      set { _cash = value; }
    }

    private int _oil = default(int);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"oil", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int oil
    {
      get { return _oil; }
      set { _oil = value; }
    }

    private int _gems = default(int);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"gems", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gems
    {
      get { return _gems; }
      set { _gems = value; }
    }

    private string _email = "";
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"email", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string email
    {
      get { return _email; }
      set { _email = value; }
    }

    private string _fbData = "";
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"fbData", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string fbData
    {
      get { return _fbData; }
      set { _fbData = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserCreateResponseProto")]
  public partial class UserCreateResponseProto : global::ProtoBuf.IExtensible
  {
    public UserCreateResponseProto() {}
    

    private com.lvl6.proto.UserCreateResponseProto.UserCreateStatus _status = com.lvl6.proto.UserCreateResponseProto.UserCreateStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UserCreateResponseProto.UserCreateStatus.SUCCESS)]
    public com.lvl6.proto.UserCreateResponseProto.UserCreateStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"UserCreateStatus")]
    public enum UserCreateStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INVALID_NAME", Value=2)]
      FAIL_INVALID_NAME = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_USER_WITH_UDID_ALREADY_EXISTS", Value=3)]
      FAIL_USER_WITH_UDID_ALREADY_EXISTS = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INVALID_REFER_CODE", Value=4)]
      FAIL_INVALID_REFER_CODE = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_USER_WITH_FACEBOOK_ID_EXISTS", Value=5)]
      FAIL_USER_WITH_FACEBOOK_ID_EXISTS = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=6)]
      FAIL_OTHER = 6
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LevelUpRequestProto")]
  public partial class LevelUpRequestProto : global::ProtoBuf.IExtensible
  {
    public LevelUpRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _nextLevel = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"nextLevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int nextLevel
    {
      get { return _nextLevel; }
      set { _nextLevel = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LevelUpResponseProto")]
  public partial class LevelUpResponseProto : global::ProtoBuf.IExtensible
  {
    public LevelUpResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.LevelUpResponseProto.LevelUpStatus _status = com.lvl6.proto.LevelUpResponseProto.LevelUpStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.LevelUpResponseProto.LevelUpStatus.SUCCESS)]
    public com.lvl6.proto.LevelUpResponseProto.LevelUpStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"LevelUpStatus")]
    public enum LevelUpStatus
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RetrieveUsersForUserIdsRequestProto")]
  public partial class RetrieveUsersForUserIdsRequestProto : global::ProtoBuf.IExtensible
  {
    public RetrieveUsersForUserIdsRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private readonly global::System.Collections.Generic.List<string> _requestedUserUuids = new global::System.Collections.Generic.List<string>();
    [global::ProtoBuf.ProtoMember(2, Name=@"requestedUserUuids", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<string> requestedUserUuids
    {
      get { return _requestedUserUuids; }
    }
  

    private bool _includeCurMonsterTeam = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"includeCurMonsterTeam", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool includeCurMonsterTeam
    {
      get { return _includeCurMonsterTeam; }
      set { _includeCurMonsterTeam = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RetrieveUsersForUserIdsResponseProto")]
  public partial class RetrieveUsersForUserIdsResponseProto : global::ProtoBuf.IExtensible
  {
    public RetrieveUsersForUserIdsResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.FullUserProto> _requestedUsers = new global::System.Collections.Generic.List<com.lvl6.proto.FullUserProto>();
    [global::ProtoBuf.ProtoMember(2, Name=@"requestedUsers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.FullUserProto> requestedUsers
    {
      get { return _requestedUsers; }
    }
  
    private readonly global::System.Collections.Generic.List<com.lvl6.proto.UserCurrentMonsterTeamProto> _curTeam = new global::System.Collections.Generic.List<com.lvl6.proto.UserCurrentMonsterTeamProto>();
    [global::ProtoBuf.ProtoMember(3, Name=@"curTeam", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<com.lvl6.proto.UserCurrentMonsterTeamProto> curTeam
    {
      get { return _curTeam; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LogoutRequestProto")]
  public partial class LogoutRequestProto : global::ProtoBuf.IExtensible
  {
    public LogoutRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpdateClientUserResponseProto")]
  public partial class UpdateClientUserResponseProto : global::ProtoBuf.IExtensible
  {
    public UpdateClientUserResponseProto() {}
    

    private com.lvl6.proto.FullUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.FullUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private long _timeOfUserUpdate = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"timeOfUserUpdate", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfUserUpdate
    {
      get { return _timeOfUserUpdate; }
      set { _timeOfUserUpdate = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetFacebookIdRequestProto")]
  public partial class SetFacebookIdRequestProto : global::ProtoBuf.IExtensible
  {
    public SetFacebookIdRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private string _fbId = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"fbId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string fbId
    {
      get { return _fbId; }
      set { _fbId = value; }
    }

    private bool _isUserCreate = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"isUserCreate", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isUserCreate
    {
      get { return _isUserCreate; }
      set { _isUserCreate = value; }
    }

    private string _email = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"email", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string email
    {
      get { return _email; }
      set { _email = value; }
    }

    private string _fbData = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"fbData", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string fbData
    {
      get { return _fbData; }
      set { _fbData = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetFacebookIdResponseProto")]
  public partial class SetFacebookIdResponseProto : global::ProtoBuf.IExtensible
  {
    public SetFacebookIdResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.SetFacebookIdResponseProto.SetFacebookIdStatus _status = com.lvl6.proto.SetFacebookIdResponseProto.SetFacebookIdStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.SetFacebookIdResponseProto.SetFacebookIdStatus.SUCCESS)]
    public com.lvl6.proto.SetFacebookIdResponseProto.SetFacebookIdStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private com.lvl6.proto.MinimumUserProto _existing = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"existing", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto existing
    {
      get { return _existing; }
      set { _existing = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"SetFacebookIdStatus")]
    public enum SetFacebookIdStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=2)]
      FAIL_OTHER = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_FB_ID_EXISTS", Value=3)]
      FAIL_FB_ID_EXISTS = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_USER_FB_ID_ALREADY_SET", Value=4)]
      FAIL_USER_FB_ID_ALREADY_SET = 4
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpdateUserCurrencyRequestProto")]
  public partial class UpdateUserCurrencyRequestProto : global::ProtoBuf.IExtensible
  {
    public UpdateUserCurrencyRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _cashSpent = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"cashSpent", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int cashSpent
    {
      get { return _cashSpent; }
      set { _cashSpent = value; }
    }

    private int _oilSpent = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"oilSpent", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int oilSpent
    {
      get { return _oilSpent; }
      set { _oilSpent = value; }
    }

    private int _gemsSpent = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"gemsSpent", DataFormat = global::ProtoBuf.DataFormat.ZigZag)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int gemsSpent
    {
      get { return _gemsSpent; }
      set { _gemsSpent = value; }
    }

    private long _clientTime = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"clientTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long clientTime
    {
      get { return _clientTime; }
      set { _clientTime = value; }
    }

    private string _reason = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"reason", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string reason
    {
      get { return _reason; }
      set { _reason = value; }
    }

    private string _details = "";
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"details", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string details
    {
      get { return _details; }
      set { _details = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpdateUserCurrencyResponseProto")]
  public partial class UpdateUserCurrencyResponseProto : global::ProtoBuf.IExtensible
  {
    public UpdateUserCurrencyResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.UpdateUserCurrencyResponseProto.UpdateUserCurrencyStatus _status = com.lvl6.proto.UpdateUserCurrencyResponseProto.UpdateUserCurrencyStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.UpdateUserCurrencyResponseProto.UpdateUserCurrencyStatus.SUCCESS)]
    public com.lvl6.proto.UpdateUserCurrencyResponseProto.UpdateUserCurrencyStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"UpdateUserCurrencyStatus")]
    public enum UpdateUserCurrencyStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_OTHER", Value=2)]
      FAIL_OTHER = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_CASH", Value=3)]
      FAIL_INSUFFICIENT_CASH = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_OIL", Value=4)]
      FAIL_INSUFFICIENT_OIL = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL_INSUFFICIENT_GEMS", Value=5)]
      FAIL_INSUFFICIENT_GEMS = 5
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetGameCenterIdRequestProto")]
  public partial class SetGameCenterIdRequestProto : global::ProtoBuf.IExtensible
  {
    public SetGameCenterIdRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private string _gameCenterId = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"gameCenterId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string gameCenterId
    {
      get { return _gameCenterId; }
      set { _gameCenterId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetGameCenterIdResponseProto")]
  public partial class SetGameCenterIdResponseProto : global::ProtoBuf.IExtensible
  {
    public SetGameCenterIdResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private string _gameCenterId = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"gameCenterId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string gameCenterId
    {
      get { return _gameCenterId; }
      set { _gameCenterId = value; }
    }

    private com.lvl6.proto.SetGameCenterIdResponseProto.SetGameCenterIdStatus _status = com.lvl6.proto.SetGameCenterIdResponseProto.SetGameCenterIdStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.SetGameCenterIdResponseProto.SetGameCenterIdStatus.SUCCESS)]
    public com.lvl6.proto.SetGameCenterIdResponseProto.SetGameCenterIdStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"SetGameCenterIdStatus")]
    public enum SetGameCenterIdStatus
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetAvatarMonsterRequestProto")]
  public partial class SetAvatarMonsterRequestProto : global::ProtoBuf.IExtensible
  {
    public SetAvatarMonsterRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private int _monsterId = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"monsterId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SetAvatarMonsterResponseProto")]
  public partial class SetAvatarMonsterResponseProto : global::ProtoBuf.IExtensible
  {
    public SetAvatarMonsterResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.SetAvatarMonsterResponseProto.SetAvatarMonsterStatus _status = com.lvl6.proto.SetAvatarMonsterResponseProto.SetAvatarMonsterStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.SetAvatarMonsterResponseProto.SetAvatarMonsterStatus.SUCCESS)]
    public com.lvl6.proto.SetAvatarMonsterResponseProto.SetAvatarMonsterStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"SetAvatarMonsterStatus")]
    public enum SetAvatarMonsterStatus
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