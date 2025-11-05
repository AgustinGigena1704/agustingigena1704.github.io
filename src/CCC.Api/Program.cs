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
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddScoped<IMiddleware, CustomMiddleware>();
builder.Services.AddScoped<IAuthService, FireBaseService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var serviceAccountPath = Path.Combine(Directory.GetCurrentDirectory(), "Secrets/firebase-proyect-data.json");
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(serviceAccountPath),
    ProjectId = projectId
});

var app = builder.Build();

app.UseMiddleware<IMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();