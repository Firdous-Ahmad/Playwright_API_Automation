
using System.Collections.Generic;
using Microsoft.Playwright;

namespace Playwright_API_Automation.API
{
    public class APIClient
    {
        private readonly IAPIRequestContext _request;
        private string? _authToken;

        public APIClient(IAPIRequestContext request)
        {
            _request = request;
        }

        // Store authentication token
        public void SetAuthToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Authentication token cannot be empty.");

            _authToken = token;
        }

        // Attach authentication cookie to request
        private APIRequestContextOptions AddAuthToken(APIRequestContextOptions? options)
        {
            options ??= new APIRequestContextOptions();
            if (!string.IsNullOrWhiteSpace(_authToken))
            {
                var headers = options.Headers != null
                    ? new Dictionary<string, string>(options.Headers)
                    : new Dictionary<string, string>();

                headers["Cookie"] = $"token={_authToken}";
                options.Headers = headers;
            }
            return options;
        }

        public Task<IAPIResponse> GetAsync(
            string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.GetAsync(endpoint, AddAuthToken(options));
        }

        public Task<IAPIResponse> PostAsync(
            string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.PostAsync(endpoint, AddAuthToken(options));
        }

        public Task<IAPIResponse> PutAsync(
            string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.PutAsync(endpoint, AddAuthToken(options));
        }

        public Task<IAPIResponse> PatchAsync(
            string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.PatchAsync(endpoint, AddAuthToken(options));
        }

        public Task<IAPIResponse> DeleteAsync(
            string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.DeleteAsync(endpoint, AddAuthToken(options));
        }
    }
}
