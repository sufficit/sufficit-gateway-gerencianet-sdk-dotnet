using GerencianetSDK.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK
{
    /// <summary>
    /// Route to lookup and method to use 
    /// </summary>
    public class APIEndPoint : IEndpoint
    {
        public APIEndPoint() { }
        public APIEndPoint(string title, string route, HttpMethod method)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentNullException("route", "an api endpoint must have an pre defined route");

            Title = title;
            Route = route;
            Method = method;
        }

        [JsonPropertyName("title")]
        public virtual string Title { get; }

        [JsonPropertyName("route")]
        public virtual string Route { get; }

        [JsonPropertyName("method")]
        public virtual HttpMethod Method { get; }

        public virtual Uri GetRelativeUri() { return new Uri(Route.TrimStart('/'), UriKind.Relative); }
    }

    public class APIEndPoint<T> where T : APIEndPoint, new()
    {
        public static T Endpoint => new T();
    }
}
