using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.api.Abstractions;
using sicoain.shared.Entities;
using sicoain.api.Repositories;
using sicoain.api.Data.Seeders;

var builder = WebApplication.CreateBuilder(args);

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
})
.AddMvc();

// ========== SERVICES CONFIGURATION ==========

// Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity with strong password requirements
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Password settings (strict)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings (security)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication Settings
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettingsSection["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured."));

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
        ValidIssuer = jwtSettingsSection["Issuer"],
        ValidAudience = jwtSettingsSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };

    // Read token from HttpOnly cookie (not from Authorization header)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// Anti-forgery for CSRF protection
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "csrf_token";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Authorization
builder.Services.AddAuthorization();

#pragma warning disable ASP0000
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    try
    {
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var permissions = dbContext.Permissions.Select(p => p.Name).ToList();
            foreach (var perm in permissions)
            {
                builder.Services.AddAuthorizationBuilder()
                    .AddPolicy(perm, policy => policy.RequireClaim("Permission", perm));
            }
        }
    }
    catch
    {
        // Handle exceptions (e.g., database not available during startup)
        Console.WriteLine("Warning: Could not load permissions from database. Authorization policies will not be registered.");
    }
#pragma warning restore ASP0000

// CORS - restrict to specific origins only
builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictCors", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ========== SWAGGER CONFIGURATION (Swashbuckle) ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SICOAIN API",
        Version = "v1",
        Description = "API for Occupational Accident Management System"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGciOiJIUzI1NiIs..."
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

// Services for application logic (e.g., UserService, AccidentService) would be registered here
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ICookieManager, CookieManager>();
builder.Services.AddScoped<IIpAddressProvider, IpAddressProvider>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IRoleSyncService, RoleSyncService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAccidentService, AccidentService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<ICorrectiveActionService, CorrectiveActionService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDigitalEvidenceService, DigitalEvidenceService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEventCategoryService, EventCategoryService>();
builder.Services.AddScoped<IHealthPromotionEntityService, HealthPromotionEntityService>();
builder.Services.AddScoped<IOccupationalRiskAdministratorService, OccupationalRiskAdministratorService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IRiskClassService, RiskClassService>();
builder.Services.AddScoped<IWitnessService, WitnessService>();


// HttpContextAccessor (Necessary for CookieManager and IpAddressProvider
builder.Services.AddHttpContextAccessor();

// Controllers
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// ========== HTTP PIPELINE CONFIGURATION ==========

// Swagger (only in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SICOAIN API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

// CORS must come before Authentication and Authorization
app.UseCors("StrictCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Running Seeder");
    var services = scope.ServiceProvider;
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    /* Create roles in Identity if they don't exist */
    await RoleSeeder.SeedAsync(services).ConfigureAwait(false);
    Console.WriteLine("Base roles seed in Identity");

    /* Synchronize Identity -> CustomRoles */
    var roleSyncService = services.GetRequiredService<IRoleSyncService>();
    await roleSyncService.SynchronizeRoleAsync().ConfigureAwait(false);
    Console.WriteLine("Synchronized Roles");

    /* Seed permissions */
    await PermissionSeeder.SeedAsync(dbContext).ConfigureAwait(false);
    Console.WriteLine("Seeded Permissions");

    /* Asign permissions to roles (in RolePermissions) */
    await RolePermissionSeeder.SeedAsync(dbContext).ConfigureAwait(false);
    Console.WriteLine("Role-Permission assignments completed");
}

app.Run();
