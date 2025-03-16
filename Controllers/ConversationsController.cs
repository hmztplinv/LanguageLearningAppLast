using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LanguageLearningApp.Data;
using LanguageLearningApp.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LanguageLearningApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConversationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/conversations
        [HttpGet]
        public async Task<IActionResult> GetUserConversations()
        {
            // Token'dan UserId çekelim
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversations = await _context.Conversations
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Basit DTO dönebilirsiniz veya direkt entity
            return Ok(conversations);
        }
                // POST api/conversations
        [HttpPost]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = new Conversation
            {
                UserId = userId,
                TopicTitle = model.TopicTitle,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            return Ok(conversation);
        }
                // GET api/conversations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversation(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (conversation == null)
                return NotFound("Conversation not found or you don't have access.");

            return Ok(conversation);
        }
                // DELETE api/conversations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversation(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (conversation == null)
                return NotFound("Conversation not found or you don't have access.");

            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();

            return Ok("Conversation deleted.");
        }
    }
}



