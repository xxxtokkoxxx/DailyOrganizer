using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyOrganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public EventsController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        //GET: api/Events/date
        [HttpGet("date/{date}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByDate(DateTime date)
        {
            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);

            int userId = _userService.GetUserId(User);

            if (userId < 0)
            {
                return Forbid("Auth token is expired or incorect");
            }

            List<EventModel> events = await _context.Events
                .Where(e => (e.StartDate.Date <= date.Date && e.UserId == userId)
                && (e.EndDate.Date >= date.Date && e.UserId == userId))
                .ToListAsync();
            List<EventDto> eventDtos = new List<EventDto>();

            if (events == null || events.Count == 0)
            {
                return Ok(events);
            }

            foreach (EventModel ev in events)
            {
                eventDtos.Add(new EventDto(ev));
            }

            return Ok(events);
        }

        //GET: api/Events/date
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            int userId = _userService.GetUserId(User);

            if (userId < 0)
            {
                return Forbid("Auth token is expired or incorect");
            }

            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();

            List<EventModel> events = await _context.Events
                .Where(e => (e.StartDate.Date <= endDate.Date && e.UserId == userId)
                && (e.EndDate.Date >= startDate.Date && e.UserId == userId))
                .ToListAsync();

            List<EventDto> eventDtos = new List<EventDto>();

            if (events == null || events.Count == 0)
            {
                return Ok(events);
            }


            foreach (EventModel ev in events)
            {
                eventDtos.Add(new EventDto(ev));
            }

            return Ok(events);
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            EventModel? @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserId(User);

            if (userId < 0 || @event.UserId != userId)
            {
                return Forbid();
            }

            EventDto eventDto = new EventDto(@event);
            return eventDto;
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutEvent(int id, EventDto @event)
        {
            EventModel? eventModel = await _context.Events.FindAsync(id);

            if (@eventModel == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserId(User);

            if (userId < 0 || eventModel.UserId != userId)
            {
                return Forbid();
            }

            eventModel.StartDate = @event.StartDate.ToUniversalTime();
            eventModel.EndDate = @event.EndDate.ToUniversalTime();
            eventModel.Title = @event.Title;
            eventModel.Description = @event.Description;
            eventModel.NotifyBefore = @event.NotifyBefore;
            _context.Events.Update(eventModel);

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EventModel>> PostEvent([FromBody] EventDto @event)
        {
            int id = _userService.GetUserId(User);

            if (id < 0)
            {
                return NotFound("Auth token is expired or incorect");
            }

            UserModel? user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            EventModel eventModel = new EventModel()
            {
                UserId = id,
                Title = @event.Title,
                Description = @event.Description,
                StartDate = @event.StartDate.ToUniversalTime(),
                EndDate = @event.EndDate.ToUniversalTime(),
                User = user,
                NotifyBefore = @event.NotifyBefore
            };

            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserId(User);

            if (userId < 0)
            {
                return NotFound("Auth token is expired or incorect");
            }

            if (@event.UserId != userId)
            {
                return Forbid();
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
