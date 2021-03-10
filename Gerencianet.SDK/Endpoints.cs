using Gerencianet.SDK.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gerencianet.SDK
{
    public class Endpoints : DynamicObject
    {
        private const string ApiBaseURL = "https://api.gerencianet.com.br/v1";
        private const string ApiBaseSandboxURL = "https://sandbox.gerencianet.com.br/v1";
        private const string Version = "1.0.9";

        private JObject endpoints;
        private string clientId;
        private string clientSecret;
        private string accesstoken;
        private HttpHelper httpHelper;
        private string partnerToken;

        public string PartnerToken {
            get { return partnerToken; }
            set { partnerToken = value; }
        }

        public Endpoints(string clientId, string clientSecret, bool sandbox)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            string jsonString = Resources.ResourceManager.GetString("endpoints");
            this.endpoints = JObject.Parse(jsonString);
            this.httpHelper = new HttpHelper();
            this.httpHelper.BaseUrl = sandbox ? Endpoints.ApiBaseSandboxURL : Endpoints.ApiBaseURL;
            this.accesstoken = null;
            this.partnerToken = null;
        }
        
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        { 
            JObject endpoint = null;
            endpoint = (JObject)this.endpoints[binder.Name];
            
            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", binder.Name));

            var route = (string)endpoint["route"];
            var method = (string)endpoint["method"];
            object body = new { };
            object query = new { };

            if (args.Length > 0 && args[0] != null)
                query = args[0];

            if (args.Length > 1 && args[1] != null)
                body = args[1];

            if (accesstoken == null)
                Authenticate();

            try
            {
                result = RequestEndpoint(route, method, query, body);
                return true;
            }
            catch (GnException e)
            {
                if (e.Code == 401)
                {
                    this.Authenticate();
                    result = this.RequestEndpoint(route, method, query, body);
                    return true;
                }
                else throw;
            }
        }

        public async Task<object> InvokeAsync(string EndPointTitle, CancellationToken token = default, params object[] args)
        {
            JObject endpoint = null;
            endpoint = (JObject)this.endpoints[EndPointTitle];

            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", EndPointTitle));

            var route = (string)endpoint["route"];
            var method = (string)endpoint["method"];
            object body = new { };
            object query = new { };

            if (args.Length > 0 && args[0] != null)
                query = args[0];

            if (args.Length > 1 && args[1] != null)
                body = args[1];

            if (this.accesstoken == null)
                await AuthenticateAsync();

            try
            {
                return await RequestEndpointAsync(route, method, query, body, token);
            }
            catch (GnException e)
            {
                if (e.Code == 401)
                {
                    await AuthenticateAsync();
                    return await RequestEndpointAsync(route, method, query, body, token);
                }
                else throw;
            }
        }

        private void Authenticate()
        {
            object body = new
            {
                grant_type = "client_credentials"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest request = this.httpHelper.GetWebRequest("/authorize", "post", null);
            string credentials = string.Format("{0}:{1}", this.clientId, this.clientSecret);
            string encodedAuth = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(credentials));
            request.Headers.Add("Authorization", string.Format("Basic {0}", encodedAuth));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", Endpoints.Version));

            try
            {
                dynamic response = this.httpHelper.SendRequest(request, body);
                this.accesstoken = response.access_token;
            }
            catch (WebException)
            {
                throw GnException.Build("", 401);
            }
        }

        private async Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            object body = new
            {
                grant_type = "client_credentials"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpRequestMessage request = this.httpHelper.GetHttpRequest("/authorize", "post", null);
            string credentials = string.Format("{0}:{1}", this.clientId, this.clientSecret);
            string encodedAuth = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(credentials));
            request.Headers.Add("Authorization", string.Format("Basic {0}", encodedAuth));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", Endpoints.Version));

            try
            {
                dynamic response = await httpHelper.SendRequestAsync(request, body, cancellationToken);
                this.accesstoken = response.access_token;
            }
            catch (WebException)
            {
                throw GnException.Build("", 401);
            }
        }

        private object RequestEndpoint(string endpoint, string method, object query, object body)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest request = this.httpHelper.GetWebRequest(endpoint, method, query);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", this.accesstoken));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", Version));
            if (partnerToken != null)
            {
                request.Headers.Add("partner-token", this.partnerToken);
            }

            try
            {
                return httpHelper.SendRequest(request, body);
            }
            catch (WebException e)
            {
                throw;
                if (e.Response != null && (e.Response is HttpWebResponse))
                {
                    var statusCode = (int)((HttpWebResponse)e.Response).StatusCode;
                    StreamReader reader = new StreamReader(e.Response.GetResponseStream());
                    throw GnException.Build(reader.ReadToEnd(), statusCode);
                }
                else
                {
                    throw GnException.Build("", 500);
                }
            }
        }

        private async Task<object> RequestEndpointAsync(string endpoint, string method, object query, object body, CancellationToken cancellationToken = default)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpRequestMessage request = this.httpHelper.GetHttpRequest(endpoint, method, query);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", this.accesstoken));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", Version));
            if (partnerToken != null)
            {
                request.Headers.Add("partner-token", this.partnerToken);
            }

            try
            {
                return await httpHelper.SendRequestAsync(request, body, cancellationToken);
            }
            catch (WebException e)
            {
                // Executando direto 
                throw;

                if (e.Response != null && (e.Response is HttpWebResponse))
                {
                    var statusCode = (int)((HttpWebResponse)e.Response).StatusCode;
                    StreamReader reader = new StreamReader(e.Response.GetResponseStream());
                    string result = reader.ReadToEnd();
                    if(!string.IsNullOrWhiteSpace(result))
                        throw GnException.Build(result, statusCode);
                    else throw;
                }
                else
                {
                    throw GnException.Build("", 500);
                }
            }
        }
    }
}
