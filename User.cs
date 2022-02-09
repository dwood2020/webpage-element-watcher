namespace Watcher {

    /// <summary>
    /// This class interface represents the user who shall receive element update notifications.
    /// </summary>
    public interface IUser {

        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// User Email address
        /// </summary>
        public string Email { get; }
    }


    /// <summary>
    /// This class implements the IUser interface.
    /// </summary>
    public class User {

        public string Name { get; set; }

        public string Mail { get; set; }

        public User() {
            Name = "";
            Mail = "";            
        }
    }
}