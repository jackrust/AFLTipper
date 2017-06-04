using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add("jack.timothy.rust@gmail.com");
            message.Subject = "This is the Subject line";
            message.From = new System.Net.Mail.MailAddress("jack.timothy.rust+tippingbot@gmail.com");
            message.Body = "This is the message body";
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("yoursmtphost");
            smtp.Send(message);
        }
    }
}
