using Moq;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using Xunit;

namespace UrlShortener.Tests.Unit;

public class UrlShorteningServiceTests
{
    private readonly Mock<IUrlRepository> _mockRepository;
    private readonly Mock<ICodeGenerator> _mockCodeGenerator;
    private readonly UrlShorteningService _service;

    public UrlShorteningServiceTests()
    {
        _mockRepository = new Mock<IUrlRepository>();
        _mockCodeGenerator = new Mock<ICodeGenerator>();
        _service = new UrlShorteningService(_mockRepository.Object, _mockCodeGenerator.Object);
    }

    [Fact]
    public async Task ShortenUrlAsync_ValidUrl_ReturnsShortUrl()
    {
        var request = new UrlShortenRequest("https://example.com");
        var shortCode = "abc123";

        _mockRepository.Setup(r => r.GetByOriginalUrlAsync(It.IsAny<string>()))
            .ReturnsAsync((ShortenedUrl?)null);
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockCodeGenerator.Setup(g => g.GenerateCode(It.IsAny<int>())).Returns(shortCode);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<ShortenedUrl>()))
            .ReturnsAsync((ShortenedUrl url) => url);

        var result = await _service.ShortenUrlAsync(request);

        Assert.NotNull(result);
        Assert.Contains(shortCode, result.ShortUrl);
    }

    [Fact]
    public async Task ShortenUrlAsync_InvalidUrl_ThrowsArgumentException()
    {
        var request = new UrlShortenRequest("invalid-url");

        await Assert.ThrowsAsync<ArgumentException>(() => _service.ShortenUrlAsync(request));
    }

    [Fact]
    public async Task ShortenUrlWithCustomCodeAsync_ExistingCode_ThrowsInvalidOperationException()
    {
        var request = new CustomUrlShortenRequest("https://example.com", "custom");

        var existing = new ShortenedUrl(request.Url, request.CustomCode, 5);

        _mockRepository.Setup(r => r.GetByShortCodeAsync("custom"))
            .ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.ShortenUrlWithCustomCodeAsync(request));
    }

    [Fact]
    public async Task GetOriginalUrlAsync_ValidCode_ReturnsOriginalUrl()
    {
        var shortCode = "abc123";
        var originalUrl = "https://example.com";
        var shortenedUrl = new ShortenedUrl(originalUrl, shortCode);

        _mockRepository.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(shortenedUrl);

        var result = await _service.GetOriginalUrlAsync(shortCode);

        Assert.Equal(originalUrl, result);
    }

    [Fact]
    public async Task GetOriginalUrlAsync_NotFound_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByShortCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((ShortenedUrl?)null);

        var result = await _service.GetOriginalUrlAsync("notfound");

        Assert.Null(result);
    }
}
