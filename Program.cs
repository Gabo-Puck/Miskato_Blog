using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Miskato_Blog.Models;
using Miskato_Blog.Services;

var builder = WebApplication.CreateBuilder(args);
var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
var audience = builder.Configuration["Auth0:Audience"];

// Add services to the container.
builder.Services.Configure<MiskatoBlogMongoSettings>(builder.Configuration.GetSection("MiskatoBlogMongo"));
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:post", policy => policy.AddRequirements(new HasScopeRequirement("read:post", domain)));
});

builder.Services.AddSingleton<PostsService>();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
