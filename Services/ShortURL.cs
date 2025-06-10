using System;
using System.Collections.Generic;
using urlshortner.Models;

namespace urlshortner.Services
{
    //This class handles URL shortening logic
    public class ShortURL
    {
        private static readonly AddDB UrlDB = new AddDB();
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

            string existShortcut = UrlDB.GetShortCodeByLongUrl(longUrl);
            if(!string.IsNullOrEmpty(existShortcut)) 
            {
                return baseUrl + existShortcut;
            }
            
            do
            {
                shortenUrl = RandomString.GenerateRandomString(shortCodeLength);
            } while (!string.IsNullOrEmpty(UrlDB.GetLongUrlByShortCode(shortenUrl)));

            UrlDB.InsertValues(longUrl, shortenUrl);
            return baseUrl + shortenUrl;
        }
        /*
        this func Retrieves the original URL based on the short code.
        the func gets "shorturl" (just the short code without the base)
        the func return the original url or null if not found
        */
        public static string? GetOriginalUrl(string shorturl)
        {
            var originalUrl = UrlDB.GetLongUrlByShortCode(shorturl);
            return string.IsNullOrEmpty(originalUrl) ? null : originalUrl;
        }   

    }
}