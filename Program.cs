using MineStudio.Agent.Hubs;
using MineStudio.Agent.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ServerProcessService>();
builder.Services.AddSingleton<ServerPropertiesService>();

builder.Services.AddSignalR();

builder.Services.AddHttpClient();
builder.Services.AddScoped<ServerCreationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
// app.UseHttpsRedirection();

app.UseCors("Frontend");

// Controllers
app.MapControllers();

// SignalR
app.MapHub<MinecraftConsoleHub>("/hubs/minecraft-console");

app.Run();