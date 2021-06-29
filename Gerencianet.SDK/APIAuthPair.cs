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
        public virtual string id { get; set; }
        public virtual string secret { get; set; }
    }
}
