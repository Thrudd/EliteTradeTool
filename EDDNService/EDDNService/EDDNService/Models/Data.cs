using Newtonsoft.Json;
namespace EDDNService.Models {
    class data {
        [JsonProperty("header")]
        public header Header { get; set; }
        [JsonProperty("$schemaRef")]
        public string SchemaRef { get; set; }
        [JsonProperty("message")]
        public message Message { get; set; }
    }
}
