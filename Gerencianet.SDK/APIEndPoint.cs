using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK
{
    /// <summary>
    /// Route to lookup and method to use 
    /// </summary>
    public class APIEndPoint
    {
        public APIEndPoint(string title, string route, HttpMethod method)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentNullException("route", "an api endpoint must have an pre defined route");

            Title = title;
            Route = route;
            Method = method;
        }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; }

        [JsonProperty(PropertyName = "route")]
        public string Route { get; }

        [JsonProperty(PropertyName = "method")]
        public HttpMethod Method { get; }
    }
}
