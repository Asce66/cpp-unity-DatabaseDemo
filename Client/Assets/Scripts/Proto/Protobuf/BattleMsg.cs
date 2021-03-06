// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BattleMsg.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from BattleMsg.proto</summary>
public static partial class BattleMsgReflection {

  #region Descriptor
  /// <summary>File descriptor for BattleMsg.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static BattleMsgReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg9CYXR0bGVNc2cucHJvdG8iLAoJTXNnTW92ZXBkEgkKAXgYASABKAUSCQoB",
          "eRgCIAEoBRIJCgF6GAMgASgFIhsKC01zZ0F0dGFja3BkEgwKBGRlc2MYASAB",
          "KAliBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::MsgMovepd), global::MsgMovepd.Parser, new[]{ "X", "Y", "Z" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::MsgAttackpd), global::MsgAttackpd.Parser, new[]{ "Desc" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class MsgMovepd : pb::IMessage<MsgMovepd> {
  private static readonly pb::MessageParser<MsgMovepd> _parser = new pb::MessageParser<MsgMovepd>(() => new MsgMovepd());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<MsgMovepd> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::BattleMsgReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MsgMovepd() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MsgMovepd(MsgMovepd other) : this() {
    x_ = other.x_;
    y_ = other.y_;
    z_ = other.z_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MsgMovepd Clone() {
    return new MsgMovepd(this);
  }

  /// <summary>Field number for the "x" field.</summary>
  public const int XFieldNumber = 1;
  private int x_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int X {
    get { return x_; }
    set {
      x_ = value;
    }
  }

  /// <summary>Field number for the "y" field.</summary>
  public const int YFieldNumber = 2;
  private int y_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Y {
    get { return y_; }
    set {
      y_ = value;
    }
  }

  /// <summary>Field number for the "z" field.</summary>
  public const int ZFieldNumber = 3;
  private int z_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Z {
    get { return z_; }
    set {
      z_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as MsgMovepd);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(MsgMovepd other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (X != other.X) return false;
    if (Y != other.Y) return false;
    if (Z != other.Z) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (X != 0) hash ^= X.GetHashCode();
    if (Y != 0) hash ^= Y.GetHashCode();
    if (Z != 0) hash ^= Z.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (X != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(X);
    }
    if (Y != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Y);
    }
    if (Z != 0) {
      output.WriteRawTag(24);
      output.WriteInt32(Z);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (X != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(X);
    }
    if (Y != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Y);
    }
    if (Z != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Z);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(MsgMovepd other) {
    if (other == null) {
      return;
    }
    if (other.X != 0) {
      X = other.X;
    }
    if (other.Y != 0) {
      Y = other.Y;
    }
    if (other.Z != 0) {
      Z = other.Z;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          X = input.ReadInt32();
          break;
        }
        case 16: {
          Y = input.ReadInt32();
          break;
        }
        case 24: {
          Z = input.ReadInt32();
          break;
        }
      }
    }
  }

}

public sealed partial class MsgAttackpd : pb::IMessage<MsgAttackpd> {
  private static readonly pb::MessageParser<MsgAttackpd> _parser = new pb::MessageParser<MsgAttackpd>(() => new MsgAttackpd());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<MsgAttackpd> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::BattleMsgReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MsgAttackpd() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MsgAttackpd(MsgAttackpd other) : this() {
    desc_ = other.desc_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MsgAttackpd Clone() {
    return new MsgAttackpd(this);
  }

  /// <summary>Field number for the "desc" field.</summary>
  public const int DescFieldNumber = 1;
  private string desc_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Desc {
    get { return desc_; }
    set {
      desc_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as MsgAttackpd);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(MsgAttackpd other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Desc != other.Desc) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Desc.Length != 0) hash ^= Desc.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Desc.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Desc);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Desc.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Desc);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(MsgAttackpd other) {
    if (other == null) {
      return;
    }
    if (other.Desc.Length != 0) {
      Desc = other.Desc;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Desc = input.ReadString();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
