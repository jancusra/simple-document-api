using System.Text.Json.Serialization;

namespace App.Persistence
{
    /// <summary>
    /// Represents a configuration related to the storage type
    /// </summary>
    public partial class StorageTypeConfig
    {
        /// <summary>
        /// Configured storage type
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StorageType StorageType { get; set; }
    }
}
