using Microsoft.AspNetCore.Mvc;
using urlshortner.Services;

[ApiController]
[Route("/")]
public class UrlShortenerController : ControllerBase
{
    [HttpPost("shorten")]
    public IActionResult Shorten([FromBody] UrlRequest request)
    {
        if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
            return BadRequest("Invalid URL.");

        string fullShortUrl = ShortURL.ShortenUrl(request.Url); 
        return Ok(new { shortUrl = fullShortUrl });
    }

    [HttpGet("{code}")]
    public IActionResult RedirectToOriginalUrl(string code)
    {
        var originalUrl = ShortURL.GetOriginalUrl(code);
        if (originalUrl == null)
        {
            return NotFound("Short URL not found");
        }

        return Redirect(originalUrl);
    }

}

public class UrlRequest
{
    public string Url { get; set; } = string.Empty;
}
