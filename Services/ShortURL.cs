using System;
using System.Collections.Generic;
using urlshortner.Models;

namespace urlshortner.Services
{
    //This class handles URL shortening logic
    class ShortURL
    {
        // Dictionary to store mappings: short code and original long URL
        private static Dictionary<string, string> allurls = new Dictionary<string, string>();
        private static string baseUrl = "http://harpaz.url/"; //base url
        private static int shortCodeLength = 6; // short url length

        /*
        the func gets string "longUrl" that contain the original url
        this func call to GenerateRandomString to create a short url
        then the func saves it in a dict, short url is key and long is value
        */
        public static string ShortenUrl(string longUrl)
        {
            string shortenUrl = "";

            do{
                shortenUrl = RandomString.GenerateRandomString(shortCodeLength);
            } while (allurls.ContainsKey(shortenUrl));

            allurls.Add(shortenUrl, longUrl);
            return baseUrl + shortenUrl;
        }
        /*
        this func Retrieves the original URL based on the short code.
        the func gets "shorturl" (just the short code without the base)
        the func return the original url or null if not found
        */
        public static string? GetOriginalUrl(string shorturl)
        {
            return allurls.TryGetValue(shorturl, out var url) ? url : null;
        }   

    }
}