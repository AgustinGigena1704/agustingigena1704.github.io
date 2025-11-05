using CCC.Api;
using CCC.Api.Data;
using CCC.Api.Interfaces;
using CCC.Api.Services;
using CCC.Shared;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using static CCC.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

EntityManager.RegisterAllRepositories(builder.Services, Assembly.GetExecutingAssembly());

builder.Services.AddScoped<EntityManager>();

// Add services to the container.
builder.WebHost.UseSentry(opt =>
{
    opt.SetBeforeSend((@event) =>
    {
        if (@event.Exception is ControledException)
        {
            return null;
        }
        return @event;
    });
});

// ==========================================
// CORS MODIFICADO para GitHub Pages
// ==========================================
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        // Leer orígenes permitidos desde configuración
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? new[]
            {
                "http://localhost:5249",
                "https://localhost:7011"
            };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<IMiddleware, CustomMiddleware>();
builder.Services.AddScoped<IAuthService, FireBaseService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<CCCDbContext>(opt =>
{
    var connectionString = "server=" + DB_HOST + ";port=" + DB_PORT + ";database=" + DB_NAME + ";user=" + DB_USER + ";password=" + DB_PASSWORD + ";SslMode=Required;SslCa=" + DB_SSL_PATH;
    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var projectId = builder.Configuration["Firebase:ProjectId"];
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// ==========================================
// FIREBASE MODIFICADO para leer desde variable de entorno
// ==========================================
string? firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");
GoogleCredential credential;

if (!string.IsNullOrEmpty(firebaseJson))
{
    try
    {
        // Validar que el JSON sea válido
        var jsonObject = System.Text.Json.JsonDocument.Parse(firebaseJson);

        // Producción: desde variable de entorno en Render
        credential = GoogleCredential.FromJson(firebaseJson);
    }
    catch (Exception ex)
    {
        Console.WriteLine("FIREBASE_CONFIG: " + firebaseJson);
        throw new InvalidOperationException("El JSON proporcionado en la variable de entorno FIREBASE_CONFIG no es válido.", ex);
    }
}
else
{
    // Desarrollo: desde archivo local
    var serviceAccountPath = Path.Combine(Directory.GetCurrentDirectory(), "Secrets/firebase-proyect-data.json");
    if (File.Exists(serviceAccountPath))
    {
        try
        {
            var fileContent = File.ReadAllText(serviceAccountPath);
            // Validar que el JSON sea válido
            var jsonObject = System.Text.Json.JsonDocument.Parse(fileContent);

            credential = GoogleCredential.FromJson(fileContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("El archivo de configuración Firebase no es válido.", ex);
        }
    }
    else
    {
        throw new FileNotFoundException("Firebase configuration not found. Set FIREBASE_CONFIG environment variable or add Secrets/firebase-proyect-data.json file.");
    }
}

FirebaseApp.Create(new AppOptions
{
    Credential = credential,
    ProjectId = projectId
});

// ==========================================
// HEALTH CHECKS - Nuevo
// ==========================================
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<IMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Mostrar Swagger también en producción (opcional)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CCC API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint para monitoreo
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();