using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wasalni_Utility
{
    public static class BuiltInMethods
    {
        public static async Task<string?> GetCityFromNominatimAsync(double latitude, double longitude,HttpClient _httpClient)
        {
            // رابط Nominatim
            string url = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json&addressdetails=1&zoom=18&accept-language=en";

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
    //    public static async Task<bool> driverCardsEndDateValidatorAsync(
    //IFormFile file,
    //HttpClient httpClient)
    //    {
    //        const string apiUrl = "https://api.ocr.space/parse/image";

    //        using var form = new MultipartFormDataContent();

    //        form.Add(new StringContent("K87975876588957"), "apikey");
    //        form.Add(new StringContent("ara"), "language");

    //        using var stream = file.OpenReadStream();
    //        var fileContent = new StreamContent(stream);
    //        fileContent.Headers.ContentType =
    //            new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

    //        form.Add(fileContent, "file", file.FileName);

    //        using var response = await httpClient.PostAsync(apiUrl, form);
    //        response.EnsureSuccessStatusCode();

    //        var json = await response.Content.ReadAsStringAsync();
    //        using var doc = JsonDocument.Parse(json);

    //        int ocrExitCode = doc.RootElement
    //            .GetProperty("OCRExitCode")
    //            .GetInt32();

    //        if (ocrExitCode != 1)
    //            return false;

    //        string parsedText = doc.RootElement
    //            .GetProperty("ParsedResults")[0]
    //            .GetProperty("ParsedText")
    //            .GetString() ?? "";

    //        Regex dateRegex = new Regex(@"\b(\d{4}\.\d{2}\.\d{2})\b");
    //        var matches = dateRegex.Matches(parsedText);

    //        if (matches.Count == 0)
    //            return false;

    //        bool parsed = DateOnly.TryParse(matches[^1].Value, out DateOnly result);

    //        return parsed && result > DateOnly.FromDateTime(DateTime.Today);
    //    }

        //public static async Task<bool> driverCardsEndDateValidatorAsync(IFormFile file, HttpClient _httpClient)
        //{
        //    var url = $"https://subcarbonaceous-transgressively-dione.ngrok-free.dev";
        //}
    }
}
