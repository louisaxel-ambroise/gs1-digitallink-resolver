using Goto;
using Goto.Cli;
using Goto.Infrastructure.Authentication;
using Goto.Services.Data;

var builder = WebApplication
    .CreateBuilder(args)
    .RegisterApiServices()
    .RegisterDigitalLinkToolkit()
    .RegisterDigitalLinkResolver();

var app = builder.Build();

if (builder.Environment.IsDevelopment() || args.Contains("--bootstrap"))
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<Context>();
    context.Database.EnsureCreated();

    Console.WriteLine("Database created");
}

if (args.Length > 0 && args[0] == "apikey")
{
    using var scope = app.Services.CreateScope();
    var cli = scope.ServiceProvider.GetRequiredService<ApiKeyCli>();
    
    cli.Run([.. args.Skip(1)]);
}
else
{
    app.UseCors();
    app.UseAuthorization();
    app.MapStaticAssets();
    app.UseRequestLocalization();
    app.UseExceptionHandler("/error");
    app.MapControllers();

    app.Run();
}