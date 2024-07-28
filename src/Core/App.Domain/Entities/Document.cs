namespace App.Domain.Entities
{
    /// <summary>
    /// Represents Document entity
    /// </summary>
    public partial class Document : BaseEntity
    {
        /// <summary>
        /// Document object (serialized in JSON format)
        /// </summary>
        public string Value { get; set; }
    }
}
