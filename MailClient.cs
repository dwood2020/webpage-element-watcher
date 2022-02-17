using MailKit;
using MailKit.Net.Smtp;


namespace Watcher {

    /// <summary>
    /// This is a very simple SMTP client wrapper,
    /// used for quick and dirty mail messaging.     
    /// </summary>
    public class MailClient {

        private string server;
        private int port;
        private string addr;
        private string pw;

        private IUser user;

        public MailClient(string server, int port, string addr, string pw, IUser user) {
            this.server = server;
            this.port = port;
            this.addr = addr;
            this.pw = pw;
            this.user = user;
        }
    }
}