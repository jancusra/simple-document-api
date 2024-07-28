using System.Collections.Generic;
using System.Runtime.Serialization;
using MessagePack;

namespace App.Contracts.Models
{
    public partial class DocumentDto : BaseDto
    {
        public DocumentDto()
        {
            data = new Dictionary<string, string>();
        }

        [DataMember]
        [Key(1)]
        public string[] tags { get; set; }

        [DataMember]
        [Key(2)]
        public IDictionary<string, string> data { get; set; }
    }
}