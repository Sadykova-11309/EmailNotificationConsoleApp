using EmailNotificationLibrary;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string host = config["Smtp:Host"] ?? "smtp.gmail.com";
        int port = config.GetValue<int>("Smtp:Port", 587);
        string email = config["Smtp:Email"] ?? throw new Exception("Email не указан");
        string password = config["Smtp:Password"] ?? throw new Exception("Password не указан");
        string subject = config["Email:Subject"] ?? "Уведомление";
        int delay = config.GetValue<int>("Email:DelayMs", 1500);

        var users = new List<User>
        {
            new() { Email = "test1@example.com", Name = "Алексей" },
            new() { Email = "test2@example.com", Name = "Мария"   }
        };

        string template = """
            <html>
            <body>
                <h1>Здравствуйте, {Name}!</h1>
                <p>Это уведомление.</p>
            </body>
            </html>
            """;

        var notifier = new EmailNotifier();

        try
        {
            await notifier.SendAsync(users, template, host, port, email, password, subject, delay);
            Console.WriteLine("Отправлено успешно");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }
}