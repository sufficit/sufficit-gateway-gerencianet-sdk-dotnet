﻿using GerencianetSDK.Exceptions;
using GerencianetSDK.Serializer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GerencianetSDK
{
    public class HttpHelper
    {
        private readonly HttpClient client;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        public HttpHelper(JsonSerializerOptions serializerOptions, HttpClient httpClient, ILogger? logger = default)
        {
            _serializerOptions = serializerOptions;
            client = httpClient;
            _logger = logger!;
        }

        /// <summary>
        /// Monta a solicitação
        /// </summary>
        [Obsolete]
        public WebRequest GetWebRequest(string endpoint, string method, object query)
        {
            if (query != null)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                var queryDict = new Dictionary<string, object?>();
                foreach (var property in query.GetType().GetProperties(attr))
                {
                    if (property.CanRead)
                        queryDict.Add(property.Name, property.GetValue(query, null));
                }

                MatchCollection matchCollection = Regex.Matches(endpoint, ":([a-zA-Z0-9]+)");
                for (int i = 0; i < matchCollection.Count; i++)
                {
                    string resource = matchCollection[i].Groups[1].Value;
                    try
                    {
                        var value = queryDict[resource]?.ToString() ?? string.Empty;
                        endpoint = Regex.Replace(endpoint, string.Format(":{0}", resource), value);
                        queryDict.Remove(resource);
                    }
                    catch (Exception)
                    {}
                }

                string queryString = "";
                foreach (KeyValuePair<string, object?> pair in queryDict)
                {
                    if (queryString.Equals(""))
                        queryString = "?";
                    else
                        queryString += "&";
                    queryString += string.Format("{0}={1}", pair.Key, pair.Value?.ToString());
                }
                endpoint += queryString;
            }
                        
            var request = HttpWebRequest.Create(string.Format("{0}{1}", client.BaseAddress, endpoint));
            request.Method = method;
            request.ContentType = "application/json";

            return request;
        }

        public HttpRequestMessage GetHttpRequest(string endpoint, HttpMethod method, object query)
        {
            if (query != null)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                var queryDict = new Dictionary<string, object?>();
                foreach (var property in query.GetType().GetProperties(attr))
                {
                    if (property.CanRead)
                        queryDict.Add(property.Name, property.GetValue(query, null));
                }

                MatchCollection matchCollection = Regex.Matches(endpoint, ":([a-zA-Z0-9]+)");
                for (int i = 0; i < matchCollection.Count; i++)
                {
                    string resource = matchCollection[i].Groups[1].Value;
                    try
                    {
                        var value = queryDict[resource]?.ToString() ?? string.Empty;
                        endpoint = Regex.Replace(endpoint, string.Format(":{0}", resource), value);
                        queryDict.Remove(resource);
                    }
                    catch (Exception)
                    { }
                }

                string queryString = "";
                foreach (KeyValuePair<string, object?> pair in queryDict)
                {
                    if (queryString.Equals(""))
                        queryString = "?";
                    else
                        queryString += "&";
                    queryString += string.Format("{0}={1}", pair.Key, pair.Value?.ToString());
                }
                endpoint += queryString;
            }

            var request = new HttpRequestMessage();
            _logger.LogDebug($"creating uri: { string.Format("{0}{1}", client.BaseAddress, endpoint) }");
            request.RequestUri = new Uri(string.Format("{0}{1}", client.BaseAddress, endpoint));
            request.Method = method;
            request.Headers.Add("ContentType", "application/json");
            return request;
        }

        public async Task<JsonElement> SendRequestAsync(HttpRequestMessage request, object body, CancellationToken cancellationToken)
        {
            if (!request.Method.Equals(HttpMethod.Get) && body != null)
            {                
                var data = JsonSerializer.SerializeToElement(body, _serializerOptions).GetString();
                request.Content = new StringContent(data ?? string.Empty, Encoding.UTF8, "application/json");                
            }

            HttpResponseMessage response;

            // Depuração de tempo de execução
            Stopwatch sw = new Stopwatch(); sw.Start();
            
            try
            {   
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                throw new HttpClientTimeOutException(request, client.Timeout, cancellationToken, sw.ElapsedMilliseconds);
            }

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw GnException.Build(json, (uint)response.StatusCode);
            }
            else
            {
                return JsonSerializer.Deserialize<JsonElement>(json, _serializerOptions);
            }
        }        
    }
}