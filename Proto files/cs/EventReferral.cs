//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: EventReferral.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReferralCodeUsedResponseProto")]
  public partial class ReferralCodeUsedResponseProto : global::ProtoBuf.IExtensible
  {
    public ReferralCodeUsedResponseProto() {}
    
    private com.lvl6.proto.MinimumUserProto _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto sender
    {
      get { return _sender; }
      set { _sender = value; }
    }
    private com.lvl6.proto.MinimumUserProto _referredPlayer = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"referredPlayer", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProto referredPlayer
    {
      get { return _referredPlayer; }
      set { _referredPlayer = value; }
    }
    private int _coinsGivenToReferrer = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"coinsGivenToReferrer", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int coinsGivenToReferrer
    {
      get { return _coinsGivenToReferrer; }
      set { _coinsGivenToReferrer = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}