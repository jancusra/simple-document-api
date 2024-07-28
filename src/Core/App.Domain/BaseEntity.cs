using System;

namespace App.Domain
{
    /// <summary>
    /// Basic model representing an entity
    /// </summary>
    public partial class BaseEntity
    {
        /// <summary>
        /// unique entity identifier
        /// </summary>
        public Guid Id { get; set; }
    }
}
