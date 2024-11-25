using System.Text.Json.Serialization;

public class EventModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int NotifyBefore { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [JsonIgnore]
    public UserModel? User { get; set; }
}