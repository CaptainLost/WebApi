using Host.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

WebApplication app = builder.Build();

await app.ConfigurePipelineAsync();

app.Run();