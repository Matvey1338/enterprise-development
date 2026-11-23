using System.Text.Json.Serialization;
using DryCleaning.Api.Extensions;
using DryCleaning.Api.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
                            | HttpLoggingFields.RequestPath
                            | HttpLoggingFields.ResponseStatusCode;
    options.CombineLogs = true;
});
builder.Services.AddInMemoryRepositories();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseHttpLogging();
app.UseExceptionHandler();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();
