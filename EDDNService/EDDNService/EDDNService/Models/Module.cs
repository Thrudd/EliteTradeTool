using Newtonsoft.Json;
namespace EDDNService.Models {
    class module {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("rating")]
        public char Rating { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("class")]
        public int Class { get; set; }
    }
}