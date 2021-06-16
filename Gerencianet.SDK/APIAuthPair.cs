using System;
using System.Collections.Generic;
using System.Text;

namespace Gerencianet.SDK
{
    /// <summary>
    /// ID and Secret for authentication calls on endpoints
    /// </summary>
    public struct APIAuthPair
    {
        public string id { get; set; }
        public string secret { get; set; }
    }
}
