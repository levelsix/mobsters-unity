//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventApns.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EnableAPNSRequestProto")]
  public partial class EnableAPNSRequestProto : global::ProtoBuf.IExtensible
  {
    public EnableAPNSRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private string _deviceToken = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"deviceToken", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string deviceToken
    {
      get { return _deviceToken; }
      set { _deviceToken = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EnableAPNSResponseProto")]
  public partial class EnableAPNSResponseProto : global::ProtoBuf.IExtensible
  {
    public EnableAPNSResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.EnableAPNSResponseProto.EnableAPNSStatus _status = com.lvl6.proto.EnableAPNSResponseProto.EnableAPNSStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.EnableAPNSResponseProto.EnableAPNSStatus.SUCCESS)]
    public com.lvl6.proto.EnableAPNSResponseProto.EnableAPNSStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"EnableAPNSStatus")]
    public enum EnableAPNSStatus
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=1)]
      SUCCESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOT_ENABLED", Value=2)]
      NOT_ENABLED = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}