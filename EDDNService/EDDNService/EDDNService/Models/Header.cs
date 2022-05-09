using Newtonsoft.Json;
namespace EDDNService.Models {
    class header {
        [JsonProperty("softwareVersion")]
        public string SoftwareVersion { get; set; }
        [JsonProperty("gatewayTimestamp")]
        public string GatewayTimestamp { get; set; }
        [JsonProperty("softwareName")]
        public string SoftwareName { get; set; }
        [JsonProperty("uploaderId")]
        public string UploaderId { get; set; }
    }
}
