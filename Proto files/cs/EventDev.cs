//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventDev.proto
// Note: requires additional types generated from: Dev.proto
// Note: requires additional types generated from: MonsterStuff.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DevRequestProto")]
  public partial class DevRequestProto : global::ProtoBuf.IExtensible
  {
    public DevRequestProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.DevRequest _devRequest = com.lvl6.proto.DevRequest.GET_ITEM;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"devRequest", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.DevRequest.GET_ITEM)]
    public com.lvl6.proto.DevRequest devRequest
    {
      get { return _devRequest; }
      set { _devRequest = value; }
    }

    private int _staticDataId = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"staticDataId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int staticDataId
    {
      get { return _staticDataId; }
      set { _staticDataId = value; }
    }

    private int _quantity = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"quantity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DevResponseProto")]
  public partial class DevResponseProto : global::ProtoBuf.IExtensible
  {
    public DevResponseProto() {}
    

    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private com.lvl6.proto.DevResponseProto.DevStatus _status = com.lvl6.proto.DevResponseProto.DevStatus.SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(com.lvl6.proto.DevResponseProto.DevStatus.SUCCESS)]
    public com.lvl6.proto.DevResponseProto.DevStatus status
    {
      get { return _status; }
      set { _status = value; }
    }

    private com.lvl6.proto.FullUserMonsterProto _fump = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"fump", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.FullUserMonsterProto fump
    {
      get { return _fump; }
      set { _fump = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"DevStatus")]
    public enum DevStatus
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