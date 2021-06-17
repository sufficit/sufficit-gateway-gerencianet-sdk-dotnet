using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        #region CONSTRUTORS

        public Endpoints(APIAuthPair authpair, bool sandbox)
        {
            this.clientId = authpair.id;
            this.clientSecret = authpair.secret;
            this.httpHelper = new HttpHelper();
            this.httpHelper.BaseUrl = sandbox ? Endpoints.ApiBaseSandboxURL : Endpoints.ApiBaseURL;
            this.accesstoken = null;
            this.partnerToken = null;
        }

        public Endpoints(string clientId, string clientSecret, bool sandbox) : this(new APIAuthPair() { id = clientId, secret = clientSecret }, sandbox) { }

        static Endpoints()
        {
            endpoints = new List<APIEndPoint>(); // static to avoid refill

            // filling available endpoints
            foreach (APIEndPoint ep in GenerateEndPoints())
                endpoints.Add(ep);
        }

        #endregion

        private const string ApiBaseURL = "https://api.gerencianet.com.br/v1";
        private const string ApiBaseSandboxURL = "https://sandbox.gerencianet.com.br/v1";
        private const string Version = "1.0.9";

        private static List<APIEndPoint> endpoints;
        private string clientId;
        private string clientSecret;
        private string accesstoken;
        private HttpHelper httpHelper;
        private string partnerToken;

        public string PartnerToken {
            get { return partnerToken; }
            set { partnerToken = value; }
        }
        
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        { 
            APIEndPoint endpoint = endpoints.Find(s => s.Title.Trim().ToLowerInvariant() == binder.Name.Trim().ToLowerInvariant());            
            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", binder.Name));

            object body = new { };
            object query = new { };

            if (args.Length > 0 && args[0] != null)
                query = args[0];

            if (args.Length > 1 && args[1] != null)
                body = args[1];

            if (this.accesstoken == null)
                Authenticate();

            try
            {
                result = RequestEndpoint(endpoint.Route, endpoint.Method.ToString(), query, body);
                return true;
            }
            catch (GnException e)
            {
                if (e.Code == 401)
                {
                    this.Authenticate();
                    result = this.RequestEndpoint(endpoint.Route, endpoint.Method.ToString(), query, body);
                    return true;
                }
                else throw;
            }
        }

        public async Task<object> InvokeAsync(string EndPointTitle, CancellationToken token = default, params object[] args)
        {
            APIEndPoint endpoint = endpoints.Find(s => s.Title.Trim().ToLowerInvariant() == EndPointTitle.Trim().ToLowerInvariant());
            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", EndPointTitle));

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
                return await RequestEndpointAsync(endpoint.Route, endpoint.Method, query, body, token);
            }
            catch (GnException e)
            {
                // Caso a resposta seja de authenticação inválida
                // Pode ser que o token tenha vencido
                // Portando renovamos a chave de acesso e tentamos a requisição novamente
                if (e.Code == 401)
                {
                    await AuthenticateAsync();
                    return await RequestEndpointAsync(endpoint.Route, endpoint.Method, query, body, token);
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
            catch
            {
                // Zerando a chave de acesso para facilitar nas novas requisições
                this.accesstoken = null;
                                
                throw;
            }
        }

        private async Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            APIEndPoint endpoint = endpoints.Find(s => s.Title.Trim().ToLowerInvariant() == "authorize");
            if (endpoint == null)
                throw new GnException(0, "invalid_endpoint", string.Format("Método '{0}' inexistente", "authorize"));

            object body = new
            {
                grant_type = "client_credentials"
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpRequestMessage request = this.httpHelper.GetHttpRequest(endpoint.Route, endpoint.Method, null);
            string credentials = string.Format("{0}:{1}", this.clientId, this.clientSecret);
            string encodedAuth = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(credentials));
            request.Headers.Add("Authorization", string.Format("Basic {0}", encodedAuth));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", Endpoints.Version));

            try
            {
                dynamic response = await httpHelper.SendRequestAsync(request, body, cancellationToken);
                this.accesstoken = response.access_token;
            }
            catch {

                // Zerando a chave de acesso para facilitar nas novas requisições
                this.accesstoken = null;

                throw;
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

        private async Task<object> RequestEndpointAsync(string endpoint, HttpMethod method, object query, object body, CancellationToken cancellationToken = default)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpRequestMessage request = this.httpHelper.GetHttpRequest(endpoint, method, query);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", this.accesstoken));
            request.Headers.Add("api-sdk", string.Format("dotnet-{0}", Version));
            if (partnerToken != null)
            {
                request.Headers.Add("partner-token", this.partnerToken);
            }

            return await httpHelper.SendRequestAsync(request, body, cancellationToken);           
        }

        /// <summary>
        /// Create all available endpoints
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<APIEndPoint> GenerateEndPoints()
        {
            yield return new APIEndPoint("Authorize",                   "/authorize", HttpMethod.Post);
            yield return new APIEndPoint("CreateCharge",                "/charge", HttpMethod.Post);
            yield return new APIEndPoint("DetailCharge",                "/charge/:id", HttpMethod.Get);
            yield return new APIEndPoint("UpdateChargeMetadata",        "/charge/:id/metadata", HttpMethod.Put);
            yield return new APIEndPoint("UpdateBillet",                "/charge/:id/billet", HttpMethod.Put);
            yield return new APIEndPoint("PayCharge",                   "/charge/:id/pay", HttpMethod.Post);
            yield return new APIEndPoint("CancelCharge",                "/charge/:id/cancel", HttpMethod.Put);
            yield return new APIEndPoint("CreateCarnet",                "/carnet", HttpMethod.Post);
            yield return new APIEndPoint("DetailCarnet",                "/carnet/:id", HttpMethod.Get);
            yield return new APIEndPoint("UpdateParcel",                "/carnet/:id/parcel/:parcel", HttpMethod.Put);
            yield return new APIEndPoint("UpdateCarnetMetadata",        "/carnet/:id/metadata", HttpMethod.Put);
            yield return new APIEndPoint("GetNotification",             "/notification/:token", HttpMethod.Get);
            yield return new APIEndPoint("GetPlans",                    "/plans", HttpMethod.Get);
            yield return new APIEndPoint("CreatePlan",                  "/plan", HttpMethod.Post);
            yield return new APIEndPoint("DeletePlan",                  "/plan/:id", HttpMethod.Delete);
            yield return new APIEndPoint("CreateSubscription",          "/plan/:id/subscription", HttpMethod.Post);
            yield return new APIEndPoint("DetailSubscription",          "/subscription/:id", HttpMethod.Get);
            yield return new APIEndPoint("PaySubscription",             "/subscription/:id/pay", HttpMethod.Post);
            yield return new APIEndPoint("CancelSubscription",          "/subscription/:id/cancel", HttpMethod.Put);
            yield return new APIEndPoint("UpdateSubscriptionMetadata",  "/subscription/:id/metadata", HttpMethod.Put);
            yield return new APIEndPoint("GetInstallments",             "/installments", HttpMethod.Get);
            yield return new APIEndPoint("ResendBillet",                "/charge/:id/billet/resend", HttpMethod.Post);
            yield return new APIEndPoint("CreateChargeHistory",         "/charge/:id/history", HttpMethod.Post);
            yield return new APIEndPoint("ResendCarnet",                "/carnet/:id/resend", HttpMethod.Post);
            yield return new APIEndPoint("ResendParcel",                "/carnet/:id/parcel/:parcel/resend", HttpMethod.Post);
            yield return new APIEndPoint("CreateCarnetHistory",         "/carnet/:id/history", HttpMethod.Post);
            yield return new APIEndPoint("CancelCarnet",                "/carnet/:id/cancel", HttpMethod.Put);
            yield return new APIEndPoint("CancelParcel",                "/carnet/:id/parcel/:parcel/cancel", HttpMethod.Put);
            yield return new APIEndPoint("LinkCharge",                  "/charge/:id/link", HttpMethod.Post);
            yield return new APIEndPoint("ChargeLink",                  "/charge/:id/link", HttpMethod.Post);
            yield return new APIEndPoint("UpdateChargeLink",            "/charge/:id/link", HttpMethod.Put);
            yield return new APIEndPoint("UpdatePlan",                  "/plan/:id", HttpMethod.Put);
            yield return new APIEndPoint("CreateSubscriptionHistory",   "/subscription/:id/history", HttpMethod.Post);
            yield return new APIEndPoint("SettleCharge",                "/charge/:id/settle", HttpMethod.Put);
            yield return new APIEndPoint("SettleCarnetParcel",          "/carnet/:id/parcel/:parcel/settle", HttpMethod.Put);
            yield return new APIEndPoint("OneStep",                     "/charge/one-step", HttpMethod.Post);
        }
    }
}
