using System.Net;
using App.Domain.Responses;

namespace App.Domain.Exceptions
{
    /// <summary>
    /// Exception for an invalid entity (a mandatory field is missing)
    /// </summary>
    public partial class InvalidEntityEntryException : BaseResponseException
    {
        public InvalidEntityEntryException(string entityName, string reason)
            : base(new ResponseState
            {
                Code = 11001,
                Message = $"The {entityName} entity is invalid: {reason}",
                HttpStatusCode = (int)HttpStatusCode.BadRequest
            })
        {
        }
    }
}
