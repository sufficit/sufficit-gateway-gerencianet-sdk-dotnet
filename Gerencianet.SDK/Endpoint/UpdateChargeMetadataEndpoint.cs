using GerencianetSDK.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK.Endpoint
{
    public class UpdateChargeMetadataEndpoint : IEndpoint
    {
        public string Title { get; } = "UpdateChargeMetadata";

        public string Route { get; } = "charge/:id/metadata";

        public HttpMethod Method { get; } = HttpMethod.Put;
    }
}
