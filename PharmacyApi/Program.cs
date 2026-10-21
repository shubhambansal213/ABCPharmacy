using PharmacyApi.Middleware;
using PharmacyApi.Repositories;
using PharmacyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Repository is a singleton: it owns the file lock that guards concurrent JSON file access.
builder.Services.AddSingleton<IMedicineRepository, JsonMedicineRepository>();
builder.Services.AddScoped<IMedicineService, MedicineService>();

// Built-in OpenAPI document generation (served at /openapi/v1.json).
// Learn more at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Global exception handling - first in the pipeline so it catches errors from everything downstream.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI at /swagger for trying the API in the browser, reading the built-in OpenAPI document.
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "PharmacyApi v1"));
}

app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();
