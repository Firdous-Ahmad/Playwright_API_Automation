using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using Playwright_API_Automation.Models;

namespace Playwright_API_Automation.Utilities
{
    public class AuthHelper
    {
        private readonly IAPIRequestContext _apiRequest;

        public AuthHelper(IAPIRequestContext apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public async Task<(int Status, AuthResponse Body)> AuthenticateAsync(string username, string password)
        {
            var credentials = new
            {
                username,
                password
            };

            var response = await _apiRequest.PostAsync("/auth",
                new APIRequestContextOptions
                {
                    DataObject = credentials
                });

            var responseBody = await response.JsonAsync<AuthResponse>();
            return responseBody == null
                ? throw new InvalidOperationException(
                    "Authentication response body was empty.")
                : (response.Status, responseBody);
        }
    }
}
