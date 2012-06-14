// Generated by ProtoGen, Version=2.3.0.277, Culture=neutral, PublicKeyToken=8fd7408b07f8d2cd.  DO NOT EDIT!

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace Nohros.Ruby {
  
  namespace Proto {
    
    public static partial class RubyMessageHeader {
    
      #region Extension registration
      public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
      }
      #endregion
      #region Static variables
      internal static pbd::MessageDescriptor internal__static_nohros_ruby_RubyMessageHeader__Descriptor;
      internal static pb::FieldAccess.FieldAccessorTable<global::Nohros.Ruby.RubyMessageHeader, global::Nohros.Ruby.RubyMessageHeader.Builder> internal__static_nohros_ruby_RubyMessageHeader__FieldAccessorTable;
      #endregion
      #region Descriptor
      public static pbd::FileDescriptor Descriptor {
        get { return descriptor; }
      }
      private static pbd::FileDescriptor descriptor;
      
      static RubyMessageHeader() {
        byte[] descriptorData = global::System.Convert.FromBase64String(
            "ChlydWJ5X21lc3NhZ2VfaGVhZGVyLnByb3RvEgtub2hyb3MucnVieRokZ29v" + 
            "Z2xlL3Byb3RvYnVmL2NzaGFycF9vcHRpb25zLnByb3RvIkMKEVJ1YnlNZXNz" + 
            "YWdlSGVhZGVyEgoKAmlkGAEgASgFEgwKBHNpemUYAiABKAUSFAoMbWVzc2Fn" + 
            "ZV90eXBlGAMgASgJQhJIAcI+DQoLTm9ocm9zLlJ1Ynk=");
        pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
          descriptor = root;
          internal__static_nohros_ruby_RubyMessageHeader__Descriptor = Descriptor.MessageTypes[0];
          internal__static_nohros_ruby_RubyMessageHeader__FieldAccessorTable = 
              new pb::FieldAccess.FieldAccessorTable<global::Nohros.Ruby.RubyMessageHeader, global::Nohros.Ruby.RubyMessageHeader.Builder>(internal__static_nohros_ruby_RubyMessageHeader__Descriptor,
                  new string[] { "Id", "Size", "MessageType", });
          pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();
          RegisterAllExtensions(registry);
          global::Google.ProtocolBuffers.DescriptorProtos.CSharpOptions.RegisterAllExtensions(registry);
          return registry;
        };
        pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
            new pbd::FileDescriptor[] {
            global::Google.ProtocolBuffers.DescriptorProtos.CSharpOptions.Descriptor, 
            }, assigner);
      }
      #endregion
      
    }
  }
  #region Messages
  public sealed partial class RubyMessageHeader : pb::GeneratedMessage<RubyMessageHeader, RubyMessageHeader.Builder> {
    private static readonly RubyMessageHeader defaultInstance = new Builder().BuildPartial();
    public static RubyMessageHeader DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override RubyMessageHeader DefaultInstanceForType {
      get { return defaultInstance; }
    }
    
    protected override RubyMessageHeader ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::Nohros.Ruby.Proto.RubyMessageHeader.internal__static_nohros_ruby_RubyMessageHeader__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<RubyMessageHeader, RubyMessageHeader.Builder> InternalFieldAccessors {
      get { return global::Nohros.Ruby.Proto.RubyMessageHeader.internal__static_nohros_ruby_RubyMessageHeader__FieldAccessorTable; }
    }
    
    public const int IdFieldNumber = 1;
    private bool hasId;
    private int id_ = 0;
    public bool HasId {
      get { return hasId; }
    }
    public int Id {
      get { return id_; }
    }
    
    public const int SizeFieldNumber = 2;
    private bool hasSize;
    private int size_ = 0;
    public bool HasSize {
      get { return hasSize; }
    }
    public int Size {
      get { return size_; }
    }
    
    public const int MessageTypeFieldNumber = 3;
    private bool hasMessageType;
    private string messageType_ = "";
    public bool HasMessageType {
      get { return hasMessageType; }
    }
    public string MessageType {
      get { return messageType_; }
    }
    
    public override bool IsInitialized {
      get {
        return true;
      }
    }
    
    public override void WriteTo(pb::CodedOutputStream output) {
      int size = SerializedSize;
      if (HasId) {
        output.WriteInt32(1, Id);
      }
      if (HasSize) {
        output.WriteInt32(2, Size);
      }
      if (HasMessageType) {
        output.WriteString(3, MessageType);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (HasId) {
          size += pb::CodedOutputStream.ComputeInt32Size(1, Id);
        }
        if (HasSize) {
          size += pb::CodedOutputStream.ComputeInt32Size(2, Size);
        }
        if (HasMessageType) {
          size += pb::CodedOutputStream.ComputeStringSize(3, MessageType);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static RubyMessageHeader ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static RubyMessageHeader ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static RubyMessageHeader ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(pb::CodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static RubyMessageHeader ParseFrom(pb::CodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(RubyMessageHeader prototype) {
      return (Builder) new Builder().MergeFrom(prototype);
    }
    
    public sealed partial class Builder : pb::GeneratedBuilder<RubyMessageHeader, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {}
      
      RubyMessageHeader result = new RubyMessageHeader();
      
      protected override RubyMessageHeader MessageBeingBuilt {
        get { return result; }
      }
      
      public override Builder Clear() {
        result = new RubyMessageHeader();
        return this;
      }
      
      public override Builder Clone() {
        return new Builder().MergeFrom(result);
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::Nohros.Ruby.RubyMessageHeader.Descriptor; }
      }
      
      public override RubyMessageHeader DefaultInstanceForType {
        get { return global::Nohros.Ruby.RubyMessageHeader.DefaultInstance; }
      }
      
      public override RubyMessageHeader BuildPartial() {
        if (result == null) {
          throw new global::System.InvalidOperationException("build() has already been called on this Builder");
        }
        RubyMessageHeader returnMe = result;
        result = null;
        return returnMe;
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is RubyMessageHeader) {
          return MergeFrom((RubyMessageHeader) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(RubyMessageHeader other) {
        if (other == global::Nohros.Ruby.RubyMessageHeader.DefaultInstance) return this;
        if (other.HasId) {
          Id = other.Id;
        }
        if (other.HasSize) {
          Size = other.Size;
        }
        if (other.HasMessageType) {
          MessageType = other.MessageType;
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::CodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::CodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        pb::UnknownFieldSet.Builder unknownFields = null;
        while (true) {
          uint tag = input.ReadTag();
          switch (tag) {
            case 0: {
              if (unknownFields != null) {
                this.UnknownFields = unknownFields.Build();
              }
              return this;
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag);
              break;
            }
            case 8: {
              Id = input.ReadInt32();
              break;
            }
            case 16: {
              Size = input.ReadInt32();
              break;
            }
            case 26: {
              MessageType = input.ReadString();
              break;
            }
          }
        }
      }
      
      
      public bool HasId {
        get { return result.HasId; }
      }
      public int Id {
        get { return result.Id; }
        set { SetId(value); }
      }
      public Builder SetId(int value) {
        result.hasId = true;
        result.id_ = value;
        return this;
      }
      public Builder ClearId() {
        result.hasId = false;
        result.id_ = 0;
        return this;
      }
      
      public bool HasSize {
        get { return result.HasSize; }
      }
      public int Size {
        get { return result.Size; }
        set { SetSize(value); }
      }
      public Builder SetSize(int value) {
        result.hasSize = true;
        result.size_ = value;
        return this;
      }
      public Builder ClearSize() {
        result.hasSize = false;
        result.size_ = 0;
        return this;
      }
      
      public bool HasMessageType {
        get { return result.HasMessageType; }
      }
      public string MessageType {
        get { return result.MessageType; }
        set { SetMessageType(value); }
      }
      public Builder SetMessageType(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        result.hasMessageType = true;
        result.messageType_ = value;
        return this;
      }
      public Builder ClearMessageType() {
        result.hasMessageType = false;
        result.messageType_ = "";
        return this;
      }
    }
    static RubyMessageHeader() {
      object.ReferenceEquals(global::Nohros.Ruby.Proto.RubyMessageHeader.Descriptor, null);
    }
  }
  
  #endregion
  
}