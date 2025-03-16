using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LanguageLearningApp.Domain
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key
        public int ConversationId { get; set; }
        [ForeignKey(nameof(ConversationId))]
        [JsonIgnore]
        public Conversation Conversation { get; set; }

        // Kim gönderdi? (Kullanıcı ID, asistan mesajıysa null olabilir veya "Sistem" rolu vs.)
        public int? UserId { get; set; } 

        // user / assistant / system gibi roller
        [MaxLength(20)]
        public string Role { get; set; }  

        [Required]
        public string Content { get; set; }

        public string? ErrorAnalysis { get; set; }

        public bool IsCorrected { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
