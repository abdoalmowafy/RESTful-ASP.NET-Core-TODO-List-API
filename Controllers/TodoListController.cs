using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListAPI.Data;

namespace TodoListAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoList()
        {

            var todoItems = await _context.TodoList.Where(t => t.UserName == User.Identity!.Name).ToListAsync();
            return Ok(todoItems);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetItem(int id)
        {
            var todoItem = await _context.TodoList.FirstOrDefaultAsync(t => t.Id == id);

            if (todoItem == null || todoItem.UserName != User.Identity!.Name) return NotFound("TODO item not found");

            return Ok(todoItem);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> AddTodoItem([FromBody] string title)
        {
            var todoItem = new TodoItem { Title = title, UserName = User.Identity!.Name! };
            await _context.TodoList.AddAsync(todoItem);
            await _context.SaveChangesAsync();

            return Ok(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id)
        {
            var todoItem = await _context.TodoList.FindAsync(id);

            if (todoItem == null || todoItem.UserName != User.Identity!.Name) return NotFound("TODO item not found");

            todoItem.Status = todoItem.Status switch
            {
                Status.pending => Status.completed,
                Status.completed => Status.pending,
                _ => throw new Exception("Invalid Status")
            };

            await _context.SaveChangesAsync();
            return Ok(todoItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoList.FindAsync(id);

            if (todoItem == null || todoItem.UserName != User.Identity!.Name) return NotFound("TODO item not found");

            _context.TodoList.Remove(todoItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}