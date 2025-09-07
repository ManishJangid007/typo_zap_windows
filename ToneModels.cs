using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TypoZap
{
    public class ToneData
    {
        [JsonProperty("tones")]
        public List<Tone> Tones { get; set; } = new List<Tone>();
    }

    public class Tone
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("prompt")]
        public string Prompt { get; set; } = string.Empty;
    }
}
