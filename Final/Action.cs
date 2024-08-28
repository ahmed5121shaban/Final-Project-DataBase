public class Action
{
    public int Id { get; set; }
    public string Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int SellerId { get; set; }
    //public virtual ICollection <User> Users  { get; set; }
    //public virtual ICollection<Item> Items { get; set; }
}