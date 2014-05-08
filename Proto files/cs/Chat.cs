//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Chat.proto
// Note: requires additional types generated from: User.proto
namespace com.lvl6.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PrivateChatPostProto")]
  public partial class PrivateChatPostProto : global::ProtoBuf.IExtensible
  {
    public PrivateChatPostProto() {}
    

    private int _privateChatPostId = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"privateChatPostId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int privateChatPostId
    {
      get { return _privateChatPostId; }
      set { _privateChatPostId = value; }
    }

    private com.lvl6.proto.MinimumUserProtoWithLevel _poster = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"poster", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithLevel poster
    {
      get { return _poster; }
      set { _poster = value; }
    }

    private com.lvl6.proto.MinimumUserProtoWithLevel _recipient = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"recipient", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithLevel recipient
    {
      get { return _recipient; }
      set { _recipient = value; }
    }

    private long _timeOfPost = default(long);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"timeOfPost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfPost
    {
      get { return _timeOfPost; }
      set { _timeOfPost = value; }
    }

    private string _content = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"content", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string content
    {
      get { return _content; }
      set { _content = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ColorProto")]
  public partial class ColorProto : global::ProtoBuf.IExtensible
  {
    public ColorProto() {}
    

    private int _red = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"red", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int red
    {
      get { return _red; }
      set { _red = value; }
    }

    private int _green = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"green", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int green
    {
      get { return _green; }
      set { _green = value; }
    }

    private int _blue = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"blue", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int blue
    {
      get { return _blue; }
      set { _blue = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GroupChatMessageProto")]
  public partial class GroupChatMessageProto : global::ProtoBuf.IExtensible
  {
    public GroupChatMessageProto() {}
    

    private com.lvl6.proto.MinimumUserProtoWithLevel _sender = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sender", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public com.lvl6.proto.MinimumUserProtoWithLevel sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    private long _timeOfChat = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"timeOfChat", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timeOfChat
    {
      get { return _timeOfChat; }
      set { _timeOfChat = value; }
    }

    private string _content = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"content", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string content
    {
      get { return _content; }
      set { _content = value; }
    }

    private bool _isAdmin = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"isAdmin", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool isAdmin
    {
      get { return _isAdmin; }
      set { _isAdmin = value; }
    }

    private int _chatId = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"chatId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int chatId
    {
      get { return _chatId; }
      set { _chatId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"GroupChatScope")]
    public enum GroupChatScope
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"CLAN", Value=1)]
      CLAN = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GLOBAL", Value=2)]
      GLOBAL = 2
    }
  
}