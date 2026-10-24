using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace PaymentBackend.BL.Http
{
    public abstract class AbstractHttpResolver
    {
        protected static async Task<HttpResponseData> BuildOkResponseAsync(HttpRequestData req)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.OK);
            return await Task.FromResult(httpResponseData);
        }

        protected static async Task<HttpResponseData> BuildOkResponseAsync<T>(HttpRequestData req, T realResponse, CancellationToken cancellationToken)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.OK);
            await httpResponseData.WriteAsJsonAsync(realResponse, cancellationToken);
            return httpResponseData;
        }
        protected static async Task<HttpResponseData> BuildBadRequestResponseAsync(HttpRequestData req)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            return await Task.FromResult(httpResponseData);
        }

        protected static async Task<HttpResponseData> BuildBadRequestResponseAsync(Exception e, HttpRequestData req, CancellationToken cancellationToken)
        {
            string msg = $"Bad Request: {e.Message}";

            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            await httpResponseData.WriteAsJsonAsync(msg, cancellationToken);
            return httpResponseData;
        }

        protected static async Task<HttpResponseData> BuildNotFoundResponseAsync(HttpRequestData req)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            return await Task.FromResult(httpResponseData);
        }

        protected static async Task<HttpResponseData> BuildNotFoundResponseAsync(Exception e, HttpRequestData req, CancellationToken cancellationToken)
        {
            string msg = $"Resource not found: {e.Message}"; 

            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            await httpResponseData.WriteAsJsonAsync(msg, cancellationToken);
            return httpResponseData;
        }

        protected static async Task<HttpResponseData> BuildInternalServerErrorResponseAsync(Exception e, HttpRequestData req, CancellationToken cancellationToken)
        {
            string msg = $"Internal Server Error: {e.Message}";

            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            await httpResponseData.WriteAsJsonAsync(msg, cancellationToken);
            return httpResponseData;
        }
    }
}
