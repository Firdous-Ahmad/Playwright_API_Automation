using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using Playwright_API_Automation.API;
namespace Playwright_API_Automation.Base
{
    public class BaseTest
    {
        protected IPlaywright Playwright { get; private set; }
        protected IAPIRequestContext APIRequest { get; private set; }
        protected APIClient Client { get; private set; }
        protected const string BaseUrl = "https://restful-booker.herokuapp.com";


        [SetUp]
        public async Task SetUp()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            APIRequest = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = BaseUrl
            });
            Client = new APIClient(APIRequest);
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
