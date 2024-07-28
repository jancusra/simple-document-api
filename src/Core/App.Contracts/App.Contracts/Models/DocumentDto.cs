using System.Collections.Generic;
using System.Runtime.Serialization;
using MessagePack;

namespace App.Contracts.Models
{
    /// <summary>
    /// Represents Document DTO model
    /// </summary>
    public partial class DocumentDto : BaseDto
    {
        public DocumentDto()
        {
            data = new Dictionary<string, string>();
        }

        /// <summary>
        /// array of string tags
        /// </summary>
        [DataMember]
        [Key(1)]
        public string[] tags { get; set; }

        /// <summary>
        /// additional document data
        /// </summary>
        [DataMember]
        [Key(2)]
        public IDictionary<string, string> data { get; set; }
    }
}