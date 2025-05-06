using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace PaymentBackend.BL.Http
{
    public abstract class AbstractHttpResolver
    {
        protected static async Task<HttpResponseData> BuildOkResponse(HttpRequestData req)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.OK);
            return await Task.FromResult(httpResponseData);
        }

        protected static async Task<HttpResponseData> BuildOkResponse<T>(HttpRequestData req, T realResponse)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.OK);
            await httpResponseData.WriteAsJsonAsync(realResponse);
            return httpResponseData;
        }
        protected static async Task<HttpResponseData> BuildBadRequestResponse(HttpRequestData req)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            return await Task.FromResult(httpResponseData);
        }

        protected static async Task<HttpResponseData> BuildBadRequestResponse(Exception e, HttpRequestData req)
        {
            string msg = $"Bad Request: {e.Message}";

            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            await httpResponseData.WriteAsJsonAsync(msg);
            return httpResponseData;
        }

        protected static async Task<HttpResponseData> BuildNotFoundResponse(HttpRequestData req)
        {
            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            return await Task.FromResult(httpResponseData);
        }

        protected static async Task<HttpResponseData> BuildNotFoundResponse(Exception e, HttpRequestData req)
        {
            string msg = $"Resource not found: {e.Message}"; 

            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            await httpResponseData.WriteAsJsonAsync(msg);
            return httpResponseData;
        }

        protected static async Task<HttpResponseData> BuildInternalServerErrorResponse(Exception e, HttpRequestData req)
        {
            string msg = $"Internal Server Error: {e.Message}";

            HttpResponseData httpResponseData = req.CreateResponse(HttpStatusCode.NotFound);
            await httpResponseData.WriteAsJsonAsync(msg);
            return httpResponseData;
        }
    }
}
