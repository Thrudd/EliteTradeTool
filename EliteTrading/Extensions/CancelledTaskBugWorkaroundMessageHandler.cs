using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EliteTrading.Extensions {
    class CancelledTaskBugWorkaroundMessageHandler : DelegatingHandler {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // Try to suppress response content when the cancellation token has fired; ASP.NET will log to the Application event log if there's content in this case.
            if (cancellationToken.IsCancellationRequested) {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return response;
        }
    }
}