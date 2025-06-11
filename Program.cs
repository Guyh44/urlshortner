
using urlshortner.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AddDB>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");
//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
