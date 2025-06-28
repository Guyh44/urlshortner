// this file sets up the web application host, registers application services, 
// enables Swagger for API documentation, and defines the CORS policy.
// this file is also responsible for injecting the repository, services and random code generator classes

using UrlShortener.Application.Services;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;
using UrlShortener.Infrastructure.Services;
using UrlShortener.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args); // creates the application web builder

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // register the swaggers API

// register dependencies injections
builder.Services.AddSingleton<IUrlRepository, SqliteUrlRepository>();
builder.Services.AddSingleton<ICodeGenerator, RandomCodeGenerator>();
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();

// configurs CORS, which allows the front to talk to the back
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // where the front sits
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// configure the HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();