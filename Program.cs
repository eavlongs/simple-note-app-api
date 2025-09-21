using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using simple_note_app_api;
using simple_note_app_api.Migrations;
using simple_note_app_api.Repository;
using simple_note_app_api.Services;
using simple_note_app_api.Settings;
using System.Data;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new Exception("JWT Settings are not configured properly.");
}

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>

    options.InvalidModelStateResponseFactory = context =>
    {
        var namingPolicy = JsonNamingPolicy.CamelCase;

        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => namingPolicy.ConvertName(kvp.Key),
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return ResponseBuilder.ValidationError(errors, "Validation failed");
    }
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = false,
        ValidateAudience = false
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                error = "unauthorized",
                message = "You are not authorized to access this resource"
            });

            return context.Response.WriteAsync(result);
        }
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRespository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<INoteService, NoteService>();

SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());

WebApplication app;

var cliHelper = new CLIHelper(args);
var migrationArg = cliHelper.GetMigrationArg();

if (migrationArg != MigrationArg.NoMigration)
{
    builder.Services.AddFluentMigratorCore().ConfigureRunner(rb => rb.AddSqlServer2016()
    .WithGlobalConnectionString(builder.Configuration.GetConnectionString(Constants.DB_CONNECTION))
    .ScanIn(typeof(Program).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());
    app = builder.Build();


    using (var scope = app.Services.CreateScope())
    {
        var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        var migrationService = new MigrationService(migrationRunner);
        migrationService.MigrateDatabase();
    }

    if (migrationArg == MigrationArg.MigrateOnly)
    {
        return;
    }
}
else
{
    app = builder.Build();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
