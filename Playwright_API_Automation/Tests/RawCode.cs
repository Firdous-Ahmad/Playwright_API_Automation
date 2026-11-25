using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Playwright;
using Playwright_API_Automation.Models;
using Playwright_API_Automation.Base;

namespace Playwright_API_Automation.Tests
{
    internal class RawCode : BaseTest
    {
        [Test]
        public async Task PostCall_RAW()
        {
            var booking = new Booking
            {
                FirstName = "John",
                LastName = "Doe",
                TotalPrice = 100,
                DepositPaid = true,
                BookingDates = new BookingDates
                {
                    Checkin = "2023-01-01",
                    Checkout = "2023-01-05"
                },
                AdditionalNeeds = "Breakfast"
            };
            var response = await Client.PostAsync("/booking", new APIRequestContextOptions
            {
                DataObject = booking
            });
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Status, Is.EqualTo(200),
                            "Expected status code 200 but got " + (int)response.Status);
                Assert.That(response.Ok, Is.True);
            }
            var responseBody = await response.TextAsync();
            Assert.That(responseBody, Is.Not.Null.And.Not.Empty, "Response body should not be null or empty");
            TestContext.Out.WriteLine(responseBody);
            var responseDocument = JsonDocument.Parse(responseBody);
            var rootElement = responseDocument.RootElement;
            var bookingRoot = rootElement.GetProperty("booking");
            Assert.Multiple(() =>
            {
                Assert.That(rootElement.GetProperty("bookingid").GetInt32(), Is.GreaterThan(0));
                Assert.That(bookingRoot.GetProperty("firstname").GetString(), Is.EqualTo(booking.FirstName));
                Assert.That(bookingRoot.GetProperty("lastname").GetString(), Is.EqualTo(booking.LastName));
                Assert.That(bookingRoot.GetProperty("totalprice").GetDecimal(), Is.EqualTo(booking.TotalPrice));
                Assert.That(bookingRoot.GetProperty("depositpaid").GetBoolean(), Is.EqualTo(booking.DepositPaid));
            });
        }

    }
}
