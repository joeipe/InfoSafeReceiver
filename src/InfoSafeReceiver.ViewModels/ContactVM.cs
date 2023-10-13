namespace InfoSafeReceiver.ViewModels
{
    public class ContactVM
    {
        public int Id { get; set; }
        public int RefId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DoB { get; set; }
    }
}