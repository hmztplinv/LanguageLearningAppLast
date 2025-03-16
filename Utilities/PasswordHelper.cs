using BCrypt.Net;

namespace LanguageLearningApp.Utilities
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // BCrypt kütüphanesinin HashPassword metodu
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            // Kullanıcının girdiği şifreyi (password) DB'de tutulan hash ile karşılaştırma
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
