using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LanguageLearningApp.Services
{
    public interface ILLMService
    {
        Task<string> GetAssistantReplyAsync(string prompt);
    }

    public class OllamaService : ILLMService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAssistantReplyAsync(string prompt)
        {
            // Ollama endpoint'i
            var requestUrl = "http://localhost:11434/api/generate";
            // Ollama parametreleri
            var requestBody = new
            {
                prompt = prompt,
                model = "llama2:13b-chat",
                stream = false
            };
            var json = JsonSerializer.Serialize(requestBody);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            // Ollama, chunk chunk stream döndürebilir;
            // basitçe tüm veriyi string olarak okuyoruz (daha gelişmiş senaryolarda streaming yapabilirsiniz).
            var responseContent = await response.Content.ReadAsStringAsync();

            // Ollama JSON formatında dönebiliyor,
            // Bazı versiyonlar yalnızca raw text veriyor. Formatı size göre uyarlayabilirsiniz.
            // Örneğin:
            // {
            //   "done": false,
            //   "model": "llama2-13b-chat",
            //   "response": "Merhaba! Nasıl yardımcı olabilirim?",
            //   "created_at": ...
            // }
            //
            // O yüzden parse etmemiz gerekebilir:

            var ollamaResult = JsonSerializer.Deserialize<OllamaResponse>(responseContent);
            return ollamaResult?.response ?? "";
        }
    }

    public class OllamaResponse
    {
        public bool done { get; set; }
        public string model { get; set; }
        public string response { get; set; }
        public DateTime created_at { get; set; }
    }
}
