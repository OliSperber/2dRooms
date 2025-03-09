using _2dRooms.Repositories;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEnvironment2DRepository, Environment2DRepository>();
builder.Services.AddScoped<IObject2DRepository, Object2DRepository>();


// Read SQL connection string
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);
var jwtSecret = builder.Configuration.GetValue<string>("SecretJwtKey");

// Set up Identity and Dapper stores for IdentityUser
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddDapperStores(options =>
    {
        options.ConnectionString = sqlConnectionString;
    });

// Configure Bearer Token options
builder.Services
    .AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme)
    .Configure(options =>
    {
        options.BearerTokenExpiration = TimeSpan.FromMinutes(60);
    });

// Add authentication with JWT Bearer token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"]; // Authority URL
        options.Audience = builder.Configuration["Jwt:Audience"];   // Audience
        options.RequireHttpsMetadata = true;

        // Token validation parameters
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, // Make sure this is set to true

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Fetch the key from your database or environment variable
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

            // Optional: Define the clock skew (tolerance for expiration times)
            ClockSkew = TimeSpan.Zero // This removes any grace period for token expiration
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userClaims = context.Principal?.Claims;
                Console.WriteLine("Token validated successfully, claims: " + string.Join(", ", userClaims.Select(c => c.Value)));
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection and authorization middleware
app.UseHttpsRedirection();
app.UseAuthentication(); // Enable Bearer token authentication
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Set up API routes and authentication
app.MapGroup("/account")
    .MapIdentityApi<IdentityUser>();

// Create a custom login endpoint to generate JWT token
app.MapPost("/account/login/jwt", async (SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, [FromBody] LoginModel loginModel) =>
{
    // Step 1: Authenticate the user with username/email and password
    var user = await userManager.FindByEmailAsync(loginModel.Email);
    if (user == null || !(await signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false)).Succeeded)
    {
        return Results.Unauthorized();
    }

    // Step 2: Create the JWT Token
    var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        // Add additional claims here (e.g., roles, permissions, etc.)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)); // Secret key for signing
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: builder.Configuration["Jwt:Issuer"], // Issuer
        audience: builder.Configuration["Jwt:Audience"], // Audience
        claims: claims,
        expires: DateTime.Now.AddMinutes(60), // Set expiration time
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    // Step 3: Return the generated JWT token
    return Results.Ok(new { token = tokenString });
});

// Log out functionality with Bearer token
app.MapPost("/account/logout", async (SignInManager<IdentityUser> signInManager, [FromBody] object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
}).RequireAuthorization();

app.MapControllers().RequireAuthorization();

app.MapGet("/", () => $"The API is up and running. Connection string found: {(sqlConnectionStringFound ? "True " : "False")}");

app.Run();

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
