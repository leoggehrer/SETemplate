//@BaseCode
//MdStart
using System.Text.Json.Serialization;

namespace TemplateTools.Logic.Models.ChatGPT
{
    public partial class Request
    {
        [JsonPropertyName("model")]
        public required string Model { get; set; }
        [JsonPropertyName("messages")]
        public Message[] Messages { get; set; } = [];
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 64;
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;
    }
}
//MdEnd