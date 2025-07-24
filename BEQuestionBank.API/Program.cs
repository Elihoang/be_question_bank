using BEQuestionBank.Core.Configurations;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // ✅ Cần có để Swagger hiển thị API
builder.Services.AddEndpointsApiExplorer(); // ✅ Cho minimal APIs
builder.Services.AddSwaggerGen(); // ✅ Cấu hình Swagger

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ✅ Bắt buộc
    app.UseSwaggerUI(); // ✅ Bắt buộc
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 

app.Run();