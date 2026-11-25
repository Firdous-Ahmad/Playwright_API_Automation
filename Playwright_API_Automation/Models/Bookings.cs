
using System.Text.Json.Serialization;

namespace Playwright_API_Automation.Models
{
    public class Booking
    {
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("totalprice")]
        public int TotalPrice { get; set; }

        [JsonPropertyName("depositpaid")]
        public bool DepositPaid { get; set; }

        [JsonPropertyName("bookingdates")]
        public BookingDates BookingDates { get; set; }

        [JsonPropertyName("additionalneeds")]
        public string AdditionalNeeds { get; set; }
    }

    public class BookingDates
    {
        [JsonPropertyName("checkin")]
        public string Checkin { get; set; }

        [JsonPropertyName("checkout")]
        public string Checkout { get; set; }
    }
}
