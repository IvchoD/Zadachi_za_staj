using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// ==========================
// Services
// ==========================

builder.Services.AddControllers();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ==========================
// SQLite Database
// ==========================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=../database/baba_strinka.db"));

var app = builder.Build();

app.UseCors("Frontend");
// ==========================
// Middleware
// ==========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseStaticFiles();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    DatabaseSeeder.Seed(context);
}

app.Run();