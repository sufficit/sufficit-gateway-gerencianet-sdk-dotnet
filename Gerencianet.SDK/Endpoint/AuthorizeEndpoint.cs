using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK.Endpoint
{
    public class AuthorizeEndpoint : APIEndPoint
    {
        public static APIEndPoint Endpoint => new AuthorizeEndpoint();
        public AuthorizeEndpoint() : base("Authorize", "authorize", HttpMethod.Post) { }
    }
}
