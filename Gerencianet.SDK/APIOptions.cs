using System;
using System.Collections.Generic;
using System.Text;

namespace Gerencianet.SDK
{
    public class APIOptions
    {
        /// <summary>
        /// Authentication Pair to Production Environment
        /// </summary>
        public APIAuthPair AuthProduction;

        /// <summary>
        /// Authentication Pair to Development Environment
        /// </summary>
        public APIAuthPair AuthDevelopment;

        /// <summary>
        /// Authentication Pair to Playground Environment
        /// </summary>
        public APIAuthPair AuthPlayground;
    }
}
