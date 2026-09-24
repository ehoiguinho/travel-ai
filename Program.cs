using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using Pgvector.EntityFrameworkCore;
using TravelAI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector()
    ));

builder.Services.AddHttpClient<EmbeddingService>();
builder.Services.AddHttpClient<GenerationService>();
builder.Services.AddScoped<KnowledgeService>();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();