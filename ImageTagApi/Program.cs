using ImageTagApi.Context;
using ImageTagApi.Services.Auth;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();


// EF Core
builder.Services.AddDbContext<MyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// Swagger / OpenAPI
// swaggerがAPIエンドポイントを検出するために必要
builder.Services.AddEndpointsApiExplorer();
// SwaggerUIを生成する
builder.Services.AddSwaggerGen();


// Custom Service
builder.Services.AddScoped<IRegisterService, RegisterService>();


var app = builder.Build();

// Swagger UI(開発のみ)
if (app.Environment.IsDevelopment())
{
    // Swaggerをブラウザに表示する
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Controllerのルーティングを有効化
app.MapControllers();

app.Run();