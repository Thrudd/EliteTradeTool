using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Models {
    public class AdminSelectLists {
        public List<SelectListItem> Allegiances { get; set; }
        public List<SelectListItem> Economies { get; set; }
        public List<SelectListItem> Governments { get; set; }
        public List<SelectListItem> Systems { get; set; }
        public List<SelectListItem> StationTypes { get; set; }
        public List<AdminCategoryCommodityList> CommodityCategories { get; set; }
    }
}