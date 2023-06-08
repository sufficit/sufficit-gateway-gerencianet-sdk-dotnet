using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK
{
    public class APIOptions
    {
        public const string SECTIONNAME = "Gerencianet";

        public APIAuthOptions Auth { get; }

        /// <summary>
        /// Use development environment
        /// </summary>
        public bool SandBox { get; set; }

        /// <summary>
        /// Certificate file for PIX Api
        /// </summary>
        public string? Certificate { get; set; }

        /// <summary>
        /// Default Uri to notifications callback
        /// </summary>
        public string? Notification { get; set; }

        public APIOptions()
        {
            Auth = new APIAuthOptions();
        }

        public override bool Equals(object? other) =>
            other is APIOptions p && (p.Auth, p.SandBox, p.Certificate, p.Notification).Equals((Auth, SandBox, Certificate, Notification));

        public override int GetHashCode() => (Auth, SandBox, Certificate, Notification).GetHashCode();
    }
}
