using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Controllers
{
    public class CleanupController : Controller
    {
        // GET: Cleanup
        public async Task<ActionResult> Index() {
            return View(await GetSystems());
        }

        // POST: Cleanup/Delete/5
        [HttpPost]
        public async Task<ActionResult> Index(IEnumerable<int> duplicateSystemDeletebyId) {
            if (ModelState.IsValid) {
                try {
                    using (ApplicationDbContext db = new ApplicationDbContext()) {

                        db.Systems.RemoveRange(db.Systems.Where(m => duplicateSystemDeletebyId.Contains(m.Id)));

                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                } catch(Exception ex){
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            
            return View(await GetSystems());
        }

        private async Task<List<DuplicateSystem>> GetSystems() {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd = new SqlCommand("DuplicatedSystems", con);
            cmd.CommandType = CommandType.StoredProcedure;

            List<DuplicateSystem> model = new List<DuplicateSystem>();

            try {
                // Execute the command.
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows) {


                    /* Second is the starting stations goods for sale */
                    while (reader.Read()) {
                        DuplicateSystem item = new DuplicateSystem();
                        item.Id = (int)reader["Id"];
                        item.Name = reader["Name"].ToString();
                        item.DevData = (bool)reader["DevData"];
                        item.Allegiance = reader["Allegiance"].ToString();
                        item.Government = reader["Government"].ToString();
                        item.Station = reader["Station"].ToString();
                        model.Add(item);
                    }
                }
                reader.Close();
            } catch (Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            } finally {
                con.Close();
            }
            return model;
        }
    }
}

public class DuplicateSystem {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool DevData { get; set; }
    public string Allegiance { get; set; }
    public string Government { get; set; }
    public string Station { get; set; }
}