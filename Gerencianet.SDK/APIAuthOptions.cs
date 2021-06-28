using System;
using System.Collections.Generic;
using System.Text;

namespace GerencianetSDK
{
    public class APIAuthOptions : APIAuthPair
    {
        private APIAuthPair _prod;
        private APIAuthPair _devel;
        private APIAuthPair _play;

        /// <summary>
        /// Authentication Pair to Production Environment
        /// </summary>
        public APIAuthPair Production
        {
            get { return _prod ?? this; }
            set { _prod = value; }
        }

        /// <summary>
        /// Authentication Pair to Development Environment
        /// </summary>
        public APIAuthPair Development
        {
            get { return _devel ?? this; }
            set { _devel = value; }
        }

        /// <summary>
        /// Authentication Pair to Playground Environment
        /// </summary>
        public APIAuthPair Playground
        {
            get { return _play ?? this; }
            set { _play = value; }
        }
    }
}
