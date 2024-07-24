using System.Runtime.Serialization;

namespace App.Persistence
{
    /// <summary>
    /// Represents storage type enumeration
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumMember(Value = "")]
        Unknown,

        /// <summary>
        /// RAM memory
        /// </summary>
        [EnumMember(Value = "memory")]
        Memory,

        /// <summary>
        /// MS SQL Server
        /// </summary>
        [EnumMember(Value = "sqlserver")]
        SqlServer
    }
}