using Goto.Services;
using Goto.Services.Data;
using Goto.Services.Data.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Goto.Cli;

public sealed class ApiKeyCli(Context context, Clock clock)
{
    public void Run(string[] parameters)
    {
        var command = parameters.FirstOrDefault(string.Empty);

        switch (command.ToLower())
        {
            case "list" when parameters.Length == 1:
                HandleListCommand();
                break;
            case "create" when parameters.Length >= 3:
                HandleCreateCommand(parameters[1], parameters[2], parameters[3..]);
                break;
            case "revoke" when parameters.Length == 2:
                HandleRevokeCommand(parameters[1]);
                break;
            case "rotate" when parameters.Length == 2:
                HandleRotateCommand(parameters[1]);
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("""
            API Key Management

            Commands:

              apikey list
              apikey create [name] [companyPrefix]
              apikey rotate [name]
              apikey revoke [name]
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

        key.Disable(clock.UtcNow);
        context.SaveChanges();

        Console.WriteLine($"Deleted API key: {name}");
    }

    private void HandleCreateCommand(string name, string companyPrefix, string[] remaining)
    {
        try
        {
            string secret = remaining.Length > 0 ? StoreKey(name, companyPrefix, remaining[0]) : GenerateKey(name, companyPrefix);

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

        return StoreKey(name, companyPrefix, secret);
    }

    private string StoreKey(string name, string companyPrefix, string secret)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(secret));

        var apiKey = new ApiKey
        {
            Id = Convert.ToBase64String(hash),
            Name = name,
            CompanyPrefix = companyPrefix,
            BeginValidityDate = clock.UtcNow,
            EndValidityDate = DateTime.MaxValue
        };

        context.Add(apiKey);
        context.SaveChanges();

        return secret;
    }
}
