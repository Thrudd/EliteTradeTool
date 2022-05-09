using EliteTrading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace EliteTrading.Services {
    public class EDDNChecker {
        private static System.Timers.Timer aTimer;
        public EDDNChecker(){
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(CheckEDDN);
            aTimer.Interval = 45000;
            aTimer.Enabled = true;
        }

        private async void CheckEDDN(object source, ElapsedEventArgs e) {
            try {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri("http://eddn.elitetradingtool.co.uk/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // New code:
                    HttpResponseMessage response = await client.GetAsync("/EDDN/EDDNStatus");
                    if (response.IsSuccessStatusCode) {
                        EDDNStatus status = await response.Content.ReadAsAsync<EDDNStatus>();
                        DefaultCacheProvider _cache = new DefaultCacheProvider();
                        _cache.Set("EDDNStatus", status, 1);
                    }
                }
            } catch (Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }            
    }

    public class EDDNStatus{
        public bool Connected { get; set; }
        public string Status { get; set; }
        public EDDNStatus() {
            Connected = false;
            Status = "";
        }
    }
}