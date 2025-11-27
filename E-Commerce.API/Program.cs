using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Mappings;
using E_Commerce.Application.Services;
using E_Commerce.Core.Interfaces;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//kestrel configuration for docker and railway deployment
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080); //railway uses port 8080
});

//add services to the container
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
//cart repo registerartion
builder.Services.AddScoped<ICartRepository, CartRepository>();



// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
//cart repo registration
builder.Services.AddScoped<ICartService, CartService>();

// Register token service
builder.Services.AddScoped<JwtTokenService>();

//postgresql configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//configure jwt
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddAuthorization();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce API",
        Version = "v1",
        Description = "A robust e-commerce backend API with authentication and product management"
    });

    //add jwt auth to swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
            Array.Empty<string>()
        }
    });
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

//apply migrations automativcally on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Apply pending migrations
        context.Database.Migrate();

        Console.WriteLine("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

//HTTP request pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API V1");
        c.RoutePrefix = string.Empty; //Swagger is the root
    });
}

app.UseHttpsRedirection();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            //development: Allow all
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            //production -Specify allowed origins
            policy.WithOrigins(
                "https://yourdomain.com",           // Your frontend
                "https://www.yourdomain.com"        // www version
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        }
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();