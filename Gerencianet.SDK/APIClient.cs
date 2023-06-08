using GerencianetSDK.Exceptions;
using GerencianetSDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using GerencianetSDK.Serializer;
using GerencianetSDK.Endpoint;
using System.Text.Json.Nodes;

namespace GerencianetSDK
{
    public class APIClient
    {
        /// <summary>
        /// Tempo padrão para expirar cada requisição individual ao serviço gerencianet <br />
        /// Mili Segundos ex: (3000)ms
        /// </summary>
        public const int DEFAULTREQUESTTIMEOUT = 3000;
        public const string DEFAULTVERSION = "2.0.1";

        protected readonly ILogger _logger;
        protected readonly HttpClient _http;
        protected readonly HttpHelper _helper;
        protected readonly JsonSerializerOptions _serializerOptions;

        private bool _initialized;
        private string? _clientId;
        private string? _clientSecret;
        private string? _token;

        public APIClient(string id, string secret, ILogger logger) : this(logger)
        {
            Initialize(id, secret);
        }

        public APIClient(ILogger logger) 
        {
            _logger = logger;
            _serializerOptions = JsonSerializerExtensions.Options;

            _http = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(DEFAULTREQUESTTIMEOUT),
                BaseAddress = new Uri(APIConstants.URLDEFAULT.Production, UriKind.Absolute)
            };

            // Old methods
            _helper = new HttpHelper(_serializerOptions, _http, logger);
        }

        protected void Initialize(string id, string secret)
        {
            _clientId = id;
            _clientSecret = secret;
            _http.DefaultRequestHeaders.Add("api-sdk", string.Format("dotnet-core-{0}", DEFAULTVERSION));

            _initialized = true;
            _logger?.LogDebug("api client of gerencianet initialized");
        }

        private void UpdateAuthorizationHeaders(string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task Authenticate(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_clientSecret))
                throw new Exception("api client not initialized");

            _token = await GetToken(_clientId!, _clientSecret!, cancellationToken);
            UpdateAuthorizationHeaders(_token);
        }

        public async Task<string> GetToken(string id, string secret, CancellationToken cancellationToken = default)
        {
            var credentials = string.Format("{0}:{1}", id, secret);
            var bytesCredentials = Encoding.UTF8.GetBytes(credentials);
            var encodedAuth = Convert.ToBase64String(bytesCredentials);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var requestUri = AuthorizeEndpoint.Endpoint.GetRelativeUri();
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            _logger?.LogDebug($"authenticating api client, { request.RequestUri }");

            request.Headers.Add("Authorization", string.Format("Basic {0}", encodedAuth));
            string jsonRequest = "{ \"grant_type\": \"client_credentials\" }";
            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger?.LogDebug($"token request: { jsonResponse }");
                return JsonSerializer.Deserialize<JsonElement>(jsonResponse).GetProperty("access_token").ToString();                
            }
            else throw new Exception("error on authenticate");
        }

        public async Task<APIResponse> UpdateChargeMetadataAsync(uint chargeId, ChargeMetadata metadata, CancellationToken cancellationToken = default)
        {
            var param = new { id = chargeId };
            try
            {
                var body = JsonSerializer.Serialize(metadata);
                var response = await InvokeAsync<UpdateChargeMetadataEndpoint>(cancellationToken, param, body);
                return response.Deserialize<APIResponse>(_serializerOptions)!;
            }
            catch (GnException ex)
            {
                return new GetNotificationResponse() { Exception = ex.Message, Code = ex.Code };
            }
        }

        public async Task<APIResponse> GetChargeAsync(uint chargeId, CancellationToken cancellationToken = default)
        {
            var param = new { id = chargeId };
            try
            {
                var response = await InvokeAsync<DetailChargeEndpoint>(cancellationToken, param);
                return response.Deserialize<APIResponse>(_serializerOptions)!;
            }
            catch (GnException ex)
            {
                return new GetNotificationResponse() { Exception = ex.Message, Code = ex.Code };
            }
        }

        /// <summary>
        /// Método para consultar informações acerca de uma notificação
        /// </summary>
        public async Task<GetNotificationResponse> GetNotificationAsync(string notificationToken, CancellationToken cancellationToken = default)
        {
            var param = new { token = notificationToken }; 
            try
            {
                var response = await InvokeAsync<GetNotificationEndpoint>(cancellationToken, param);                            
                return response.Deserialize<GetNotificationResponse>(_serializerOptions)!;
            } catch (GnException ex)
            {  
                return new GetNotificationResponse() { Exception = ex.Message, Code = ex.Code };
            }            
        }

        public async Task<JsonElement> InvokeAsync<T>(CancellationToken cancellationToken = default, params object[] args) where T : IEndpoint, new()
        {
            return await InvokeAsync(new T(), cancellationToken, args);
        }


        public async Task<JsonElement> InvokeAsync(string EndPointTitle, CancellationToken cancellationToken = default, params object[] args)
        {
            IEndpoint endpoint = APIConstants.ENDPOINTS.Find(s => s.Title.Trim().ToLowerInvariant() == EndPointTitle.Trim().ToLowerInvariant());
            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", EndPointTitle));

            return await InvokeAsync(endpoint, cancellationToken, args);
        }


        public async Task<JsonElement> InvokeAsync(IEndpoint endpoint, CancellationToken cancellationToken = default, params object[] args)
        {
            if (!_initialized) 
                throw new Exception("not initialized");           

            object body = new { };
            object query = new { };

            if (args.Length > 0 && args[0] != null)
                query = args[0];

            if (args.Length > 1 && args[1] != null)
                body = args[1];

            if (string.IsNullOrWhiteSpace(_token))
                await Authenticate(cancellationToken);

            try
            {
                return await RequestEndpointAsync(endpoint.Route, endpoint.Method, query, body, cancellationToken);
            }
            catch (GnException e)
            {
                // Caso a resposta seja de authenticação inválida
                // Pode ser que o token tenha vencido
                // Portando renovamos a chave de acesso e tentamos a requisição novamente
                if (e.Code == 401)
                {
                    await Authenticate(cancellationToken);
                    return await RequestEndpointAsync(endpoint.Route, endpoint.Method, query, body, cancellationToken);
                }
                else throw;
            }
        }

        private async Task<JsonElement> RequestEndpointAsync(string endpoint, HttpMethod method, object query, object body, CancellationToken cancellationToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpRequestMessage request = _helper.GetHttpRequest(endpoint, method, query);
            _logger?.LogDebug($"requesting: { request.RequestUri }");

            request.Headers.Add("Authorization", string.Format("Bearer {0}", _token));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", DEFAULTVERSION));
            return await _helper.SendRequestAsync(request, body, cancellationToken);
        }
    }
}
