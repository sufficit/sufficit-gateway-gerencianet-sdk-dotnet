using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Gerencianet.SDK
{
    public class HttpHelper
    {
        private HttpClient client;
        private string baseUrl;

        public HttpHelper()
        {
            client = new HttpClient();
        }

        public string BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        /// <summary>
        /// Monta a solicitação
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="method"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public WebRequest GetWebRequest(string endpoint, string method, object query)
        {
            if (query != null)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                var queryDict = new Dictionary<string, object>();
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
                        var value = queryDict[resource].ToString();
                        endpoint = Regex.Replace(endpoint, string.Format(":{0}", resource), value);
                        queryDict.Remove(resource);
                    }
                    catch (Exception)
                    {}
                }

                string queryString = "";
                foreach (KeyValuePair<String, object> pair in queryDict)
                {
                    if (queryString.Equals(""))
                        queryString = "?";
                    else
                        queryString += "&";
                    queryString += string.Format("{0}={1}", pair.Key, pair.Value.ToString());
                }
                endpoint += queryString;
            }

            WebRequest request = HttpWebRequest.Create(string.Format("{0}{1}", baseUrl, endpoint));
            request.Method = method;
            request.ContentType = "application/json";

            return request;
        }

        public HttpRequestMessage GetHttpRequest(string endpoint, string method, object query)
        {
            if (query != null)
            {
                var attr = BindingFlags.Public | BindingFlags.Instance;
                var queryDict = new Dictionary<string, object>();
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
                        var value = queryDict[resource].ToString();
                        endpoint = Regex.Replace(endpoint, string.Format(":{0}", resource), value);
                        queryDict.Remove(resource);
                    }
                    catch (Exception)
                    { }
                }

                string queryString = "";
                foreach (KeyValuePair<String, object> pair in queryDict)
                {
                    if (queryString.Equals(""))
                        queryString = "?";
                    else
                        queryString += "&";
                    queryString += string.Format("{0}={1}", pair.Key, pair.Value.ToString());
                }
                endpoint += queryString;
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(string.Format("{0}{1}", baseUrl, endpoint));
            request.Method = new HttpMethod(method);
            request.Headers.Add("ContentType", "application/json");
            return request;
        }

        public dynamic SendRequest(WebRequest request, object body)
        {
            if (!request.Method.Equals("GET") && body != null)
            {
                using (Stream postStream = request.GetRequestStream())
                {
                    var data = Encoding.UTF8.GetBytes(JObject.FromObject(body).ToString());
                    postStream.Write(data, 0, data.Length);
                }
            }

            using (WebResponse response = request.GetResponse())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                object def = new { };
                return JsonConvert.DeserializeAnonymousType(reader.ReadToEnd(), def);
            }
        }

        public async Task<object> SendRequestAsync(HttpRequestMessage request, object body, CancellationToken token = default)
        {
            if (!request.Method.Equals(HttpMethod.Get) && body != null)
            {
                var data = JObject.FromObject(body).ToString();
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");                
            }

            HttpResponseMessage response = await client.SendAsync(request, token);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(json);
        }
    }
}
