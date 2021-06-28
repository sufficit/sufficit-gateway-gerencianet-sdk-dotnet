using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GerencianetSDK
{
    public class APIConstants
    {
        public const string DATEFORMAT = "ddMMyy";
        public static APIUrl URLDEFAULT = new APIUrl
        {
            Production = "https://api.gerencianet.com.br/v1",
            Development = "https://sandbox.gerencianet.com.br/v1"
        };

        public static APIUrl URLPIX = new APIUrl
        {
            Production = "https://api-pix.gerencianet.com.br",
            Development = "https://api-pix-h.gerencianet.com.br"
        };

        public static List<APIEndPoint> ENDPOINTS { get; }

        static APIConstants()
        {
            ENDPOINTS = new List<APIEndPoint>(); // static to avoid refill

            // filling available endpoints
            foreach (APIEndPoint ep in GenerateEndPoints())
                ENDPOINTS.Add(ep);          
        }

        /// <summary>
        /// Create all available endpoints
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<APIEndPoint> GenerateEndPoints()
        {
            yield return new APIEndPoint("Authorize", "/authorize", HttpMethod.Post);
            yield return new APIEndPoint("CreateCharge", "/charge", HttpMethod.Post);
            yield return new APIEndPoint("DetailCharge", "/charge/:id", HttpMethod.Get);
            yield return new APIEndPoint("UpdateChargeMetadata", "/charge/:id/metadata", HttpMethod.Put);
            yield return new APIEndPoint("UpdateBillet", "/charge/:id/billet", HttpMethod.Put);
            yield return new APIEndPoint("PayCharge", "/charge/:id/pay", HttpMethod.Post);
            yield return new APIEndPoint("CancelCharge", "/charge/:id/cancel", HttpMethod.Put);
            yield return new APIEndPoint("CreateCarnet", "/carnet", HttpMethod.Post);
            yield return new APIEndPoint("DetailCarnet", "/carnet/:id", HttpMethod.Get);
            yield return new APIEndPoint("UpdateParcel", "/carnet/:id/parcel/:parcel", HttpMethod.Put);
            yield return new APIEndPoint("UpdateCarnetMetadata", "/carnet/:id/metadata", HttpMethod.Put);
            yield return new APIEndPoint("GetNotification", "/notification/:token", HttpMethod.Get);
            yield return new APIEndPoint("GetPlans", "/plans", HttpMethod.Get);
            yield return new APIEndPoint("CreatePlan", "/plan", HttpMethod.Post);
            yield return new APIEndPoint("DeletePlan", "/plan/:id", HttpMethod.Delete);
            yield return new APIEndPoint("CreateSubscription", "/plan/:id/subscription", HttpMethod.Post);
            yield return new APIEndPoint("DetailSubscription", "/subscription/:id", HttpMethod.Get);
            yield return new APIEndPoint("PaySubscription", "/subscription/:id/pay", HttpMethod.Post);
            yield return new APIEndPoint("CancelSubscription", "/subscription/:id/cancel", HttpMethod.Put);
            yield return new APIEndPoint("UpdateSubscriptionMetadata", "/subscription/:id/metadata", HttpMethod.Put);
            yield return new APIEndPoint("GetInstallments", "/installments", HttpMethod.Get);
            yield return new APIEndPoint("ResendBillet", "/charge/:id/billet/resend", HttpMethod.Post);
            yield return new APIEndPoint("CreateChargeHistory", "/charge/:id/history", HttpMethod.Post);
            yield return new APIEndPoint("ResendCarnet", "/carnet/:id/resend", HttpMethod.Post);
            yield return new APIEndPoint("ResendParcel", "/carnet/:id/parcel/:parcel/resend", HttpMethod.Post);
            yield return new APIEndPoint("CreateCarnetHistory", "/carnet/:id/history", HttpMethod.Post);
            yield return new APIEndPoint("CancelCarnet", "/carnet/:id/cancel", HttpMethod.Put);
            yield return new APIEndPoint("CancelParcel", "/carnet/:id/parcel/:parcel/cancel", HttpMethod.Put);
            yield return new APIEndPoint("LinkCharge", "/charge/:id/link", HttpMethod.Post);
            yield return new APIEndPoint("ChargeLink", "/charge/:id/link", HttpMethod.Post);
            yield return new APIEndPoint("UpdateChargeLink", "/charge/:id/link", HttpMethod.Put);
            yield return new APIEndPoint("UpdatePlan", "/plan/:id", HttpMethod.Put);
            yield return new APIEndPoint("CreateSubscriptionHistory", "/subscription/:id/history", HttpMethod.Post);
            yield return new APIEndPoint("SettleCharge", "/charge/:id/settle", HttpMethod.Put);
            yield return new APIEndPoint("SettleCarnetParcel", "/carnet/:id/parcel/:parcel/settle", HttpMethod.Put);
            yield return new APIEndPoint("OneStep", "/charge/one-step", HttpMethod.Post);
        }
    }
}
