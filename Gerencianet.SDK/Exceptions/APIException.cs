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
        public APIException(string message, Exception inner = default) : base(message, inner) { }

        public static APIException FromJson(string source)
        {
            object def = new { };
            dynamic jsonObject = JsonSerializerExtensions.DeserializeAnonymousType(source, def);

            string message = jsonObject.error.ToString();
            string description = jsonObject.error_description.ToString();

            var ex = new APIException(message, new Exception(description))
            {
                HResult = jsonObject.code
            };
            return ex;
        }

    }
}
