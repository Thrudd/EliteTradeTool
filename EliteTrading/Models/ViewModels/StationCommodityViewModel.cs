using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    [NotMapped]
    public class StationCommodityViewModel : StationCommodity {
        public string TimeSince { get; set; }
        public string CommodityName { get; set; }
        public string StationName { get; set; }

        public StationCommodity GetStationCommodity() {
            StationCommodity entity = new StationCommodity {
                Id = this.Id,
                StationId = this.StationId,
                CommodityId = this.CommodityId,
                Buy = this.Buy,
                Sell = this.Sell,
                LastUpdate = this.LastUpdate,
                UpdatedBy = this.UpdatedBy
            };

            return entity;
        }
    }
}