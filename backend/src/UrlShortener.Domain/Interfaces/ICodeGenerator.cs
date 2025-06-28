// an interface for generating a random short code

namespace UrlShortener.Domain.Interfaces;

public interface ICodeGenerator
{
    // gets a number that represents the length of the wanted short code
    //returns a random code in that length
    string GenerateCode(int length);
}