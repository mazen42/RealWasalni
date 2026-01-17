using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wasalni_Utility
{
    public static class BuiltInMethods
    {
        public static async Task<string?> GetCityFromNominatimAsync(double latitude, double longitude,HttpClient _httpClient)
        {
            // رابط Nominatim
            string url = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json&addressdetails=1&zoom=18";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            // مهم جدًا: User-Agent لتجنب الحظر
            request.Headers.Add("User-Agent", "WasalniApp/1.0 (mazentohameee@gmail..com)");

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("address", out JsonElement address))
            {
                // أفضل محاولة: neighbourhood أولًا، ثم borough، ثم city
                if (address.TryGetProperty("neighbourhood", out JsonElement neighbourhood))
                    return neighbourhood.GetString();
                if (address.TryGetProperty("borough", out JsonElement borough))
                    return borough.GetString();
                if (address.TryGetProperty("city", out JsonElement city))
                    return city.GetString();
            }

            return null;
        }
    }
}
