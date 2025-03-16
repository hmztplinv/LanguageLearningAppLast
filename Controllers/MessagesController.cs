using System.Security.Claims;
using System.Text;
using LanguageLearningApp.Data;
using LanguageLearningApp.Domain;
using LanguageLearningApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/conversations/{conversationId}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILLMService _llmService;

    public MessagesController(AppDbContext context, ILLMService llmService)
    {
        _context = context;
        _llmService = llmService;
    }
   

    // GET: api/conversations/5/messages
    [HttpGet]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Önce, bu konuşma gerçekten bu kullanıcıya mı ait?
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation == null)
            return NotFound("Conversation not found or you don't have access.");

        // Konuşmaya ait mesajları çekelim
        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return Ok(messages);
    }

    
    [HttpPost]
public async Task<IActionResult> CreateMessage(int conversationId, [FromBody] CreateMessageDto model)
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    // 1) Kullanıcı bu konuşmaya ait mi?
    var conversation = await _context.Conversations
        .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

    if (conversation == null)
        return NotFound("Conversation not found or you don't have access.");

    // 2) Kullanıcı mesajını (role=user) DB'ye kaydet
    var userMessage = new Message
    {
        ConversationId = conversationId,
        UserId = userId,
        Role = "user",
        Content = model.Content,
        CreatedAt = DateTime.UtcNow
    };
    _context.Messages.Add(userMessage);
    await _context.SaveChangesAsync();

    // 3) Son X mesajı çek (ör. 10) ve prompt oluştur
    var lastMessages = await _context.Messages
        .Where(m => m.ConversationId == conversationId)
        .OrderByDescending(m => m.CreatedAt)
        .Take(10)
        .ToListAsync();

    lastMessages = lastMessages.OrderBy(m => m.CreatedAt).ToList(); // eskiden yeniye sıralayalım

    var promptBuilder = new StringBuilder();

    // (isteğe bağlı) system prompt
    promptBuilder.AppendLine($"System: You are an English teacher. Correct grammar mistakes...");
    foreach (var msg in lastMessages)
    {
        if (msg.Role == "user")
            promptBuilder.AppendLine($"User: {msg.Content}");
        else
            promptBuilder.AppendLine($"Assistant: {msg.Content}");
    }
    promptBuilder.AppendLine("Assistant:");

    var prompt = promptBuilder.ToString();

    // 4) LLM'e istek
    var assistantReply = await _llmService.GetAssistantReplyAsync(prompt);

    // 5) Asistan mesajını (role=assistant) DB'ye kaydet
    var assistantMessage = new Message
    {
        ConversationId = conversationId,
        Role = "assistant",
        Content = assistantReply,
        CreatedAt = DateTime.UtcNow
    };
    _context.Messages.Add(assistantMessage);
    await _context.SaveChangesAsync();

    // 6) İsteğe bağlı: Kullanıcıya dönülecek veriler
    return Ok(new
    {
        userMessage = userMessage.Content,
        assistantMessage = assistantMessage.Content
    });
}


}
