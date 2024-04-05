using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK
{
    /// <summary>
    /// ID and Secret for authentication calls on endpoints
    /// </summary>
    public class APIAuthPair
    {
        public string Id { get; set; } = default!;

        public string Secret { get; set; } = default!;

        public override bool Equals(object? other) =>
            other is APIAuthPair p && (p.Id, p.Secret).Equals((Id, Secret));

        public override int GetHashCode() => (Id, Secret).GetHashCode();
    }
}
