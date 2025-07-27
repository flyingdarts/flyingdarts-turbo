using FlyingDarts.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "FlyingDarts Game API",
        Version = "v1",
        Description = "API for managing game player access using Authress"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configure JWT authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Configure authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // Configure your JWT authentication here
        // This is a basic example - customize based on your auth provider
        options.Authority = "https://your-auth-server.com";
        options.RequireHttpsMetadata = true;
        options.Audience = "your-api-audience";
    });

builder.Services.AddAuthorization(options =>
{
    // Define authorization policies if needed
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add the Game Player Management service with Authress
builder.Services.AddGamePlayerManagement(builder.Configuration);

// Alternative configuration approach:
// builder.Services.AddGamePlayerManagement(options =>
// {
//     options.CustomDomain = "your-account.api.authress.io";
//     options.ServiceClientAccessKey = "your-service-client-access-key";
// });

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlyingDarts Game API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Example endpoints for testing
app.MapGet("/", () => "FlyingDarts Game API is running! Visit /swagger for API documentation.");

// Log startup information
app.Logger.LogInformation("FlyingDarts Game API starting up...");
app.Logger.LogInformation("Authress Domain: {Domain}", 
    builder.Configuration.GetSection("Authress")["CustomDomain"]);

try
{
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application terminated unexpectedly");
    throw;
} 