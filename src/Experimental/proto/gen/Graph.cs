// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: graph.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace org.tensorflow.framework {

  /// <summary>Holder for reflection information generated from graph.proto</summary>
  public static partial class GraphReflection {

    #region Descriptor
    /// <summary>File descriptor for graph.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GraphReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgtncmFwaC5wcm90bxIKdGVuc29yZmxvdxoObm9kZV9kZWYucHJvdG8aDmZ1",
            "bmN0aW9uLnByb3RvGg52ZXJzaW9ucy5wcm90byKdAQoIR3JhcGhEZWYSIQoE",
            "bm9kZRgBIAMoCzITLnRlbnNvcmZsb3cuTm9kZURlZhIoCgh2ZXJzaW9ucxgE",
            "IAEoCzIWLnRlbnNvcmZsb3cuVmVyc2lvbkRlZhITCgd2ZXJzaW9uGAMgASgF",
            "QgIYARIvCgdsaWJyYXJ5GAIgASgLMh4udGVuc29yZmxvdy5GdW5jdGlvbkRl",
            "ZkxpYnJhcnlChgEKGG9yZy50ZW5zb3JmbG93LmZyYW1ld29ya0ILR3JhcGhQ",
            "cm90b3NQAVo9Z2l0aHViLmNvbS90ZW5zb3JmbG93L3RlbnNvcmZsb3cvdGVu",
            "c29yZmxvdy9nby9jb3JlL2ZyYW1ld29ya/gBAaoCGG9yZy50ZW5zb3JmbG93",
            "LmZyYW1ld29ya2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::org.tensorflow.framework.NodeDefReflection.Descriptor, global::org.tensorflow.framework.FunctionReflection.Descriptor, global::org.tensorflow.framework.VersionsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::org.tensorflow.framework.GraphDef), global::org.tensorflow.framework.GraphDef.Parser, new[]{ "Node", "Versions", "Version", "Library" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// Represents the graph of operations
  /// </summary>
  public sealed partial class GraphDef : pb::IMessage<GraphDef> {
    private static readonly pb::MessageParser<GraphDef> _parser = new pb::MessageParser<GraphDef>(() => new GraphDef());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GraphDef> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::org.tensorflow.framework.GraphReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GraphDef() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GraphDef(GraphDef other) : this() {
      node_ = other.node_.Clone();
      versions_ = other.versions_ != null ? other.versions_.Clone() : null;
      version_ = other.version_;
      library_ = other.library_ != null ? other.library_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GraphDef Clone() {
      return new GraphDef(this);
    }

    /// <summary>Field number for the "node" field.</summary>
    public const int NodeFieldNumber = 1;
    private static readonly pb::FieldCodec<global::org.tensorflow.framework.NodeDef> _repeated_node_codec
        = pb::FieldCodec.ForMessage(10, global::org.tensorflow.framework.NodeDef.Parser);
    private readonly pbc::RepeatedField<global::org.tensorflow.framework.NodeDef> node_ = new pbc::RepeatedField<global::org.tensorflow.framework.NodeDef>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::org.tensorflow.framework.NodeDef> Node {
      get { return node_; }
    }

    /// <summary>Field number for the "versions" field.</summary>
    public const int VersionsFieldNumber = 4;
    private global::org.tensorflow.framework.VersionDef versions_;
    /// <summary>
    /// Compatibility versions of the graph.  See core/public/version.h for version
    /// history.  The GraphDef version is distinct from the TensorFlow version, and
    /// each release of TensorFlow will support a range of GraphDef versions.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::org.tensorflow.framework.VersionDef Versions {
      get { return versions_; }
      set {
        versions_ = value;
      }
    }

    /// <summary>Field number for the "version" field.</summary>
    public const int VersionFieldNumber = 3;
    private int version_;
    /// <summary>
    /// Deprecated single version field; use versions above instead.  Since all
    /// GraphDef changes before "versions" was introduced were forward
    /// compatible, this field is entirely ignored.
    /// </summary>
    [global::System.ObsoleteAttribute]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Version {
      get { return version_; }
      set {
        version_ = value;
      }
    }

    /// <summary>Field number for the "library" field.</summary>
    public const int LibraryFieldNumber = 2;
    private global::org.tensorflow.framework.FunctionDefLibrary library_;
    /// <summary>
    /// EXPERIMENTAL. DO NOT USE OR DEPEND ON THIS YET.
    ///
    /// "library" provides user-defined functions.
    ///
    /// Naming:
    ///   * library.function.name are in a flat namespace.
    ///     NOTE: We may need to change it to be hierarchical to support
    ///     different orgs. E.g.,
    ///     { "/google/nn", { ... }},
    ///     { "/google/vision", { ... }}
    ///     { "/org_foo/module_bar", { ... }}
    ///     map&lt;string, FunctionDefLib> named_lib;
    ///   * If node[i].op is the name of one function in "library",
    ///     node[i] is deemed as a function call. Otherwise, node[i].op
    ///     must be a primitive operation supported by the runtime.
    ///
    /// Function call semantics:
    ///
    ///   * The callee may start execution as soon as some of its inputs
    ///     are ready. The caller may want to use Tuple() mechanism to
    ///     ensure all inputs are ready in the same time.
    ///
    ///   * The consumer of return values may start executing as soon as
    ///     the return values the consumer depends on are ready.  The
    ///     consumer may want to use Tuple() mechanism to ensure the
    ///     consumer does not start until all return values of the callee
    ///     function are ready.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::org.tensorflow.framework.FunctionDefLibrary Library {
      get { return library_; }
      set {
        library_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as GraphDef);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(GraphDef other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!node_.Equals(other.node_)) return false;
      if (!object.Equals(Versions, other.Versions)) return false;
      if (Version != other.Version) return false;
      if (!object.Equals(Library, other.Library)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= node_.GetHashCode();
      if (versions_ != null) hash ^= Versions.GetHashCode();
      if (Version != 0) hash ^= Version.GetHashCode();
      if (library_ != null) hash ^= Library.GetHashCode();
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
      node_.WriteTo(output, _repeated_node_codec);
      if (library_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Library);
      }
      if (Version != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Version);
      }
      if (versions_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Versions);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += node_.CalculateSize(_repeated_node_codec);
      if (versions_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Versions);
      }
      if (Version != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Version);
      }
      if (library_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Library);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(GraphDef other) {
      if (other == null) {
        return;
      }
      node_.Add(other.node_);
      if (other.versions_ != null) {
        if (versions_ == null) {
          versions_ = new global::org.tensorflow.framework.VersionDef();
        }
        Versions.MergeFrom(other.Versions);
      }
      if (other.Version != 0) {
        Version = other.Version;
      }
      if (other.library_ != null) {
        if (library_ == null) {
          library_ = new global::org.tensorflow.framework.FunctionDefLibrary();
        }
        Library.MergeFrom(other.Library);
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
            node_.AddEntriesFrom(input, _repeated_node_codec);
            break;
          }
          case 18: {
            if (library_ == null) {
              library_ = new global::org.tensorflow.framework.FunctionDefLibrary();
            }
            input.ReadMessage(library_);
            break;
          }
          case 24: {
            Version = input.ReadInt32();
            break;
          }
          case 34: {
            if (versions_ == null) {
              versions_ = new global::org.tensorflow.framework.VersionDef();
            }
            input.ReadMessage(versions_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
