using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Playwright;
using Playwright_API_Automation.Base;
using Playwright_API_Automation.Models;

namespace Playwright_API_Automation.Tests
{
    internal class BookingTests : BaseTest
    {
        [Test]
        public async Task GetAllBookings_ShouldReturnSuccess()
        {
            var response = await Client.GetAsync("/booking");
            Assert.That(response.Status, Is.EqualTo(200),
                "Expected status code 200 but got " + (int)response.Status);
            var responseBody = await response.TextAsync();
            TestContext.Out.WriteLine(responseBody);
            Console.WriteLine(responseBody);
            Assert.That(responseBody, Is.Not.Null.And.Not.Empty, "Response body should not be null or empty");
        }

        [Test]
        public async Task CreateBooking_ShouldReturnSuccess()
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

            // Read response body
            var responseBody = await response.TextAsync();
            TestContext.Out.WriteLine(responseBody);

            // Deserialize JSON into BookingResponse object
            var bookingResponse =
                JsonSerializer.Deserialize<BookingResponse>(responseBody);
           
            // Validate response
            Assert.That(bookingResponse, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                // Validate HTTP response
                Assert.That(response.Status, Is.EqualTo(200));
                Assert.That(response.Ok, Is.True);

                // Validate generated Booking ID
                Assert.That(bookingResponse.BookingId, Is.GreaterThan(0));

                // Validate booking details
                Assert.That(bookingResponse.Booking.FirstName, Is.EqualTo(booking.FirstName));
                Assert.That(bookingResponse.Booking.LastName, Is.EqualTo(booking.LastName));
                Assert.That(bookingResponse.Booking.TotalPrice, Is.EqualTo(booking.TotalPrice));
                Assert.That(bookingResponse.Booking.DepositPaid, Is.EqualTo(booking.DepositPaid));

                // Validate booking dates
                Assert.That(bookingResponse.Booking.BookingDates.Checkin,
                    Is.EqualTo(booking.BookingDates.Checkin));

                Assert.That(bookingResponse.Booking.BookingDates.Checkout,
                    Is.EqualTo(booking.BookingDates.Checkout));

                // Validate additional needs
                Assert.That(bookingResponse.Booking.AdditionalNeeds,
                    Is.EqualTo(booking.AdditionalNeeds));
            }


            // GET: Retrieve the newly created booking
            var getResponse = await Client.GetAsync(
                    $"/booking/{bookingResponse.BookingId}");

            // Validate GET status
            Assert.That(getResponse.Status, Is.EqualTo(200));

            // Read GET response
            var getResponseBody = await getResponse.TextAsync();

            // Deserialize GET response into Booking model
            var retrievedBooking =
                JsonSerializer.Deserialize<Booking>(getResponseBody);

            // Validate retrieved booking
            Assert.That(retrievedBooking, Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(retrievedBooking.FirstName, Is.EqualTo(booking.FirstName));
                Assert.That(retrievedBooking.LastName, Is.EqualTo(booking.LastName));
                Assert.That(retrievedBooking.TotalPrice, Is.EqualTo(booking.TotalPrice));
                Assert.That(retrievedBooking.DepositPaid, Is.EqualTo(booking.DepositPaid));
            }
        }
    }
}
