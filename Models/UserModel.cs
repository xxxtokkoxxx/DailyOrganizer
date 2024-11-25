
using System.Text.Json.Serialization;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }

    [JsonIgnore]
    public virtual ICollection<EventModel>? Events { get; set; }
    [JsonIgnore]
    public virtual ICollection<UserTaskModel>? UserTasks { get; set; }
}