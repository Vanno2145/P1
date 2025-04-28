using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailSenderApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Консольное приложение для отправки email");

            
            Console.Write("Введите адрес электронной почты отправителя: ");
            string senderEmail = Console.ReadLine();
            Console.Write("Введите пароль отправителя: ");
            string senderPassword = Console.ReadLine();

            
            Console.Write("Введите адрес SMTP сервера (например, smtp.gmail.com): ");
            string smtpServer = Console.ReadLine();
            Console.Write("Введите порт SMTP сервера (например, 587 для TLS, 465 для SSL): ");
            if (!int.TryParse(Console.ReadLine(), out int smtpPort))
            {
                Console.WriteLine("Некорректный порт. Завершение работы.");
                return;
            }
            Console.Write("Использовать SSL/TLS (true/false)? ");
            if (!bool.TryParse(Console.ReadLine(), out bool useSsl))
            {
                Console.WriteLine("Некорректный ввод. Завершение работы.");
                return;
            }

          
            Console.Write("Введите тему письма: ");
            string subject = Console.ReadLine();

           
            Console.WriteLine("Введите текст письма:");
            string body = Console.ReadLine();

           
            Console.Write("Введите список получателей (через запятую): ");
            string recipientsInput = Console.ReadLine();
            List<string> recipients = recipientsInput.Split(',').Select(r => r.Trim()).ToList();

            
            try
            {
                await SendEmailAsync(senderEmail, senderPassword, smtpServer, smtpPort, useSsl, subject, body, recipients);
                Console.WriteLine("Письма успешно отправлены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при отправке: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static async Task SendEmailAsync(string senderEmail, string senderPassword, string smtpServer, int smtpPort, bool useSsl, string subject, string body, List<string> recipients)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", senderEmail));
            foreach (var recipient in recipients)
            {
                message.To.Add(new MailboxAddress("", recipient));
            }
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, useSsl);

             
                await client.AuthenticateAsync(senderEmail, senderPassword);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}
