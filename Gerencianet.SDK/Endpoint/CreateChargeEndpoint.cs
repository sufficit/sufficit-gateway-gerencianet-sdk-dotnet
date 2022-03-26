using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK.Endpoint
{
    public class CreateChargeEndpoint : APIEndPoint
    {
        public static CreateChargeEndpoint Endpoint => new CreateChargeEndpoint();
        public CreateChargeEndpoint() : base("CreateCharge", "charge", HttpMethod.Post) { }
    }
}
