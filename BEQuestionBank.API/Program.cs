using BEQuestionBank.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using BEQuestionBank.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // ✅ Cần có để Swagger hiển thị API
builder.Services.AddEndpointsApiExplorer(); // ✅ Cho minimal APIs
builder.Services.AddSwaggerGen(); // ✅ Cấu hình Swagger

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        x => x.MigrationsAssembly("BEQuestionBank.Core") // 👈 Thêm dòng này
    ));

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // ✅ Đúng port React
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // nếu dùng cookie hoặc auth
        });
});


// Gọi extension từ Core
builder.Services.AddCoreServices();

// Bật annotation Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ✅ Bắt buộc
    app.UseSwaggerUI(); // ✅ Bắt buộc
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 

app.Run();