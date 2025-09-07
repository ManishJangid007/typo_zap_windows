using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace TypoZap
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private string? _apiKey;
        private List<Tone> _availableTones = new List<Tone>();
        private string _selectedTone = "default";
        
        public GeminiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            // Load previously saved API key
            _apiKey = LoadApiKeySecurely();
            
            // Load available tones
            LoadTones();
        }
        
        public bool HasValidApiKey()
        {
            return !string.IsNullOrEmpty(_apiKey);
        }
        
        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey?.Trim();
            SaveApiKeySecurely(apiKey);
        }
        
        public void SetSelectedTone(string toneTitle)
        {
            _selectedTone = toneTitle ?? "default";
        }
        
        public List<Tone> GetAvailableTones()
        {
            return _availableTones.ToList();
        }
        
        public async Task<string?> CorrectGrammarAsync(string text)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("No API key found. Please set your Gemini API key.");
            }
            
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be empty or whitespace.");
            }
            
            try
            {
                Console.WriteLine($"🤖 Sending text to Gemini for correction: {text.Substring(0, Math.Min(50, text.Length))}...");
                
                var request = new GeminiRequest
                {
                    Contents = new[]
                    {
                        new GeminiContent
                        {
                            Parts = new[]
                            {
                                new GeminiPart
                                {
                                    Text = CreatePrompt(text)
                                }
                            }
                        }
                    },
                    GenerationConfig = new GeminiGenerationConfig
                    {
                        Temperature = 0.1,
                        TopK = 1,
                        TopP = 0.8,
                        MaxOutputTokens = 1024
                    }
                };
                
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = $"{_baseUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ HTTP Error {response.StatusCode}: {errorContent}");
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseContent);
                
                if (geminiResponse?.Candidates?.Length > 0 && 
                    geminiResponse.Candidates[0]?.Content?.Parts?.Length > 0)
                {
                    var correctedText = geminiResponse.Candidates[0].Content.Parts[0].Text?.Trim();
                    Console.WriteLine($"✅ Gemini correction successful: {correctedText?.Substring(0, Math.Min(50, correctedText?.Length ?? 0))}...");
                    return correctedText;
                }
                else
                {
                    Console.WriteLine("❌ Invalid response structure from Gemini API");
                    throw new InvalidOperationException("Invalid response structure from Gemini API");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling Gemini API: {ex.Message}");
                throw;
            }
        }
        
        private string CreatePrompt(string text)
        {
            // Find the selected tone
            var selectedTone = _availableTones.FirstOrDefault(t => t.Title == _selectedTone) 
                              ?? _availableTones.FirstOrDefault();
            
            if (selectedTone != null)
            {
                // Replace {text} placeholder with the actual text
                return selectedTone.Prompt.Replace("{text}", text);
            }
            
            // Fallback to default prompt if tone not found
            return $@"Please correct the grammar, spelling, and punctuation in the following text. 
Return only the corrected text without any explanations or additional formatting:

{text}";
        }
        
        private void SaveApiKeySecurely(string apiKey)
        {
            try
            {
                if (string.IsNullOrEmpty(apiKey)) return;
                
                // Use Windows Data Protection API to encrypt the API key
                var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);
                var encryptedData = ProtectedData.Protect(apiKeyBytes, null, DataProtectionScope.CurrentUser);
                
                // Store in user's app data folder
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var typoZapPath = Path.Combine(appDataPath, "TypoZap");
                Directory.CreateDirectory(typoZapPath);
                
                var configPath = Path.Combine(typoZapPath, "config.dat");
                File.WriteAllBytes(configPath, encryptedData);
                
                Console.WriteLine("🔐 API key saved securely");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving API key securely: {ex.Message}");
            }
        }
        
        private void LoadTones()
        {
            try
            {
                var tonesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tones.json");
                if (File.Exists(tonesPath))
                {
                    var json = File.ReadAllText(tonesPath);
                    var toneData = JsonConvert.DeserializeObject<ToneData>(json);
                    _availableTones = toneData?.Tones ?? new List<Tone>();
                }
                else
                {
                    // Fallback to default tone if file doesn't exist
                    _availableTones = new List<Tone>
                    {
                        new Tone
                        {
                            Title = "default",
                            Description = "Corrects grammar, spelling, and punctuation without changing the tone or meaning of the text.",
                            Prompt = "Please correct the grammar, spelling, and punctuation in the following text. Return only the corrected text without any explanations or additional formatting:\n{text}"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading tones: {ex.Message}");
                // Fallback to default tone
                _availableTones = new List<Tone>
                {
                    new Tone
                    {
                        Title = "default",
                        Description = "Corrects grammar, spelling, and punctuation without changing the tone or meaning of the text.",
                        Prompt = "Please correct the grammar, spelling, and punctuation in the following text. Return only the corrected text without any explanations or additional formatting:\n{text}"
                    }
                };
            }
        }

        private string? LoadApiKeySecurely()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var configPath = Path.Combine(appDataPath, "TypoZap", "config.dat");
                
                if (!File.Exists(configPath)) return null;
                
                var encryptedData = File.ReadAllBytes(configPath);
                var decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading API key: {ex.Message}");
                return null;
            }
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
    
    // Request/Response models for Gemini API
    public class GeminiRequest
    {
        [JsonProperty("contents")]
        public GeminiContent[] Contents { get; set; } = Array.Empty<GeminiContent>();
        
        [JsonProperty("generationConfig")]
        public GeminiGenerationConfig GenerationConfig { get; set; } = new();
    }
    
    public class GeminiContent
    {
        [JsonProperty("parts")]
        public GeminiPart[]? Parts { get; set; }
    }
    
    public class GeminiPart
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
    
    public class GeminiGenerationConfig
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
        
        [JsonProperty("topK")]
        public int TopK { get; set; }
        
        [JsonProperty("topP")]
        public double TopP { get; set; }
        
        [JsonProperty("maxOutputTokens")]
        public int MaxOutputTokens { get; set; }
    }
    
    public class GeminiResponse
    {
        [JsonProperty("candidates")]
        public GeminiCandidate[]? Candidates { get; set; }
    }
    
    public class GeminiCandidate
    {
        [JsonProperty("content")]
        public GeminiContent? Content { get; set; }
    }
}
