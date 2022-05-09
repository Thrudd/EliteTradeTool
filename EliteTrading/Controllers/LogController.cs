using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Controllers {
    [Authorize(Roles = "Elite")]
    public class LogController : Controller {
        //
        // GET: /Log/
        public ActionResult Index() {
            return View();
        }

        public async Task<ActionResult> Clear() {
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var conn = new SqlConnection(connString)) {
                var cmd = new SqlCommand("DELETE FROM [ELMAH_Error]", conn);
                try {
                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                } catch (Exception ex) {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                } finally {
                    cmd.Dispose();
                }
            }
            return RedirectToAction("Index");
        }
    }
}