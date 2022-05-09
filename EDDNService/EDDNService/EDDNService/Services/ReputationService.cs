using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using EDDNService.Extensions;
using Microsoft.AspNet.Identity.Owin;

namespace EDDNService.Services {
    public class ReputationService {

        public async Task<RepResult> AddRepAsync(string userName, int rep) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {


                RepResult result = new RepResult();

                // Get the user and add the rep
                ApplicationUser user = await db.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync();
                user.Reputation += rep;


                //// Try to get the roles list from cache
                //List<ApplicationRole> roles = new List<ApplicationRole>();
                //DefaultCacheProvider _cache = new DefaultCacheProvider();
                //if (_cache.IsSet("Roles")) {
                //    roles = (List<ApplicationRole>)_cache.Get("Roles");
                //} else {
                //    roles = await db.Roles.OrderBy(x => x.Rank).ToListAsync();
                //    _cache.Set("Roles", roles, 86400);
                //}

                //// Set the current Title and Badge
                //result.Badge = roles[user.Rank - 1].Badge;
                //result.Title = roles[user.Rank - 1].Name;

                //// See if the user has leveled up
                //if (user.Rank < 9) {
                //    if (user.Reputation >= roles[user.Rank].RepRequired) { // User has more rep then the next highest rank
                //        // Rank up and assign values to the result object
                //        user.Rank += 1;
                //        user.Title = roles[user.Rank - 1].Name;
                //        user.Badge = roles[user.Rank - 1].Badge;
                //        result.RankUp = true;
                //        result.Badge = user.Badge;
                //        result.Title = user.Title;

                //        // Assign the new role
                //        var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //        await userManager.AddToRoleAsync(user.Id, roles[user.Rank - 1].Name);
                //        user.Reputation = roles[user.Rank - 1].RepRequired;
                //    }
                //    // Calculate the rep needed for the next rank
                //    user.ReputationNeeded = roles[user.Rank].RepRequired - user.Reputation;
                //}


                // Save the changes
                await db.SaveChangesAsync();

                // Assign the rest of the values to the result object
                result.CommanderName = user.CommanderName;
                result.Reputation = user.Reputation;
                result.ReputationNeeded = user.ReputationNeeded;

                return result;
            }
        }

        public RepResult AddRep(string userName, int rep) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {


                RepResult result = new RepResult();

                // Get the user and add the rep
                ApplicationUser user = db.Users.Where(x => x.UserName == userName).FirstOrDefault();
                user.Reputation += rep;


                //// Try to get the roles list from cache
                //List<ApplicationRole> roles = new List<ApplicationRole>();
                //DefaultCacheProvider _cache = new DefaultCacheProvider();
                //if (_cache.IsSet("Roles")) {
                //    roles = (List<ApplicationRole>)_cache.Get("Roles");
                //} else {
                //    roles = db.Roles.OrderBy(x => x.Rank).ToList();
                //    _cache.Set("Roles", roles, 86400);
                //}

                //// Set the current Title and Badge
                //result.Badge = roles[user.Rank - 1].Badge;
                //result.Title = roles[user.Rank - 1].Name;

                //// See if the user has leveled up
                //if (user.Rank < 9) {
                //    if (user.Reputation >= roles[user.Rank].RepRequired) { // User has more rep then the next highest rank
                //        // Rank up and assign values to the result object
                //        user.Rank += 1;
                //        user.Title = roles[user.Rank - 1].Name;
                //        user.Badge = roles[user.Rank - 1].Badge;
                //        result.RankUp = true;
                //        result.Badge = user.Badge;
                //        result.Title = user.Title;

                //        // Assign the new role
                //        var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //        userManager.AddToRoleAsync(user.Id, roles[user.Rank - 1].Name);
                //        user.Reputation = roles[user.Rank - 1].RepRequired;
                //    }
                //    // Calculate the rep needed for the next rank
                //    user.ReputationNeeded = roles[user.Rank].RepRequired - user.Reputation;
                //}


                // Save the changes
                db.SaveChanges();

                // Assign the rest of the values to the result object
                result.CommanderName = user.CommanderName;
                result.Reputation = user.Reputation;
                result.ReputationNeeded = user.ReputationNeeded;

                return result;
            }
        }
    }

    public class RepResult {
        public string CommanderName { get; set; }
        public int Reputation { get; set; }
        public int ReputationNeeded { get; set; }
        public string Title { get; set; }
        public bool RankUp { get; set; }
        public string Badge { get; set; }
    }

    public class ActionRep {
        public const int Query = 1;
        public const int PriceCheck = 5;
        public const int CommodityChange = 10;
        public const int AddedStationCommodity = 20;
        public const int AddEditCommodity = 100;
        public const int ImportData = 300;
        public const int AddEditSystem = 500;
        public const int AddEditStation = 500;
    }
}