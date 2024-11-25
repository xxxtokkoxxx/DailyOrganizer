using System.ComponentModel;
using System.Text.Json.Serialization;

public class EventDto
{
    [JsonConstructor]
    public EventDto() { }
    public EventDto(EventModel @event)
    {
        Id = @event.Id;
        Title = @event.Title;
        Description = @event.Description;
        NotifyBefore = @event.NotifyBefore;
        StartDate = @event.StartDate;
        EndDate = @event.EndDate;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int NotifyBefore { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}