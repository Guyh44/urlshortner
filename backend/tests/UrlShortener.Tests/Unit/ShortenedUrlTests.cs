using UrlShortener.Domain.Entities;
using Xunit;

namespace UrlShortener.Tests.Unit;

public class ShortenedUrlTests
{
    //test that chacks that everything works as expected when given a vail input
    [Fact]
    public void Constructor_ValidParameters_CreatesShortenedUrl()
    {
        var originalUrl = "https://guy.com";
        var shortCode = "abc123";
        var ttl = 60;

        var shortenedUrl = new ShortenedUrl(originalUrl, shortCode, ttl);

        Assert.Equal(originalUrl, shortenedUrl.OriginalUrl);
        Assert.Equal(shortCode, shortenedUrl.ShortCode);
        Assert.True(shortenedUrl.ExpiresAt.HasValue);
        Assert.True(shortenedUrl.IsValid);
    }
    
    //test that chacks that i get an error when inputting empty url
    [Fact]
    public void Constructor_EmptyOriginalUrl_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ShortenedUrl("", "abc123"));
    }

    //test that chacks that i cant have urls with empty short codes
    [Fact]
    public void Constructor_EmptyShortCode_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ShortenedUrl("https://example.com", ""));
    }
    
    //test that chacks the logic of a never-expire url
    [Fact]
    public void IsValid_PermanentUrl_ReturnsTrue()
    {
        var shortenedUrl = new ShortenedUrl("https://example.com", "abc123", 0);

        Assert.True(shortenedUrl.IsValid);
        Assert.False(shortenedUrl.IsExpired);
    }
}
