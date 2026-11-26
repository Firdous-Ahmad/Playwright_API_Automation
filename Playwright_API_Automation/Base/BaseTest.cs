using System;
using System.Collections.Generic;
using System.Text;
using Allure.NUnit;
using Microsoft.Playwright;
using Playwright_API_Automation.API;
using Playwright_API_Automation.Models;
using Playwright_API_Automation.Utilities;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace Playwright_API_Automation.Base
{
    [AllureNUnit]
    public class BaseTest
    {
        protected IPlaywright Playwright { get; private set; }
        protected IAPIRequestContext APIRequest { get; private set; }
        protected APIClient Client { get; private set; }
        protected string BaseUrl { get; private set; }
        protected AuthHelper Auth { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        protected string Username { get; private set; }
        protected string Password { get; private set; }
        protected async Task<(int Status, AuthResponse Body)>AuthenticateClientAsync(
            string? username = null,string? password = null)
        {
            var (status, body) = await Auth.AuthenticateAsync(
                username ?? Username,
                password ?? Password);

            if (status == 200 && !string.IsNullOrWhiteSpace(body.Token))
            {
                Client.SetAuthToken(body.Token);
            }

            return (status, body);
        }

        [SetUp]
        public async Task SetUp()
        {
            var configPath = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,"appsettings.json");

            Configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false)
                .Build();
            BaseUrl = Configuration["Settings:BaseUrl"] 
                ?? throw new InvalidOperationException(
                    "BaseUrl is missing from appsettings.json.");
            Username = Environment.GetEnvironmentVariable("BOOKING_USERNAME")
                ?? throw new InvalidOperationException(
                     "BOOKING_USERNAME environment variable is missing.");
            Password = Environment.GetEnvironmentVariable("BOOKING_PASSWORD")
                ?? throw new InvalidOperationException(
                    "BOOKING_PASSWORD environment variable is missing.");

            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            APIRequest = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = BaseUrl
            });
            Client = new APIClient(APIRequest);
            Auth = new AuthHelper(APIRequest);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (APIRequest != null)
            {
                await APIRequest.DisposeAsync();
                Playwright.Dispose();
            }
        }
    }
}
