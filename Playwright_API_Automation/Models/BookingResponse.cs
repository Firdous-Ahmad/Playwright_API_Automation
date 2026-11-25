using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Playwright_API_Automation.Models
{
    public class BookingResponse
    {
        [JsonPropertyName("bookingid")]
        public int BookingId { get; set; }

        [JsonPropertyName("booking")]
        public Booking Booking { get; set; }
    }

}
