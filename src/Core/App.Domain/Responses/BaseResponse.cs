using System.Runtime.Serialization;
using MessagePack;

namespace App.Domain.Responses
{
    /// <summary>
    /// Defines the basic api response model
    /// </summary>
    [DataContract]
    [MessagePackObject]
    public partial class BaseResponse
    {
        public BaseResponse()
        {
        }

        public BaseResponse(int resultCode) => ResultCode = resultCode;

        public BaseResponse(int resultCode, string resultReason)
          : this(resultCode)
          => ResultReason = resultReason;

        [DataMember]
        [Key(0)]
        public int ResultCode { get; set; }

        [DataMember]
        [Key(1)]
        public string ResultReason { get; set; }
    }
}
