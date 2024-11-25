using System.Text.Json.Serialization;

public class UserTaskDto
{
    [JsonConstructor]
    public UserTaskDto() { }
    public UserTaskDto(UserTaskModel model)
    {
        Id = model.Id;
        Title = model.Title;
        Description = model.Description;
        TaskDate = model.TaskDate;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime TaskDate { get; set; }
}