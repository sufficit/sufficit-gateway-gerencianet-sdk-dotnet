using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Gerencianet.SDK.Exceptions
{
    /// <summary>
    /// Acionado quando o tempo máximo de requisição do cliente http foi alcançado 
    /// </summary>
    public class HttpClientTimeOutException : Exception
    {
        public HttpClientTimeOutException(HttpRequestMessage request, TimeSpan timeout, CancellationToken cancellationToken, long elapsedMilliseconds = default)
        {
            this.Request = request;
            this.Timeout = timeout;
            this.CancellationToken = cancellationToken;
            this.ElapsedMilliseconds = elapsedMilliseconds;
        }

        public long ElapsedMilliseconds { get; }

        /// <summary>
        /// Tempo de expiração do http client usado
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Requisição HTTP utilizada para gerar essa exceção
        /// </summary>
        public HttpRequestMessage Request { get; }

        /// <summary>
        /// Ficha de cancelamento externa utilizada <br />
        /// Serve para saber se essa exceção foi acionada externamente ou por timeout interno do http client
        /// </summary>
        public CancellationToken CancellationToken { get; }

        public override string Message => $"http client request timeout ( { ElapsedMilliseconds } / { Timeout.TotalMilliseconds } )ms";
    }
}
