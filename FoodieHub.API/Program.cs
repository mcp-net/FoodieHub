using AutoMapper;
using FoodieHub.API.Authentication;
using FoodieHub.API.Data;
using FoodieHub.API.Mappings;
using FoodieHub.API.Middlewares;
using FoodieHub.API.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;
using ExceptionHandlerMiddleware = FoodieHub.API.Middlewares.ExceptionHandlerMiddleware;

var builder = WebApplication.CreateBuilder(args);

//–– Serilog
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/FoodieHub_Log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//–– Controllers & HTTP Context
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

//–– Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodieHub API", Version = "v1" });
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste your JWE token here (no 'Bearer ' prefix)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWE"
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

//–– EF Core
builder.Services.AddDbContext<FoodieHubDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("FoodieHubConnectionString")));
builder.Services.AddDbContext<FoodieHubAuthDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("FoodieHubAuthConnectionString")));

//–– Repositories & AutoMapper
builder.Services.AddScoped<ICityRepository, SqlCityRepository>();
builder.Services.AddScoped<IRestaurantRepository, SqlRestaurantRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();


builder.Services.AddScoped<IPriceRangeRepository, SqlPriceRangeRepository>();



builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfiles>());

//–– Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("FoodieHub")
    .AddEntityFrameworkStores<FoodieHubAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequiredLength = 6;
    opts.Password.RequiredUniqueChars = 1;
});

//–– JWE Authentication Scheme
builder.Services
    .AddAuthentication(JweAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, JweAuthenticationHandler>(
        JweAuthenticationDefaults.AuthenticationScheme, _ => { });

var app = builder.Build();

//–– Exception handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

//–– HTTPS + AuthN/Z
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//–– Static files & controllers
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});
app.MapControllers();

app.Run();














////////////using AutoMapper;
////////////using FoodieHub.API.Data;
////////////using FoodieHub.API.Mappings;
////////////using FoodieHub.API.Middlewares;
////////////using FoodieHub.API.Repositories;
////////////using Jose;
////////////using Microsoft.AspNetCore.Authentication.JwtBearer;
////////////using Microsoft.AspNetCore.Diagnostics;
////////////using Microsoft.AspNetCore.Identity;
////////////using Microsoft.EntityFrameworkCore;
////////////using Microsoft.Extensions.DependencyInjection;
////////////using Microsoft.Extensions.FileProviders;
////////////using Microsoft.IdentityModel.Tokens;
////////////using Microsoft.OpenApi.Models;
////////////using Serilog;
////////////using System.Security.Claims;
////////////using System.Text;
////////////using System.Text.Json;

////////////var builder = WebApplication.CreateBuilder(args);

////////////// Configure Serilog
////////////var logger = new LoggerConfiguration()
////////////    .WriteTo.Console()
////////////    .WriteTo.File("Logs/FoodieHub_Log.txt", rollingInterval: RollingInterval.Day)
////////////    .MinimumLevel.Information()
////////////    .CreateLogger();

////////////builder.Logging.ClearProviders();
////////////builder.Logging.AddSerilog(logger);

////////////// Add services to the container.
////////////builder.Services.AddControllers();
////////////builder.Services.AddHttpContextAccessor();

////////////// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
////////////builder.Services.AddEndpointsApiExplorer();
////////////builder.Services.AddSwaggerGen(options =>
////////////{
////////////    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodieHub API", Version = "v1" });

////////////    //old JWT Authentication
////////////    //options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
////////////    //{
////////////    //    Name = "Authorization",
////////////    //    In = ParameterLocation.Header,
////////////    //    Type = SecuritySchemeType.ApiKey,
////////////    //    Scheme = JwtBearerDefaults.AuthenticationScheme
////////////    //});

////////////    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
////////////    //{
////////////    //    {
////////////    //        new OpenApiSecurityScheme
////////////    //        {
////////////    //            Reference = new OpenApiReference
////////////    //            {
////////////    //                Type = ReferenceType.SecurityScheme,
////////////    //                Id = JwtBearerDefaults.AuthenticationScheme
////////////    //            },
////////////    //            Scheme = "OAuth2",
////////////    //            Name = JwtBearerDefaults.AuthenticationScheme,
////////////    //            In = ParameterLocation.Header
////////////    //        },
////////////    //        new List<string>()
////////////    //    }
////////////    //});


