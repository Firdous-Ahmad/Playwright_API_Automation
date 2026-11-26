using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
namespace Playwright_API_Automation.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
