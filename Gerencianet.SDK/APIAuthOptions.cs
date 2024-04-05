using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK
{
    /// <summary>
    /// Authentication Pair to Production Environment
    /// </summary>
    public class APIAuthOptions : APIAuthPair
    {       
        /// <summary>
        /// Authentication Pair to Development Environment
        /// </summary>
        public APIAuthPair? Development { get; set; }

        /// <summary>
        /// Authentication Pair to Playground Environment
        /// </summary>
        public APIAuthPair? Playground { get; set; }

        public override bool Equals(object? other) =>
            other is APIAuthOptions p && (p.Development, p.Playground, p.Id, p.Secret).Equals((Development, Playground, Id, Secret));

        public override int GetHashCode() => (Development, Playground, Id, Secret).GetHashCode();
    }
}
