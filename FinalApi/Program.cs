using E_commerce;
using Final;
using FinalApi;
using Hangfire;
using Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<FinalDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddDbContext<FinalDbContext>(
    c => { c.UseSqlServer(builder.Configuration.GetConnectionString("LocalConn"))
        .UseLazyLoadingProxies(); },ServiceLifetime.Scoped
);
builder.Services.AddControllers();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("LocalConn")));

builder.Services.AddHangfireServer();

//register your manager hereeee
builder.Services.AddSignalR();
builder.Services.AddScoped<HangfireManager>();
builder.Services.AddScoped<ChatManager>();
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<MessageManager>();
builder.Services.AddScoped<TokenManager>();
builder.Services.AddScoped<SellerManager>();
builder.Services.AddScoped<ItemManager>();
builder.Services.AddScoped<AuctionManager>();
builder.Services.AddScoped<PaymentManager>();
builder.Services.AddScoped<BidManager>();
builder.Services.AddScoped<BuyerManager>();
builder.Services.AddScoped<CategoryManager>();
builder.Services.AddScoped<BuyerManager>();
builder.Services.AddScoped<ComplainManager>();  // √÷› Â–« «·”ÿ—
builder.Services.AddScoped<ProfileManager>();
builder.Services.AddScoped<ReviewManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateActor = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

builder.Services.AddCors(i => i.AddDefaultPolicy(
    i => i.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseAuthorization();
app.UseCors();
app.UseHangfireDashboard("/hangfire");
app.MapHub<BidsHub>("/BidsHub");
app.MapControllers();

app.Run();
