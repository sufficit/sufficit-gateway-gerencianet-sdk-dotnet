using GerencianetSDK.Serializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK.Exceptions
{
    /// <summary>
    /// Gerencianet internal knowing errors
    /// </summary>
    public class APIException : Exception
    {
        public APIException(string message, Exception? inner = default) : base(message, inner) { }

        public static APIException FromJson(string source)
        {
            var converted = JsonSerializerExtensions.Deserialize<APIResponseError>(source);

            string message = converted.ErrorType;
            string description = converted.Description?.Message ?? "internal server error";

            var ex = new APIException(message, new Exception(description))
            {
                HResult = (int)converted.Code
            };
            return ex;
        }

    }
}
