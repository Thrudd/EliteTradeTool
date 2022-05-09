using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EliteTrading.Models.ViewModels;
using EliteTrading.Models;
using EliteTrading.Entities;
using System.Configuration;


namespace EliteTrading.Controllers {
    [Authorize(Roles = "Elite")]
    public class UsersAdminController : Controller {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public UsersAdminController() {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager) {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager {
            get {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        public ActionResult Index() {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> FilterBy(string q, string filterType = "CommanderName") {
            UserListViewModel model = new UserListViewModel();

            if (q != string.Empty) {
                if (filterType == "CommanderName") {
                    model.Users =  await (from u in db.Users
                                   where (u.CommanderName.Contains(q))
                                   orderby u.CommanderName
                                   select u).ToListAsync();
                } else {
                    model.Users = await (from u in db.Users
                                   where (u.UserName.Contains(q))
                                   orderby u.CommanderName
                                   select u).ToListAsync();
                }
            }
            ViewBag.Filtered = true;
            ViewBag.FilterName = q;
            model.Page = 1;
            model.TotalPages = 1;
            return PartialView("_List", model);

        }

        // GET: /Users/List
        public async Task<ActionResult> List(int page = 0, string sort = "CommanderName") {
            UserListViewModel model = new UserListViewModel();
            List <ApplicationUser> users = await GetListAsync(sort);
            model.Users = users.Skip(50 * page).Take(50).ToList();
            model.Page = page;
            model.TotalPages = db.Users.Count() / 50;
            return View("_List", model);
        }

        public async Task<List<ApplicationUser>> GetListAsync(string sort) {
            List<ApplicationUser> list = new List<ApplicationUser>();
            var pi = typeof(ApplicationUser).GetProperty(sort);
            list = await db.Users.ToListAsync();

            return await Task.FromResult<List<ApplicationUser>>(list.OrderBy(x => pi.GetValue(x, null)).ToList());
        }

        public async Task<JsonResult> ListForSearch(string term) {
            List<ApplicationUser> users = await GetListAsync("CommanderName");

            var results = users.Where(m => m.CommanderName.Contains(term)).Select(d => new { value = d.CommanderName });
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id, int page) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            
            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            ViewBag.rank = new SelectList(RoleManager.Roles.OrderByDescending(x => x.Rank), "Name", "Name");
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRank(string id, string rank) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            var userRoles = await UserManager.GetRolesAsync(user.Id);

            if (rank == "Harmless") {
                user.Reputation = 0;
                user.Rank = 1;
                user.ReputationNeeded = 25;
                user.Title = "Harmless";
                user.Badge = "harmless.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                await UserManager.AddToRoleAsync(user.Id, "Harmless");
            } else if (rank == "Mostly Harmless") {
                user.Reputation = 25;
                user.Rank = 2;
                user.ReputationNeeded = 25;
                user.Title = "Mostly Harmless";
                user.Badge = "mostlyharmless.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Novice") {
                user.Reputation = 50;
                user.Rank = 3;
                user.ReputationNeeded = 200;
                user.Title = "Novice";
                user.Badge = "novice.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Competent") {
                user.Reputation = 250;
                user.Rank = 4;
                user.ReputationNeeded = 1750;
                user.Title = "Competent";
                user.Badge = "competent.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                roles.Add("Competent");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Expert") {
                user.Reputation = 2000;
                user.Rank = 5;
                user.ReputationNeeded = 2000;
                user.Title = "Expert";
                user.Badge = "expert.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                roles.Add("Competent");
                roles.Add("Expert");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Master") {
                user.Reputation = 4000;
                user.Rank = 6;
                user.ReputationNeeded = 4000;
                user.Title = "Master";
                user.Badge = "master.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                roles.Add("Competent");
                roles.Add("Expert");
                roles.Add("Master");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Dangerous") {
                user.Reputation = 8000;
                user.Rank = 7;
                user.ReputationNeeded = 8000;
                user.Title = "Dangerous";
                user.Badge = "dangerous.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                roles.Add("Competent");
                roles.Add("Expert");
                roles.Add("Master");
                roles.Add("Dangerous");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Deadly") {
                user.Reputation = 16000;
                user.Rank = 8;
                user.ReputationNeeded = 319984000;
                user.Title = "Deadly";
                user.Badge = "Deadly.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                roles.Add("Competent");
                roles.Add("Expert");
                roles.Add("Master");
                roles.Add("Dangerous");
                roles.Add("Deadly");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            } else if (rank == "Elite") {
                user.Reputation = 320000000;
                user.Rank = 9;
                user.ReputationNeeded = 0;
                user.Title = "Elite";
                user.Badge = "elite.png";
                await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.ToList<string>());
                List<string> roles = new List<string>();
                roles.Add("Harmless");
                roles.Add("Mostly Harmless");
                roles.Add("Novice");
                roles.Add("Competent");
                roles.Add("Expert");
                roles.Add("Master");
                roles.Add("Dangerous");
                roles.Add("Deadly");
                roles.Add("Elite");
                await UserManager.AddUserToRolesAsync(user.Id, roles);
            }

            return Json(rank + " applied");
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create() {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles) {
            if (ModelState.IsValid) {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded) {
                    if (selectedRoles != null) {
                        var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded) {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                } else {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel() {
                Id = user.Id,
                Email = user.Email,
                CommanderName = user.CommanderName,
                Rank = user.Rank,
                Reputation = user.Reputation,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem() {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel editUser, params string[] selectedRole) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null) {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.CommanderName = editUser.CommanderName;
                user.Reputation = editUser.Reputation;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRole.Except(userRoles).ToList<string>());

                if (!result.Succeeded) {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(editUser);
                }
                result = await UserManager.RemoveUserFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToList<string>());

                if (!result.Succeeded) {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(editUser);
                }
                return RedirectToAction("Index", new { page=1 });
            }
            ModelState.AddModelError("", "Something failed.");
            return View(editUser);
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id) {
            if (ModelState.IsValid) {
                if (id == null) {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null) {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded) {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //public ActionResult GiveRole() {
        //    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    List<ApplicationUser> list = db.Users.ToList();
        //    foreach (ApplicationUser user in list) {
        //        var rolesForUser = userManager.GetRoles(user.Id);
        //        if (user.Email != "sean@whatsyourtechnique.com") {
        //            if (!rolesForUser.Contains("Harmless")) {
        //                var result = userManager.AddToRole(user.Id, "Harmless");
        //            }
        //            if (!rolesForUser.Contains("Mostly Harmless")) {
        //                var result = userManager.AddToRole(user.Id, "Mostly Harmless");
        //            }
                    
        //            user.Rank = 2;
        //            user.Reputation = 50;
        //            user.ReputationNeeded = 50;
        //            user.Title = "Mostly Harmless";
        //            user.Badge = "mostlyharmless.png";
        //            user.Updates = 0;
        //            user.Queries = 0;
        //        }
        //    }
        //    db.SaveChanges();

        //    return Content("Done");
        //}
    }
}