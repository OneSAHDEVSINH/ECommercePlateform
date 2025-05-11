using ECommercePlateform.Server.Core.Application.Interfaces;
using ECommercePlateform.Server.Core.Application.Mappings;
using ECommercePlateform.Server.Core.Application.Services;
using ECommercePlateform.Server.Infrastructure.Persistence;
using ECommercePlateform.Server.Infrastructure.Persistence.Repositories;
using ECommercePlateform.Server.Presentation.Middleware;
using ECommercePlateform.Server.Presentation.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Reflection;
using System.IO;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => 
{
    options.Filters.Add<ValidationFilter>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressConsumesConstraintForFormFileParameters = true;
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressModelStateInvalidFilter = true;
    options.SuppressMapClientErrors = true;
    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
        "https://httpstatuses.com/404";
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register services
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = true;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "MyTemporarySecretKeyForDevelopment12345")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    // Set more descriptive documentation failure behavior
    c.DescribeAllParametersInCamelCase();
    c.EnableAnnotations();
    
    try {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath, true);
        }
        else
        {
            Console.WriteLine($"XML documentation file not found at: {xmlPath}");
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"Error loading XML comments: {ex.Message}");
    }
    
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce API",
        Version = "v1",
        Description = "A clean architecture based e-commerce API"
    });
    
    // Handle enums as strings
    c.UseOneOfForPolymorphism();
    c.SelectDiscriminatorNameUsing(type => "discriminator");
    c.SchemaFilter<EnumSchemaFilter>();
    
    // Prevent circular references
    c.CustomSchemaIds(type => type.FullName);
    
    // Ignore cycling references
    c.DocInclusionPredicate((name, api) => true);
    
    // Add bearer authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add JSON options to handle reference loops globally
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        // Enable migration warning suppression for development only
        if (app.Environment.IsDevelopment())
        {
            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5)); // Allow more time for migrations
        }
        
        dbContext.Database.Migrate(); // This will apply any pending migrations and create the database if it doesn't exist
        Console.WriteLine("Database migrated successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Use the Swagger exception middleware before Swagger
    app.UseSwaggerExceptionMiddleware();
    
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api-docs/{documentName}/swagger.json";
    });
    
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api-docs/v1/swagger.json", "ECommerce API v1");
        c.RoutePrefix = "swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
        c.ConfigObject.AdditionalItems.Add("syntaxHighlight", false); // Helps with performance
        c.ConfigObject.AdditionalItems.Add("tryItOutEnabled", true);
    });
}
else
{
    app.UseHsts();
}

// Use custom middleware
app.UseCustomExceptionMiddleware();
app.UseCustomValidationMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run(); 