using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddLogging();

builder.Services
    .AddPersistence()
    .AddOpenSearch();

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.WithOrigins("http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              ;
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}


app.UseRouting();

app.UseCors("cors");


app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
