using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MessagePack;

namespace App.Contracts.Models
{
    /// <summary>
    /// Represents Document DTO model
    /// </summary>
    [DataContract]
    public partial class DocumentDto : BaseDto
    {
        /// <summary>
        /// array of string tags
        /// </summary>
        [DataMember]
        [Key(1)]
        public string[] tags { get; set; }

        /// <summary>
        /// Additional document data with an arbitrary (schema-free) JSON structure.
        /// Used directly by the JSON formatter and storage.
        /// </summary>
        [JsonPropertyName("data")]
        [IgnoreDataMember]
        [IgnoreMember]
        public JsonObject data { get; set; }

        /// <summary>
        /// JSON-encoded form of <see cref="data"/>. The XML and MessagePack formatters cannot
        /// represent an arbitrary schema natively, so the data is carried as a JSON string there.
        /// </summary>
        [JsonIgnore]
        [DataMember(Name = "data")]
        [Key(2)]
        public string dataSerialized
        {
            get => data?.ToJsonString();
            set => data = string.IsNullOrEmpty(value) ? null : JsonNode.Parse(value)?.AsObject();
        }
    }
}
