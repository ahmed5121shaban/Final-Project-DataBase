namespace Final.Models
{
    public class PhoneNumber
    {
        public int ID { get; set; }
        public string Phone { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}