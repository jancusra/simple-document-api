using System;
using System.Net;
using App.Domain.Responses;

namespace App.Domain.Exceptions
{
    /// <summary>
    /// Exception for non existing entity
    /// </summary>
    public partial class NonExistingEntityEntryException : BaseResponseException
    {
        public NonExistingEntityEntryException(string entityName, Guid id)
            : base(new ResponseState { 
                Code = 11003, 
                Message = $"The {entityName} entity with ID: {id.ToString()} does not exist.", 
                HttpStatusCode = (int)HttpStatusCode.NotFound 
            })
        {
        }
    }
}
