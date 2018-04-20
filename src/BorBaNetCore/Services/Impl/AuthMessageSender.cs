using BorBaNetCore.Classes;
using BorBaNetCore.Services;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BorBaNetCore.DataModel;

namespace BorBaNetCore.Services.Impl
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public EmailSettings _emailSettings { get; }

        public Task SendEmailAsync(string email, string subject, string message, string name)
        {

            Execute(email, subject, message, name,true).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute(string email, string subject, string message,string name, bool isFirst)
        {
            try
           {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(email,string.Format("{0} <{1}>",name,email));
                mail.Sender = new MailAddress(email, name);
                mail.To.Add(isFirst ? new MailAddress(_emailSettings.ToEmail) : new MailAddress(_emailSettings.ToEmail2));
                mail.CC.Add(new MailAddress(_emailSettings.CcEmail));
                mail.CC.Add(new MailAddress(_emailSettings.CcEmail2));

                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = false;
                mail.Priority = MailPriority.High;


                using (SmtpClient smtp = new SmtpClient(isFirst ? _emailSettings.PrimaryDomain : _emailSettings.SecondayDomain, isFirst ? _emailSettings.PrimaryPort: _emailSettings.SecondaryPort))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(isFirst ? _emailSettings.UsernameEmail: _emailSettings.UsernameEmail2, isFirst ? _emailSettings.UsernamePassword: _emailSettings.UsernamePassword2);  
                    await smtp.SendMailAsync(mail);
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        Task ISmsSender.SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailAsync(Messages msg)
        {
            Execute(msg.Email, msg.Subject,msg.Text, msg.Name,false).Wait();
            return Task.FromResult(0);
        }
    }
}
