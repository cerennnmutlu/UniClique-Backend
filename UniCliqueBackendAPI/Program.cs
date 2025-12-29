using Microsoft.EntityFrameworkCore;
using UniCliqueBackend.Persistence.Contexts;
using UniCliqueBackend.Persistence.Seed;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSql"),
        b => b.MigrationsAssembly("UniCliqueBackend.Persistence")
    );
});



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    if (env.IsDevelopment())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await AppDbSeeder.SeedAsync(dbContext);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

