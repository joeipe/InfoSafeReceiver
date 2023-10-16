namespace InfoSafeReceiver.API.Messages
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DoB { get; set; } = null!;
    }
}
