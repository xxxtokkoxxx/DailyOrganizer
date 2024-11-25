using System.Text.Json.Serialization;

public class UserTaskModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime TaskDate { get; set; }
    [JsonIgnore]
    public UserModel? User { get; set; }
}