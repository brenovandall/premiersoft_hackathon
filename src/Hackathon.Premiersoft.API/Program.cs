using Hackathon.Premiersoft.API;
using Hackathon.Premiersoft.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);
builder.Services.AddSingleton<S3Service>();

var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
