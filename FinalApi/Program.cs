using Final;
using Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<FinalDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddDbContext<FinalDbContext>(
    c => { c.UseSqlServer(builder.Configuration.GetConnectionString("LocalConn"))
        .UseLazyLoadingProxies(); }
);
builder.Services.AddControllers();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


//register your manager hereeee
builder.Services.AddScoped<ChatManager>();
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<MessageManager>();
builder.Services.AddScoped<TokenManager>();
builder.Services.AddScoped<SellerManager>();
builder.Services.AddScoped<ItemManager>();
builder.Services.AddScoped<AuctionManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
