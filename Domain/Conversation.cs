using System.ComponentModel.DataAnnotations;

namespace LanguageLearningApp.Domain
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; } // Kimlik doğrulama sonrasında eklenecek.
        public string TopicTitle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Message> Messages { get; set; }
    }
}
