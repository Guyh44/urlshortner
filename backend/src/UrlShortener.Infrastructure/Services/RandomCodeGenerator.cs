//this code is responsible for generating a random string for the shortcode

using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Services;

public class RandomCodeGenerator : ICodeGenerator
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // all the chars
    private readonly Random _random = new();

    public string GenerateCode(int length)
    {
        if (length <= 0) // given shortcode length must be a positive number 
            throw new ArgumentException("Length must be greater than 0", nameof(length));
        var shortcode = new char[length];
        for (int i = 0; i < length; i++)
        {
            shortcode[i] = Characters[_random.Next(Characters.Length)];
        }

        return new string(shortcode);
    }
}