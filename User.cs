namespace Watcher {

    /// <summary>
    /// This class represents the user who shall receive element update notifications.
    /// </summary>
    public class User {

        /// <summary>
        /// User Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// User Email address
        /// </summary>
        public string Mail { get; set; }

        public User() {
            Mail = "";
        }
    }
}