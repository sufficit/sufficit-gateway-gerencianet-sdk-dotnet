using GerencianetSDK.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK.Endpoint
{
    public class DetailChargeEndpoint : IEndpoint
    {
        public string Title { get; } = "DetailCharge";

        public string Route { get; } = "charge/:id";

        public HttpMethod Method { get; } = HttpMethod.Get;
    }
}
