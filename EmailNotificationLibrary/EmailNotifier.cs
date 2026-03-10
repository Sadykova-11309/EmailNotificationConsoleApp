using MailKit.Net.Smtp;
using MimeKit;

namespace EmailNotificationLibrary
{
    public class EmailNotifier
    {
        /// <summary>
        /// This is the core class in the library responsible for email notification logic.
        /// It contains an asynchronous method to send HTML emails to a list of users via an SMTP client,
        /// incorporating a delay between sends.
        /// It uses the MailKit library for email composition and transmission.
        /// </summary>
  
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
            //Iterates over each user. Replaces the {Name} placeholder in the template with
            //the user's name (or "Пользователь" if null) to create a personalized body.

            foreach (var user in users)
            {
                string body = htmlTemplate.Replace("{Name}", user.Name ?? "Пользователь");

                //Creates a new MimeMessage object using MailKit.

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", smtpEmail));
                message.To.Add(new MailboxAddress("", user.Email));
                message.Subject = subject;

                message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

                //Creates and disposes an SmtpClient instance per email.
                //Connects to the SMTP server using StartTls for security,
                //authenticates with credentials, sends the message, disconnects,
                //and delays before the next iteration.

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
