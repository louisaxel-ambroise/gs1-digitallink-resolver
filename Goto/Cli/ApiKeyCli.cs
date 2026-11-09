using Goto.Services.Data;
using Goto.Services.Data.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Goto.Cli;

public class ApiKeyCli(Context context)
{
    public void Run(string[] parameters)
    {
        var command = parameters.FirstOrDefault();

        switch (command)
        {
            case "list" when parameters.Length == 1:
                HandleListCommand();
                break;
            case "create" when parameters.Length == 3:
                HandleCreateCommand(parameters[1], parameters[2]);
                break;
            case "revoke" when parameters.Length == 2:
                HandleRevokeCommand(parameters[1]);
                break;
            case "rotate" when parameters.Length == 2:
                HandleRotateCommand(parameters[1]);
                break;
            default:
                Help();
                break;
        }
    }

    private static void Help()
    {
        Console.WriteLine("""
            API Key Management

            Commands:

              apikey list
              apikey create [name] [companyPrefix]
              apikey revoke [name]
              apikey rotate [name]
            """);
    }

    private void HandleListCommand()
    {
        context.ListApiKeys().ForEach(apiKey =>
        {
            Console.WriteLine($"""
            API Key: {apiKey.Name}
              Company Prefix: {apiKey.CompanyPrefix}
              Creation Date:  {apiKey.BeginValidityDate:dd/MM/yyyy HH:mm}

            """);
        });
    }

    private void HandleRevokeCommand(string name)
    {
        var key = context.GetApiKey(name);

        if(key == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"API key not found: {name}");
            Console.ResetColor();
            return;
        }

        key.Disable(TimeProvider.System.GetUtcNow());
        context.SaveChanges();

        Console.WriteLine($"Deleted API key: {name}");
    }

    private void HandleCreateCommand(string name, string companyPrefix)
    {
        try
        {
            var secret = GenerateKey(name, companyPrefix);

            Console.Write("Created API key. Secret: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(secret);
            Console.ResetColor();
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to create API key");
            Console.ResetColor();
        }
    }

    private void HandleRotateCommand(string name)
    {
        var key = context.GetApiKey(name);

        if (key == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"API key not found: {name}");
            Console.ResetColor();
            return;
        }

        try
        {
            key.Disable(TimeProvider.System.GetUtcNow());
            var secret = GenerateKey(name, key.CompanyPrefix);

            Console.Write("Rotated API key. New secret: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(secret);
            Console.ResetColor();
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to rotate API key");
            Console.ResetColor();
        }
    }

    private string GenerateKey(string name, string companyPrefix)
    {
        var random = new Random(name.GetHashCode());
        var secret = random.GetHexString(32, true);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(secret));

        var apiKey = new ApiKey
        {
            Id = Convert.ToBase64String(hash),
            Name = name,
            CompanyPrefix = companyPrefix,
            BeginValidityDate = TimeProvider.System.GetUtcNow(),
            EndValidityDate = DateTime.MaxValue
        };

        context.Add(apiKey);
        context.SaveChanges();

        return secret;
    }
}
