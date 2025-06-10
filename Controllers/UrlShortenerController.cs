using Microsoft.AspNetCore.Mvc;
using urlshortner.Services;

[ApiController]
[Route("/")]
public class UrlShortenerController : ControllerBase
{

    //Creates a shortened URL from a provided long URL
    //"request" Request the URL to be shortened
    //the post JSON response with the shortened URL or BadRequest if URL is invalid
    [HttpPost("api/shorten")]
    public IActionResult Shorten([FromBody] UrlRequest request)
    {
        if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
            return BadRequest("Invalid URL.");

        string fullShortUrl = ShortURL.ShortenUrl(request.Url); 
        return Ok(new { shortUrl = fullShortUrl });
    }

    //Redirects from a short code to the original URL
    //"code" the short code that represents a long url
    //redirect to the original url
    [HttpGet("{code}")]
    public IActionResult RedirectToOriginalUrl(string code)
    {
        var originalUrl = ShortURL.GetOriginalUrl(code);
        if (originalUrl == null)
        {
            return NotFound("Short URL not found");
        }
        //return Ok(new { shortUrl = originalUrl });
        return Redirect(originalUrl);
    }

}

public class UrlRequest
{
    public string Url { get; set; } = string.Empty;
}
