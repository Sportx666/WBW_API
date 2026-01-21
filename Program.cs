using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WBM_API.Controllers;
using WBM_API.WBM_API_DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<wbmdbcontext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("wbmdbConnection")));

// --- Custom JWT Authentication ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "WBMJwt";
    options.DefaultChallengeScheme = "WBMJwt";
})
    .AddJwtBearer("WBMJwt", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    })

    // --- Azure AD Authentication ---
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("Azure"), "Azure")
    // --- mTLS Authentication ---
    .AddCertificate("MTLS", options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.RevocationMode = X509RevocationMode.NoCheck;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                context.Success();
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                context.Fail("Invalid client certificate");
                return Task.CompletedTask;
            }
        };
    });



builder.Services.AddControllers();


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<WBMDatabase>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine("AUTH ERROR: " + ex);
        throw;
    }
});


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.MapPost("broadcast", async (string message, IHubContext<SignalRHub, IChatClientSignalRInterface> context) =>
//{
//    await context.Clients.All.ReceiveMessage("message");

//    return Results.NoContent();
//});

app.MapHub<CarrierSignalR>("/CarrierListExcelReportSignalR");

app.Run();
