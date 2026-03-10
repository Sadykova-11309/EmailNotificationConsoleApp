using MailKit.Net.Smtp;
using MimeKit;

namespace EmailNotificationLibrary
{
    public class EmailNotifier
    {
        public async Task SendAsync(
            List<User> users,
            string htmlTemplate,
            string smtpHost,
            int smtpPort,
            string smtpEmail,
            string smtpPassword,
            string subject,
            int delayMs = 1500)
        {
            foreach (var user in users)
            {
                string body = htmlTemplate.Replace("{Name}", user.Name ?? "Пользователь");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", smtpEmail));
                message.To.Add(new MailboxAddress("", user.Email));
                message.Subject = subject;

                message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpEmail, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                await Task.Delay(delayMs);
            }
        }
    }
}
