using System.ComponentModel.DataAnnotations;

namespace LanguageLearningApp.Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(250)]
        public string PasswordHash { get; set; }  // Şifre düz metin tutulmayacak, hashlenecek.

        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
