using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

namespace GerencianetSDK.Models
{
    /// <summary>
    /// Route to lookup and method to use 
    /// </summary>
    public interface IEndpoint
    {
        [JsonPropertyName("title")]
        string Title { get; }

        [JsonPropertyName("route")]
        string Route { get; }

        [JsonPropertyName("method")]
        HttpMethod Method { get; }
    }

    public class IEndpoint<T> where T : IEndpoint, new()
    {
        public static T Endpoint => new T();
    }
}
