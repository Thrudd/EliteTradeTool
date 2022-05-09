using Newtonsoft.Json;
namespace EDDNService.Models {
    class commodity {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("buyPrice")]
        public int BuyPrice { get; set; }
        [JsonProperty("stockBracket")]
        public int? SupplyLevel { get; set; }
        [JsonProperty("stock")]
        public int Supply { get; set; }
        [JsonProperty("demandBracket")]
        public int? DemandLevel { get; set; }
        [JsonProperty("demand")]
        public int Demand { get; set; }
        [JsonProperty("sellPrice")]
        public int SellPrice { get; set; }
    }
}
