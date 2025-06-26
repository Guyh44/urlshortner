namespace UrlShortener.Domain.Interfaces;

public interface ICodeGenerator
{
    string GenerateCode(int length);
}