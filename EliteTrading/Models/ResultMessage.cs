using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class ResultMessage {
        public bool Result { get; set; }
        public string Message { get; set; }
        public RepResult RepResult { get; set; }
    }
}