using System;
using System.Net;
using App.Domain.Responses;

namespace App.Domain.Exceptions
{
    /// <summary>
    /// Exception for non existing entity
    /// </summary>
    public partial class EntityEntryAlreadyExistsException : BaseResponseException
    {
        public EntityEntryAlreadyExistsException(string entityName, Guid id)
            : base(new ResponseState { 
                Code = 11002,
                Message = $"The {entityName} entity with ID: {id.ToString()} already exists.",
                HttpStatusCode = (int)HttpStatusCode.Conflict 
            })
        {
        }
    }
}
