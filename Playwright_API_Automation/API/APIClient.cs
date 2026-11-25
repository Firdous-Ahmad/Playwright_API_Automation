using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;

namespace Playwright_API_Automation.API
{
    public class APIClient
    {
        private readonly IAPIRequestContext _request;
        public APIClient(IAPIRequestContext request)
        {
            _request = request;
        }

        public Task<IAPIResponse> GetAsync(string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.GetAsync(endpoint, options);
        }

        public Task<IAPIResponse> PostAsync(string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.PostAsync(endpoint, options);
        }
        public Task<IAPIResponse> PutAsync(string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.PutAsync(endpoint, options);
        }
        public Task<IAPIResponse> PatchAsync(string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.PatchAsync(endpoint, options);
        }

        public Task<IAPIResponse> DeleteAsync(string endpoint, APIRequestContextOptions? options = null)
        {
            return _request.DeleteAsync(endpoint, options);
        }
    }
}