////////////    // JWE Authentication
////////////    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////////////    {
////////////        Description = "Paste your JWE token here (no 'Bearer' prefix needed)",
////////////        Name = "Authorization",
////////////        In = ParameterLocation.Header,
////////////        Type = SecuritySchemeType.Http,
////////////        Scheme = "bearer",
////////////        BearerFormat = "JWE"
////////////    });

////////////    options.AddSecurityRequirement(new OpenApiSecurityRequirement
////////////    {
////////////        {
////////////            new OpenApiSecurityScheme
////////////            {
////////////                Reference = new OpenApiReference
////////////                {
////////////                    Type = ReferenceType.SecurityScheme,
////////////                    Id = "Bearer"
////////////                }
////////////            },
////////////            new List<string>()
////////////        }
////////////    });


////////////});

////////////// Database connections
////////////builder.Services.AddDbContext<FoodieHubDbContext>(options =>
////////////    options.UseSqlServer(builder.Configuration.GetConnectionString("FoodieHubConnectionString")));

////////////builder.Services.AddDbContext<FoodieHubAuthDbContext>(options =>
////////////    options.UseSqlServer(builder.Configuration.GetConnectionString("FoodieHubAuthConnectionString")));

////////////// Repository registration
////////////builder.Services.AddScoped<ICityRepository, SqlCityRepository>();
////////////builder.Services.AddScoped<IRestaurantRepository, SqlRestaurantRepository>();
////////////builder.Services.AddScoped<ITokenRepository, TokenRepository>();
////////////builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

////////////// AutoMapper
//////////////builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

////////////builder.Services.AddAutoMapper(cfg => {
////////////    cfg.AddProfile<AutoMapperProfiles>();
////////////});




////////////// Identity
////////////builder.Services.AddIdentityCore<IdentityUser>()
////////////    .AddRoles<IdentityRole>()
////////////    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("FoodieHub")
////////////    .AddEntityFrameworkStores<FoodieHubAuthDbContext>()
////////////    .AddDefaultTokenProviders();

////////////builder.Services.Configure<IdentityOptions>(options =>
////////////{
////////////    options.Password.RequireDigit = false;
////////////    options.Password.RequireLowercase = false;
////////////    options.Password.RequireNonAlphanumeric = false;
////////////    options.Password.RequireUppercase = false;
////////////    options.Password.RequiredLength = 6;
////////////    options.Password.RequiredUniqueChars = 1;
////////////});

////////////// JWT Authentication
////////////////////builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
////////////////////    .AddJwtBearer(options =>
////////////////////    {
////////////////////        options.TokenValidationParameters = new TokenValidationParameters
////////////////////        {
////////////////////            ValidateIssuer = true,
////////////////////            ValidateAudience = true,
////////////////////            ValidateLifetime = true,
////////////////////            ValidateIssuerSigningKey = true,
////////////////////            ValidIssuer = builder.Configuration["JWT:Issuer"],
////////////////////            ValidAudience = builder.Configuration["JWT:Audience"],
////////////////////            IssuerSigningKey = new SymmetricSecurityKey(
////////////////////                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
////////////////////        };
////////////////////    });

////////////////////var app = builder.Build();


////////////app.Use(async (context, next) =>
////////////{
////////////    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

////////////    if (!string.IsNullOrEmpty(token))
////////////    {
////////////        try
////////////        {
////////////            var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
////////////            var json = JWT.Decode(token, key, JweAlgorithm.DIR, JweEncryption.A256GCM);
////////////            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

////////////            var identity = new ClaimsIdentity("JWE");
////////////            identity.AddClaim(new Claim(ClaimTypes.Email, claims["email"].ToString()));
////////////            foreach (var role in (JsonElement)claims["roles"])
////////////            {
////////////                identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()));
////////////            }

////////////            var principal = new ClaimsPrincipal(identity);
////////////            context.User = principal;
////////////        }
////////////        catch
////////////        {
////////////            // Invalid token
////////////        }
////////////    }

////////////    await next();
////////////});


////////////// Configure the HTTP request pipeline.
////////////if (app.Environment.IsDevelopment())
////////////{
////////////    app.UseSwagger();
////////////    app.UseSwaggerUI();
////////////}

////////////app.UseMiddleware<FoodieHub.API.Middlewares.ExceptionHandlerMiddleware>();

////////////app.UseHttpsRedirection();

////////////app.UseAuthentication();
////////////app.UseAuthorization();

////////////app.UseStaticFiles(new StaticFileOptions
////////////{
////////////    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
////////////    RequestPath = "/Images"
////////////});

////////////app.MapControllers();

////////////app.Run();