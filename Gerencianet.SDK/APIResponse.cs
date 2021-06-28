using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK
{
    public class APIResponse
    {
        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// HTTP Status Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// JSON Content for success or internal error
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Error ?!!
        /// </summary>
        public Exception Exception { get; set; }
    }
}
