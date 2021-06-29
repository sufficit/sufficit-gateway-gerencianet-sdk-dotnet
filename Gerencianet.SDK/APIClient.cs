using GerencianetSDK.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private bool _initialized;
        private string _clientId;
        private string _clientSecret;
        private string _token;

        public APIClient(string id, string secret) : this()
        {
            Initialize(id, secret);
        }

        public APIClient(ILogger logger = default) 
        {
            _logger = logger;
            _http = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(DEFAULTREQUESTTIMEOUT)
            };

            // Old methods
            _helper = new HttpHelper(logger);
            _helper.BaseUrl = APIConstants.URLDEFAULT.Production;
        }

        protected void Initialize(string id, string secret)
        {
            _clientId = id;
            _clientSecret = secret;
            _http.DefaultRequestHeaders.Add("api-sdk", string.Format("dotnet-core-{0}", DEFAULTVERSION));

            _initialized = true;
            _logger?.LogInformation("api client of gerencianet initialized");
        }

        private void UpdateAuthorizationHeaders(string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task Authenticate(CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug("authenticating api client");
            var baseUrl = APIConstants.URLDEFAULT.Production;
            var credentials = string.Format("{0}:{1}", _clientId, _clientSecret);
            var encodedAuth = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(credentials));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ baseUrl }/authorize");
            request.Headers.Add("Authorization", string.Format("Basic {0}", encodedAuth));
            string jsonRequest = "{ \"grant_type\": \"client_credentials\" }";
            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger?.LogDebug($"token request: { jsonResponse }");
                _token = JObject.Parse(jsonResponse)["access_token"].ToString();
                UpdateAuthorizationHeaders(_token);
            }
            else throw new Exception("error on authenticate");
        }

        public async Task<APIResponse> GetChargeAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = new APIResponse();
            if (_initialized)
            {
                if (string.IsNullOrWhiteSpace(_token)) await Authenticate(cancellationToken);
                _logger?.LogDebug($"consulting id: ({ id }) with token: { _token }");

                string baseUrl = APIConstants.URLDEFAULT.Production;
                string endpoint = "/charge";
                string parameters = "/:id";
                parameters = parameters.Replace(":id", id.ToString());

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await _http.GetAsync($"{ baseUrl }{ endpoint }{ parameters }", cancellationToken);                
                result.Code = (int)response.StatusCode;
                result.Message = response.ReasonPhrase;
                
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)                
                    result.Exception = APIException.FromJson(content);
                else                
                    result.Content = content;                
            }
            else
            {
                result.Exception = new Exception("not initialized");
            }
            return result;
        }







        public async Task<object> InvokeAsync(string EndPointTitle, CancellationToken cancellationToken = default, params object[] args)
        {
            APIEndPoint endpoint = APIConstants.ENDPOINTS.Find(s => s.Title.Trim().ToLowerInvariant() == EndPointTitle.Trim().ToLowerInvariant());
            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", EndPointTitle));

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

        private async Task<object> RequestEndpointAsync(string endpoint, HttpMethod method, object query, object body, CancellationToken cancellationToken = default)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string baseUrl = APIConstants.URLDEFAULT.Production;
            HttpRequestMessage request = _helper.GetHttpRequest(endpoint, method, query);
            _logger?.LogDebug($"requesting: { request.RequestUri }");

            request.Headers.Add("Authorization", string.Format("Bearer {0}", _token));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", DEFAULTVERSION));
            return await _helper.SendRequestAsync(request, body, cancellationToken);
        }
    }
}
