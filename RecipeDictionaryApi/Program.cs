using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RecipeDictionaryApi.Middleware;
using RecipeDictionaryApi.Services;
using RecipeDictionaryApi.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("Yarp"));

builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserStorage, UserDbStorage>();
builder.Services.AddScoped<IDishStorage, DishDbStorage>();
builder.Services.AddScoped<IRecipeStorage, RecipeDbStorage>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

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
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c is { Type: "Admin", Value: "true" })));

builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.WebHost.UseUrls("http://*:80");
var app = builder.Build();

if (args.Contains("--migrate"))
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при применении миграций");
        throw;
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandler>();

app.Use(async (context, next) =>
{
    var podName = Environment.GetEnvironmentVariable("POD_NAME") ?? 
                  Environment.MachineName ?? 
                  Guid.NewGuid().ToString("N")[..8];
    
    context.Response.Headers.Append("X-Backend-Instance", podName);
    await next();
});

app.MapReverseProxy();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
