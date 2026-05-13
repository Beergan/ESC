using Microsoft.Extensions.Options;
using MimeKit;
using ESC.CONCOST.Abstract;
using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Generic;

namespace ESC.CONCOST.Base
{
    public class MailSettingService : IMailSettingService
    {
        private readonly MailSettings _mailSettings;
        public MailSettingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMail(params MailRequest[] mails)
        {
            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

                foreach (var mailRequest in mails)
                {
                    try
                    {
                        var email = new MimeMessage();
                        email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                        email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                        email.Subject = mailRequest.Subject;

                        var builder = new BodyBuilder
                        {
                            HtmlBody = mailRequest.Content
                        };

                        //if (mailRequest.Attachments != null)
                        //{
                        //    foreach (var file in mailRequest.Attachments)
                        //    {
                        //        if (file.Length == 0) continue;
                        //        builder.Attachments.Add(file.FileName, file.OpenReadStream(), ContentType.Parse(file.ContentType));
                        //    }
                        //}

                        email.Body = builder.ToMessageBody();
                        await smtp.SendAsync(email);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (smtp.IsConnected)
                    await smtp.DisconnectAsync(true);
            }
        }
    }
}


