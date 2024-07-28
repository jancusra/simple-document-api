using System;
using System.Runtime.Serialization;
using MessagePack;

namespace App.Contracts
{
    /// <summary>
    /// Basic model representing a DTO object
    /// </summary>
    [DataContract]
    [MessagePackObject]
    public partial class BaseDto
    {
        /// <summary>
        /// unique DTO object identifier
        /// </summary>
        [DataMember]
        [Key(0)]
        public Guid id { get; set; }
    }
}
