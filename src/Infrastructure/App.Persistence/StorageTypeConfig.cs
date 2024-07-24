using System.Text.Json.Serialization;

namespace App.Persistence
{
    public partial class StorageTypeConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StorageType StorageType { get; set; }
    }
}
