using Postal;

namespace EliteTrading.Models {
    public class AccountEmail : Email {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        public string Message { get; set; }
    }
}