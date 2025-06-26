using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("/")]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlShorteningService _urlShorteningService;

    public UrlShortenerController(IUrlShorteningService urlShorteningService)
    {
        _urlShorteningService = urlShorteningService ?? throw new ArgumentNullException(nameof(urlShorteningService));
    }

    [HttpPost("api/shorten")]
    public async Task<IActionResult> ShortenUrl([FromBody] UrlShortenRequest request)
    {
        try
        {
            var response = await _urlShorteningService.ShortenUrlAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpPost("api/custom/shorten")]
    public async Task<IActionResult> ShortenUrlWithCustomCode([FromBody] CustomUrlShortenRequest request)
    {
        try
        {
            var response = await _urlShorteningService.ShortenUrlWithCustomCodeAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string code)
    {
        try
        {
            var originalUrl = await _urlShorteningService.GetOriginalUrlAsync(code);
            if (originalUrl == null)
            {
                return NotFound("Short URL not found or expired");
            }
            return Redirect(originalUrl);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
    
    [HttpGet("/api/stats/{code}")]
    public async Task<IActionResult> GetClickCount(string code)
    {
        try
        {
            var count = await _urlShorteningService.GetClickCountAsync(code);
            if (count == null)
            {
                return NotFound("Short URL not found or expired");
            }
            return Ok(count);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}