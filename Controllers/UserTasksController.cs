using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyOrganizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public UserTasksController(ApplicationDbContext context, 
            IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        //GET: api/UserTasks/date
        [HttpGet("date/{date}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserTaskDto>>> GetTasksByDate(DateTime date)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc); 
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc); 
            endDate = endDate.AddDays(1);

            int userId = _userService.GetUserId(User);

            if (userId < 0)
            {
                return Forbid("Auth token is expired or incorect");
            }

            List<UserTaskModel> tasksOnDate = await _context.Tasks
           .Where(e => (e.TaskDate >= startDate && e.UserId == userId) && (e.TaskDate <= endDate && e.UserId == userId))
           .ToListAsync();
            List<UserTaskDto> userTasks = new List<UserTaskDto>();

            if (tasksOnDate == null || tasksOnDate.Count == 0)
            {
                return Ok(userTasks);
            }

            foreach (UserTaskModel eventModel in tasksOnDate)
            {
                userTasks.Add(new UserTaskDto(eventModel));
            }

            return Ok(userTasks);
        }

        //GET: api/UserTasks/date
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserTaskDto>>> GetTasksDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            int userId = _userService.GetUserId(User);

            if (userId < 0)
            {
                return Forbid("Auth token is expired or incorect");
            }

            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();

            List<UserTaskModel> tasksOnDate = await _context.Tasks
           .Where(e => (e.TaskDate.Date >= startDate.Date && e.UserId == userId) && (e.TaskDate.Date <= endDate.Date && e.UserId == userId))
           .ToListAsync();

            List<UserTaskDto> userTasks = new List<UserTaskDto>();

            if (tasksOnDate == null || tasksOnDate.Count == 0)
            {
                return Ok(userTasks);
            }

            foreach (UserTaskModel eventModel in tasksOnDate)
            {
                userTasks.Add(new UserTaskDto(eventModel));
            }

            return Ok(userTasks);
        }

        // GET: api/UserTasks/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserTaskDto>> GetUserTask(int id)
        {
            UserTaskModel? userTask = await _context.Tasks.FindAsync(id);

            if (userTask == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserId(User);

            if (userId < 0 || userTask.UserId != userId)
            {
                return Forbid("Auth token is expired or incorect");
            }

            UserTaskDto userTaskDto = new UserTaskDto(userTask);
            return userTaskDto;
        }

        // PUT: api/UserTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUserTask(int id, UserTaskDto userTaskDto)
        {
            UserTaskModel? userTask = await _context.Tasks.FindAsync(id);

            if (userTask == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserId(User);

            if (userId < 0 || userTask.UserId != userId)
            {
                return Forbid("Auth token is expired or incorect");
            }

            userTask.Title = userTaskDto.Title;
            userTask.Description = userTaskDto.Description;
            userTask.TaskDate = userTaskDto.TaskDate.ToUniversalTime();
            
            _context.Entry(userTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserTaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserTasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserTaskModel>> PostUserTask(UserTaskDto userTask)
        {
            int userId = _userService.GetUserId(User);

            if (userId < 0)
            {
                return Forbid("Auth token is expired or incorect");
            }

            DateTime date = userTask.TaskDate.ToUniversalTime();

            UserTaskModel @task = new UserTaskModel() 
            {
                UserId = userId,
                Title = userTask.Title,
                Description = userTask.Description,
                TaskDate = date
            };

            _context.Tasks.Add(@task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserTask", new { id = userTask.Id }, userTask);
        }

        // DELETE: api/UserTasks/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserTask(int id)
        {
            UserTaskModel? userTask = await _context.Tasks.FindAsync(id);
            if (userTask == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserId(User);

            if (userId < 0 || userTask.UserId != userId)
            {
                return Forbid("Auth token is expired or incorect");
            }

            _context.Tasks.Remove(userTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserTaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
