using System.Net.Mail;
using System.Net;
using System.Text;

namespace EstateProManager.Models
{
    public class Utility
    {
        private const int _RoleAdministrator = 1;
        private const int _RoleClient = 2;

        private const string _ApplicationName = "EstatePro Manager";

        public static int RoleAdministrator { get { return _RoleAdministrator; } }

        public static int RoleClient { get { return _RoleClient; } }
        
        public static string ApplicationName { get { return _ApplicationName; } }

        public static bool IsUserLoggedIn(HttpContext hc)
        {
            try
            {
                return hc.Session.Keys.Count() > 0 &&
                   (hc.Session.GetString("ID") != null ||
                    hc.Session.GetString("ID") != "");
            }
            catch
            {
                return false;
            }
        }

        public static string GeneratePassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+=<>?";
            StringBuilder password = new StringBuilder();

            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                password.Append(chars[index]);
            }

            return password.ToString();
        }

        public static void SendEmail(string to, string subject, string body)
        {
            using (SmtpClient client = new SmtpClient("SMTP SERVER"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential("EMAIL", "PASSWORD");
                client.EnableSsl = true;

                MailMessage message = new MailMessage
                {
                    From = new MailAddress("EstateProManager@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                message.To.Add(to);

                client.Send(message);
            }

        }
    }
}
