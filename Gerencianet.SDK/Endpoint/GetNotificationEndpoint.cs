using GerencianetSDK.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK.Endpoint
{
    public class GetNotificationEndpoint : IEndpoint
    {
        public string Title { get; } = "GetNotification";

        public string Route { get; } = "notification/:token";

        public HttpMethod Method { get; } = HttpMethod.Get;
    }
}
