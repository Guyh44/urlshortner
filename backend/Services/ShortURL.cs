using System;
using System.Collections.Generic;
using urlshortner.Models;


namespace urlshortner.Services;

//This class handles URL shortening logic
public class ShortURL
{

    private static readonly AddDB UrlDB = new AddDB();
    private static string baseUrl = "http://localhost:5235/"; //base url
    private static int shortCodeLength = 6; // short url length

    /*
    the func gets string "longUrl" that contain the original url
    this func call to GenerateRandomString to create a short url
    then the func saves it in a db
    */
    public static string ShortenUrl(string longUrl, int ttlMinutes)
    {
        string shortenUrl = "";

        // Check if URL already exists and is still valid
        string existShortcut = UrlDB.GetShortCodeByLongUrl(longUrl);
        if (!string.IsNullOrEmpty(existShortcut) && UrlDB.IsUrlValid(existShortcut))
        {
            return baseUrl + existShortcut;
        }

        // Generate unique short code
        do
        {
            //shortenUrl = RandomStringGenerator.GenerateRandomString(shortCodeLength);
        } while (UrlDB.IsUrlValid(shortenUrl));

        UrlDB.InsertValues(longUrl, shortenUrl, ttlMinutes);
        return baseUrl + shortenUrl;
    }

    /*
    the func gets string "longUrl" that contain the original url and a custum code that will be the short code
    this func checks if the url is already in the db
    then the func saves it in a db with the custom short code
    */
    public static string CustomCode(string longUrl, string customCode, int ttlMinutes)
    {
        // Check if custom short code is valid (not expired)
        if (UrlDB.IsUrlValid(customCode))
        {
            throw new InvalidOperationException("Custom short code already in use.");
        }

        // Check if URL already exists with a different short code
        string existShortcut = UrlDB.GetShortCodeByLongUrl(longUrl);
        if (!string.IsNullOrEmpty(existShortcut))
        {
            return baseUrl + existShortcut;
        }

        UrlDB.InsertValues(longUrl, customCode, ttlMinutes);
        return baseUrl + customCode;
    }
    /*
    this func Retrieves the original URL based on the short code.
    the func gets "shorturl" (just the short code without the base)
    the func return the original url or null if not found
    */
    public static string? GetOriginalUrl(string shorturl)
    {
        // Check if URL is valid first - handles cleanup of expired URL
        if (!UrlDB.IsUrlValid(shorturl))
            return null;
        var originalUrl = UrlDB.GetLongUrlByShortCode(shorturl);
        return string.IsNullOrEmpty(originalUrl) || originalUrl == "expired url" ? null : originalUrl;
    }

}
