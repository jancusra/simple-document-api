using System;
using System.Runtime.Serialization;
using MessagePack;

namespace App.Contracts
{
    [DataContract]
    [MessagePackObject]
    public partial class BaseDto
    {
        [DataMember]
        [Key(0)]
        public Guid id { get; set; }
    }
}
