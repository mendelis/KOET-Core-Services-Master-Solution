using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using KOET.Core.Services.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Neo4jClient;
using System.Reflection;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;

AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
{
    if (eventArgs.Exception is ReflectionTypeLoadException ex)
    {
        foreach (var loaderEx in ex.LoaderExceptions)
        {
            Console.WriteLine(loaderEx?.Message);
        }
    }
};
try
{


    var builder = WebApplication.CreateBuilder(args);

    ///JWT Configuration
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    builder.Services.Configure<JwtSettings>(jwtSettings);

    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

    ///Neo4j Configuration
    var neo4jUri = builder.Configuration["Neo4j:Uri"];
    var neo4jUser = builder.Configuration["Neo4j:Username"];
    var neo4jPass = builder.Configuration["Neo4j:Password"];

    var neo4jClient = new BoltGraphClient(new Uri(neo4jUri), neo4jUser, neo4jPass);
    neo4jClient.ConnectAsync().Wait();
    builder.Services.AddSingleton<IGraphClient>(neo4jClient);

    ///Dependency Injection
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddScoped<ISessionService, SessionService>();
    builder.Services.AddSingleton<IEmailSender, EmailSender>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    ///Middleware Pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var loaderEx in ex.LoaderExceptions)
    {
        Console.WriteLine(loaderEx?.Message);
    }
}
