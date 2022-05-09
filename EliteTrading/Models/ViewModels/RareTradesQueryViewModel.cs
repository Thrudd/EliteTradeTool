using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Models.ViewModels {
    public class RareTradesQueryViewModel {
        public int RareTradeId { get; set; }
        public List<SelectListItem> RareTrades { get; set; }
    }
}