using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Services;

public class RandomCodeGenerator : ICodeGenerator
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly Random _random = new();

    public string GenerateCode(int length)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(length));

        return new string(Enumerable.Repeat(Characters, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}