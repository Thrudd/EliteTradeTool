using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class System {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Allegiance { get; set; }
        public string Economy { get; set; }
        public string Government { get; set; }
        public List<Station> Stations { get; set; }
    }
}