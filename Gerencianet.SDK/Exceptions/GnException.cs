using GerencianetSDK.Serializer;
using System;

namespace GerencianetSDK.Exceptions
{
    public class GnException : Exception
    {
        public GnException(uint code, string error, string message) : base(message)
        {
            Code = code;
            ErrorType = error;
        }

        public uint Code { get; }

        public string ErrorType { get; }

        public static GnException Build(string source, uint statusCode)
        {
            try
            {
                var converted = JsonSerializerExtensions.Deserialize<APIResponseError>(source);
                if (converted != null)
                {
                    string message = converted.ErrorType;
                    return new GnException(converted.Code, converted.ErrorType, converted.Description?.Message ?? "internal server error");
                }
                return new GnException(statusCode, "internal_server_error", source);
            }
            catch (Exception ex)
            {
                if (statusCode == 401)
                    throw new GnException(401, "authorization_error",
                        "Could not authenticate. Please make sure you are using correct credentials and if you are using then in the correct environment.");
                return new GnException(500, "internal_server_error", $"Ocorreu um erro no servidor: {ex.Message} :: " + source);
            }
        }
    }
}