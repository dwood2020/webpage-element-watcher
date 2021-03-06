using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;


namespace Watcher {


    public interface IMailClient {

        public void SendMessage(string subject, string message);
    }


    /// <summary>
    /// This is a very simple SMTP client wrapper,
    /// used for quick and dirty mail messaging.     
    /// </summary>
    public class MailClient : IMailClient {

        public string server;
        public int port;
        
        private string pw;

        private MailboxAddress sender;
        private MailboxAddress recipient;

        private ILogger logger;

        public MailClient(ILogger logger, string server, int port, string name, string addr, string pw, IUser user) {
            this.server = server;
            this.port = port;            
            this.pw = pw;
            sender = new MailboxAddress(name, addr);
            recipient = new MailboxAddress(user.Name, user.Mail);

            this.logger = logger;
        }


        public void SendMessage(string subject, string message) {
            using var client = new SmtpClient();

            try {
                client.Connect(server, port, SecureSocketOptions.StartTls);
                client.Authenticate(sender.Address, pw);

                var msg = new MimeMessage();
                msg.From.Add(sender);
                msg.To.Add(recipient);
                msg.Subject = subject;
                msg.Body = new TextPart("plain") {
                    Text = message,
                };

                client.Send(msg);
            }
            catch (Exception ex) {
                // NOTE: Catch all exceptions locally here to keep running
                logger.Error("MailClient: SendMessage failed - Exception ocurred:\n" + ex.ToString());
            }
            
        }
    }
}