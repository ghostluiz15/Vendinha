
using Microsoft.EntityFrameworkCore;
using Vendinha.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos Controllers
builder.Services.AddControllers();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do banco SQLite
builder.Services.AddDbContext<VendinhaDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("ConexaoVendinha")
    )
);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
