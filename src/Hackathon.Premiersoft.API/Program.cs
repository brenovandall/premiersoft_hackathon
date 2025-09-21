using Hackathon.Premiersoft.API;
using Hackathon.Premiersoft.API.Data.Extensions;
using Hackathon.Premiersoft.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);
builder.Services.AddSingleton<S3Service>();

// Adicionar CORS de forma mais explícita
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar CORS antes de tudo
app.UseCors("AllowAll");

// Middleware customizado para CORS manual se necessário
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept, Authorization");
    
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        return;
    }
    
    await next();
});

//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.InitialiseDatabseAsync();

app.Run();
