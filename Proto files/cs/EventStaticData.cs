//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventStaticData.proto
// Note: requires additional types generated from: StaticData.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PurgeClientStaticDataResponseProto")]
  public partial class PurgeClientStaticDataResponseProto : global::ProtoBuf.IExtensible
  {
    public PurgeClientStaticDataResponseProto() {}
    
    private string _senderUuid = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"senderUuid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string senderUuid
    {
      get { return _senderUuid; }
      set { _senderUuid = value; }
    }
    private com.lvl6.proto.StaticDataProto _staticDataStuff = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"staticDataStuff", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.StaticDataProto staticDataStuff
    {
      get { return _staticDataStuff; }
      set { _staticDataStuff = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}