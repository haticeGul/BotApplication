using System.Runtime.Serialization;

namespace BotApplication
{
    [DataContract]
    public class ResponseMessage<T> : ResponseMessage
    {
        public ResponseMessage()
            : base()
        {

        }

        public ResponseMessage(ResponseMessage response)
            : base(response)
        {

        }

        [DataMember]
        public T Message { get; set; }

    }

    [DataContract]
    public class ResponseMessage
    {
        public ResponseMessage()
        {
            this.ErrorCode = ErrorCodeType.Success;
            this.ErrorMessage = string.Empty;
        }
        public ResponseMessage(ResponseMessage response)
        {
            this.ErrorCode = response.ErrorCode;
            this.ErrorMessage = response.ErrorMessage;
        }



        [DataMember]
        public ErrorCodeType ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

    }

    [DataContract]
    public enum ErrorCodeType
    {
        [EnumMember]
        Error = 1,

        [EnumMember]
        Success = 0,

        [EnumMember]
        LoginFailedPhoneNotFound = 2,

        [EnumMember]
        InvalidRefId = 3,

        [EnumMember]
        RecordNotFound = 4,

        [EnumMember]
        BKMError = 5,

        [EnumMember]
        ForceUpdate = 100,
    }
}