using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ResultsService.Clients;
using ResultsService.Configuration;
using ResultsService.Data;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "FrontendCorsPolicy";

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Results API", Version = "v1" });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT Bearer token"
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document, null)] = new List<string>()
    });
});

var resultsConnection = builder.Configuration.GetConnectionString("ResultsDb")
                        ?? throw new InvalidOperationException("Connection string 'ResultsDb' is missing.");
builder.Services.AddDbContext<ResultsDbContext>(options => options.UseNpgsql(resultsConnection));

var authService = builder.Configuration.GetSection("AuthService").Get<AuthServiceOptions>() ?? throw new InvalidOperationException("AuthService settings are missing.");
builder.Services.AddSingleton(authService);
builder.Services.AddHttpClient<AuthUsersClient>(client => AuthUsersClient.ConfigureHttpClient(client, authService));

var rabbitMq = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? throw new InvalidOperationException("RabbitMq settings are missing.");
builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((_, rabbitCfg) =>
    {
        var virtualHost = rabbitMq.VirtualHost.Trim('/');
        var hostUri = string.IsNullOrEmpty(virtualHost)
            ? $"rabbitmq://{rabbitMq.Host}:{rabbitMq.Port}/"
            : $"rabbitmq://{rabbitMq.Host}:{rabbitMq.Port}/{virtualHost}";

        rabbitCfg.Host(new Uri(hostUri), host =>
        {
            host.Username(rabbitMq.Username);
            host.Password(rabbitMq.Password);
        });

        rabbitCfg.Message<ResultRecorded>(messageCfg => messageCfg.SetEntityName("results.recorded"));
    });
});

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt settings are missing.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ResultsDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireCors(FrontendCorsPolicy);

app.Run();
