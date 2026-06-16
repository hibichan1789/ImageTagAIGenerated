using ImageTagApi.Context;
using ImageTagApi.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using ImageTagApi.Services.Files;
using ImageTagApi.Services.Queue;
using ImageTagApi.Services.Ai;
using ImageTagApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();


// EF Core
builder.Services.AddDbContext<MyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5173", "https://green-sky-0aeb51b00.7.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
              
    });
});

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Swagger / OpenAPI
// swaggerがAPIエンドポイントを検出するために必要
builder.Services.AddEndpointsApiExplorer();
// SwaggerUIを生成する
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Image Tag API",
        Version = "v1",
        Description = "Image Tag API with JWT Authentication"
    });


    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Please enter token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        }
    );

    
});


// Custom Service
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IFileService, FileService>();
// Repository
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICssStyleRepository, CssStyleRepository>();
builder.Services.AddScoped<IFileTagRepository, FileTagRepository>();
// Azure Funcstions HttpTrigger
builder.Services.AddHttpClient<IAiFunctionService, OpenAiFunctionService>();
// Custom Storage Service
var storageType = builder.Configuration["Storage:Type"]!;
if(storageType == "Local")
{
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
}
else if(storageType == "Azure")
{
    builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
}
// Custom Queue Service
var queueType = builder.Configuration["Queue:Type"]!;
if(queueType == "Local")
{
    builder.Services.AddScoped<IQueueService, LocalQueueService>();
}
else if(queueType == "Azure")
{
    builder.Services.AddScoped<IQueueService, AzureQueueService>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
}


// Swagger UI(開発のみ)
//if (app.Environment.IsDevelopment())
//{
    // Swaggerをブラウザに表示する
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseRouting();

// CORS
app.UseCors("AllowFrontend");

//認証・認可
app.UseAuthentication();
app.UseAuthorization();

// Controllerのルーティングを有効化
app.MapControllers();

app.Run();