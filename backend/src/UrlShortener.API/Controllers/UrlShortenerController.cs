// this is the API controller, it handles all HTTP endpoints.
// it exposes routes for creating short URLs with or without custom codes,
// redirecting from short codes to original URLs, 
// it's also retrieving analytics of how many times a shortcode was used

using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("/")] // base route of the controller
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlShorteningService _urlShorteningService;

    public UrlShortenerController(IUrlShorteningService urlShorteningService)
    {
        _urlShorteningService = urlShorteningService ?? throw new ArgumentNullException(nameof(urlShorteningService));
    }
    
    // creates a new shortened URL using a random generated short code
    // gets the request and returns ok if it could shorten it and trows an error if not
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
            // return 400 for invalid URLs or bad input
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // creates a new shortened URL using an inputted short code
    // gets the request and returns ok if it could shorten it and trows an error if not
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
            // return 400 for invalid URLs or bad input
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // return 409 if custom code already in use
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // redirects the user to the original URL based on the short code
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
            // returns 302 redirect
            return Redirect(originalUrl);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    // returns the number of times the shortened URL has been accessed
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