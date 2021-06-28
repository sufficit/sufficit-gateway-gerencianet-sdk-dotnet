using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK
{
    public class APIOptions
    {
        public const string SectionName = "Gerencianet";

        public APIAuthOptions Auth { get; set; }

        /// <summary>
        /// Use development environment
        /// </summary>
        public bool SandBox { get; set; }

        /// <summary>
        /// Certificate file for PIX Api
        /// </summary>
        public string Certificate { get; set; }

        public override bool Equals(object obj)
        {
            APIOptions other = obj as APIOptions;
            if(other == null) return false;

            return
                this.Auth == other.Auth &&
                this.SandBox == other.SandBox &&
                this.Certificate == other.Certificate;            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
