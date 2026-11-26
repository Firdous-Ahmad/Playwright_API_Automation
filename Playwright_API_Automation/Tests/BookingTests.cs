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
        #region CRUD Tests:
        #region GET Tests:
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
        public async Task GetBookingWithInvalidId_ShouldReturnNotFound()
        {
            var response = await Client.GetAsync("/booking/9999999");
            Assert.That(response.Status, Is.EqualTo(404),
                "Expected status code 404 but got " + (int)response.Status);
        }

        [Test]
        public async Task GetBookingWithAuthentication_ShouldReturnSuccess()
        {
            // Step 1: Authenticate
            var (status, body) = await Auth.AuthenticateAsync("admin", "password123");

            Assert.That(status, Is.EqualTo(200));
            Assert.That(body.Token, Is.Not.Null.And.Not.Empty);

            // Step 2: Store the token in APIClient
            Client.SetAuthToken(body.Token!);

            // Step 3: Make an authenticated request
            var response = await Client.GetAsync("/booking");

            Assert.That(response.Status, Is.EqualTo(200));
        }
        #endregion GET Tests:
        #region POST Tests:
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

        [Test]
        public async Task CreateBookingWithInvalidData_ShouldReturnBadRequest()
        {
            var invalidBooking = new
            {
                FirstName = "John",
                LastName = "Doe",
                TotalPrice = -100, // Invalid total price
                DepositPaid = true,
                BookingDates = new
                {
                    Checkin = "2023-01-01",
                    Checkout = "2023-01-05"
                },
                AdditionalNeeds = "Breakfast"
            };
            var response = await Client.PostAsync("/booking", new APIRequestContextOptions
            {
                DataObject = invalidBooking
            });
            // Inspect response
            var responseBody = await response.TextAsync();
            TestContext.Out.WriteLine(responseBody);
            Assert.That(response.Status, Is.EqualTo(400),
                "Expected status code 400 but got " + (int)response.Status);
        }
        [Test]
        public async Task CreateBookingWithMissingFields_ShouldReturnBadRequest()
        {
            var incompleteBooking = new
            {
                FirstName = "John",
                // LastName is missing
                TotalPrice = 100,
                DepositPaid = true,
                BookingDates = new
                {
                    Checkin = "2023-01-01",
                    Checkout = "2023-01-05"
                },
                AdditionalNeeds = "Breakfast"
            };
            var response = await Client.PostAsync("/booking", new APIRequestContextOptions
            {
                DataObject = incompleteBooking
            });
            // Inspect response
            var responseBody = await response.TextAsync();
            TestContext.Out.WriteLine(responseBody);
            Assert.That(response.Status, Is.EqualTo(400),
                "Expected status code 400 but got " + (int)response.Status);
        }

        [Test]
        public async Task CreateAuthToken_WithValidCredentials_ShouldReturnToken()
        {
            var (status, body) = await AuthenticateClientAsync();
         //   var (status, body) = await Auth.AuthenticateAsync("admin", "password123");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status, Is.EqualTo(200));
                Assert.That(body.Token, Is.Not.Null.And.Not.Empty);
                Assert.That(body.Reason, Is.Null.Or.Empty);
            }
            TestContext.Out.WriteLine($"Authentication Token: {body.Token}");
        }


        [Test]
        public async Task CreateAuthToken_WithInvalidCredentials_ShouldReturnReason()
        {
            var (status, body) = await AuthenticateClientAsync("wronguser", "wrongpassword");
            //var (status, body) = await Auth.AuthenticateAsync("wronguser", "wrongpassword");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(status, Is.EqualTo(200));
                Assert.That(body.Token, Is.Null.Or.Empty);
                Assert.That(body.Reason, Is.EqualTo("Bad credentials"));
            }
            TestContext.Out.WriteLine($"Authentication Reason: {body.Reason}");
        }


        [Test]
        public async Task CreateBooking_ShouldReturnBookingId()
        {
            var booking = new
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = 500,
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "2026-11-26",
                    checkout = "2026-11-28"
                },
                additionalneeds = "Breakfast"
            };

            var response = await Client.PostAsync("/booking",
                new APIRequestContextOptions
                {
                    DataObject = booking
                });

            Assert.That(response.Status, Is.EqualTo(200));

            var responseBody =
                await response.JsonAsync<BookingResponse>();

            Assert.That(responseBody, Is.Not.Null);
            Assert.That(responseBody.BookingId, Is.GreaterThan(0));

            TestContext.Out.WriteLine(
                $"Created Booking ID: {responseBody.BookingId}");
        }


        #endregion POST Tests:

        #region PUT Tests:
        [Test]
        public async Task UpdateBookingWithPut_ShouldReturnUpdatedBooking()
        {
            // 1. Create a booking
            var booking = new
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = 500,
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "2026-11-26",
                    checkout = "2026-11-28"
                },
                additionalneeds = "Breakfast"
            };

            var createResponse = await Client.PostAsync("/booking",
                new APIRequestContextOptions
                {
                    DataObject = booking
                });

            Assert.That(createResponse.Status, Is.EqualTo(200));

            var createdBooking =
                await createResponse.JsonAsync<BookingResponse>();

            int bookingId = createdBooking?.BookingId ?? 0;

            // 2. Authenticate
            var (status, body) = await AuthenticateClientAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(status, Is.EqualTo(200));
                Assert.That(body.Token, Is.Not.Null.And.Not.Empty);
            }

            // 3. Prepare updated booking details
            var updatedBooking = new
            {
                firstname = "David",
                lastname = "Smith",
                totalprice = 1000,
                depositpaid = false,
                bookingdates = new
                {
                    checkin = "2026-12-01",
                    checkout = "2026-12-05"
                },
                additionalneeds = "Lunch"
            };

            // 4. Send PUT request
            var putResponse = await Client.PutAsync($"/booking/{bookingId}",
                new APIRequestContextOptions
                {
                    DataObject = updatedBooking
                });

            Assert.That(putResponse.Status, Is.EqualTo(200));

            // 5. Validate updated response
            var responseBody = await putResponse.TextAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(responseBody, Does.Contain("David"));
                Assert.That(responseBody, Does.Contain("Smith"));
                Assert.That(responseBody, Does.Contain("Lunch"));
            }

            TestContext.Out.WriteLine(
                $"Updated Booking ID: {bookingId}");
        }
        #endregion PUT Tests:

        #region PATCH Tests:
        [Test]
        public async Task UpdateBookingWithPatch_ShouldUpdateSelectedFields()
        {
            // 1. Create a booking
            var booking = new
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = 500,
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "2026-11-26",
                    checkout = "2026-11-28"
                },
                additionalneeds = "Breakfast"
            };

            var createResponse = await Client.PostAsync("/booking",
                new APIRequestContextOptions
                {
                    DataObject = booking
                });

            Assert.That(createResponse.Status, Is.EqualTo(200));

            var createdBooking =
                await createResponse.JsonAsync<BookingResponse>();

            int bookingId = createdBooking?.BookingId ?? 0;

            // 2. Authenticate
            var (status, body) = await AuthenticateClientAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(status, Is.EqualTo(200));
                Assert.That(body.Token, Is.Not.Null.And.Not.Empty);
            }

            // 3. Prepare partial update
            var partialBooking = new
            {
                firstname = "David",
                additionalneeds = "Lunch"
            };

            // 4. Send PATCH request
            var patchResponse = await Client.PatchAsync(
                $"/booking/{bookingId}",
                new APIRequestContextOptions
                {
                    DataObject = partialBooking
                });

            Assert.That(patchResponse.Status, Is.EqualTo(200));

            
            // 5. Validate updated booking
            var updatedBooking =
                await patchResponse.JsonAsync<Booking>();


            using (Assert.EnterMultipleScope())
            {
                Assert.That(updatedBooking?.FirstName,
                    Is.EqualTo("David"));

                Assert.That(updatedBooking?.LastName,
                    Is.EqualTo("Doe"));

                Assert.That(updatedBooking?.AdditionalNeeds,
                    Is.EqualTo("Lunch"));
            }

            TestContext.Out.WriteLine(
                $"Partially updated Booking ID: {bookingId}");
        }
        #endregion PATCH Tests:
        #region Delete Tests:
        [Test]
        public async Task DeleteBookingWithAuthentication_ShouldReturnCreated()
        {
            // Create a booking
            var booking = new
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = 500,
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "2026-11-26",
                    checkout = "2026-11-28"
                }
            };

            var createResponse = await Client.PostAsync("/booking",
                new APIRequestContextOptions
                {
                    DataObject = booking
                });

            Assert.That(createResponse.Status, Is.EqualTo(200));

            var createdBooking =
                await createResponse.JsonAsync<BookingResponse>();

            int bookingId = createdBooking?.BookingId ?? 0;

            // Authenticate & Save token in APIClient
            var (status, body) = await AuthenticateClientAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(status, Is.EqualTo(200));
                Assert.That(body.Token, Is.Not.Null.And.Not.Empty);
            }
            // Delete booking (token attached automatically)
            var deleteResponse =
                await Client.DeleteAsync($"/booking/{bookingId}");

            // Verify deletion
            Assert.That(deleteResponse.Status, Is.EqualTo(201));
        }
        #endregion Delete Tests:

        #endregion CRUD Tests:
    }
}
