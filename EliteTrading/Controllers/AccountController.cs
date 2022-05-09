using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using EliteTrading.Models;
using EliteTrading.Models.ViewModels;
using EliteTrading.Entities;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using Postal;
using Elmah;
using System.Configuration;
using System.Net.Mail;

namespace EliteTrading.Controllers {
    [Authorize]
    public class AccountController : Controller {
        private ApplicationUserManager _userManager;

        public AccountController() {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationRoleManager roleManager) {
            UserManager = userManager;
            RoleManager = roleManager;
        }

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

        //[AllowAnonymous]
        //public ActionResult InitAdminUser() {
        //    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //   var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
        //    const string roleName = "Elite";
        //    const string name = "sean@whatsyourtechnique.com";

        //    //Create Role Admin if it does not exist
        //    var role = roleManager.FindByName(roleName);
        //    if (role == null) {
        //        role = new IdentityRole(roleName);
        //        var roleresult = roleManager.Create(role);
        //    }
        //    var user = userManager.FindByName(name);

        //    // Add user admin to Role Admin if not already added
        //    var rolesForUser = userManager.GetRoles(user.Id);
        //    if (!rolesForUser.Contains(role.Name)) {
        //        var result = userManager.AddToRole(user.Id, role.Name);
        //    }

        //    string resultString = "User not found.";

        //    using (ApplicationDbContext db = new ApplicationDbContext()) {
        //        ApplicationUser appUser = db.Users.First(u => u.UserName == name);
        //        if (appUser != null) {
        //            db.SaveChanges();
        //            resultString = "Done, " + name + " is Admin.";
        //        }
        //    }
        //    return Content(resultString);
        //}

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        
        public async Task<ActionResult> ConfCode(string email) {
            // Send an email with this link
            var user = await UserManager.FindByEmailAsync(email);
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = "http://www.elitetradingtool.co.uk" + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code });

            return Content(callbackUrl);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null) {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                } else {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model) {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (String.IsNullOrEmpty(recaptchaHelper.Response)) {
                ModelState.AddModelError("", "Captcha answer cannot be empty.");
                return View("Register", model);
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success) {
                ModelState.AddModelError("", "Incorrect captcha answer.");
            }
            if (ModelState.IsValid) {
                var user = new ApplicationUser() { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    CommanderName = model.CommanderName, 
                    Reputation = 0, 
                    ReputationNeeded = 50, 
                    Rank = 1, 
                    Queries = 0,
                    Updates = 0, 
                    Title = "Harmless", 
                    Badge = "harmless.png",
                    
                };
                
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    UserManager.AddToRole(user.Id, "Harmless");
                    using (ApplicationDbContext db = new ApplicationDbContext()) {
                        ApplicationUser u = db.Users.Where(m => m.UserName == model.Email).FirstOrDefault();
                        if (u != null) {
                            u.UserName = model.Email;
                            u.Email = model.Email;
                            u.CommanderName = model.CommanderName;
                            u.Reputation = 0;
                            u.ReputationNeeded = 50;
                            u.Rank = 1;
                            u.Queries = 0;
                            u.Updates = 0;
                            u.Title = "Harmless";
                            u.Badge = "harmless.png";
                        }
                        db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    await SignInAsync(user, isPersistent: false);

                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = "http://www.elitetradingtool.co.uk" + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code });

                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.Email);
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                    mail.Subject = "Confirm your account";
                    mail.Body = "Please confirm your account by clicking the following link. <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a>";
                    mail.IsBodyHtml = true;
                    try {
                        SmtpClient smtp = new SmtpClient();
                        smtp.Send(mail);
                    } catch (Exception ex) {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        throw;
                    }

                    return RedirectToAction("Index", "Main");
                } else {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code) {
            if (userId == null || code == null) {
                return View("Error");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded) {
                return View("ConfirmEmail");
            } else {
                AddErrors(result);
                return View();
            }
        }

        //
        // GET: /Account/ResendConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ResendConfirmEmail(string emailAddress) {
            // Send an email with this link
            var user = await UserManager.FindByEmailAsync(emailAddress);
            if (user != null) {
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = "http://www.elitetradingtool.co.uk" + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code });

                MailMessage mail = new MailMessage();
                mail.To.Add(emailAddress);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.Subject = "Confirm your account";
                mail.Body = "Please confirm your account by clicking the following link. <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a>";
                mail.IsBodyHtml = true;
                try {
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(mail);
                } catch (Exception ex) {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw;
                }
                return RedirectToAction("ConfirmEmailSent");
            }
            return View("UserNotFound");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword() {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null) {
                    ModelState.AddModelError("", "There is no user account tied to that email address.");
                    return View();
                } else if (!(await UserManager.IsEmailConfirmedAsync(user.Id))) {
                    ViewBag.UserId = model.Email;
                    return View("ResendConfirmEmail");
                }

                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = "http://www.elitetradingtool.co.uk" + Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code });
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                MailMessage mail = new MailMessage();
                mail.To.Add(model.Email);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.Subject = "Reset Password";
                mail.Body = "Please reset your password by clicking following link. <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a>";
                mail.IsBodyHtml = true;

                try {
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(mail);
                } catch (Exception ex) {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw;
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code) {
            if (code == null) {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null) {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded) {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                } else {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation() {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey) {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded) {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            } else {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> Manage(ManageMessageId? message) {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            ApplicationUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            ViewBag.User = user;
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model) {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword) {
                if (ModelState.IsValid) {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded) {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    } else {
                        AddErrors(result);
                    }
                }
            } else {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null) {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid) {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded) {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    } else {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl) {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl) {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null) {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            } else {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider) {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback() {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null) {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded) {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl) {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid) {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null) {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    CommanderName = model.CommanderName,
                    Reputation = 0,
                    ReputationNeeded = 50,
                    Rank = 1,
                    Queries = 0,
                    Updates = 0,
                    Title = "Harmless",
                    Badge = "harmless.png"
                };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded) {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    UserManager.AddToRole(user.Id, "Harmless");
                    if (result.Succeeded) {
                        await SignInAsync(user, isPersistent: false);

                        // Send an email with this link
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = "http://www.elitetradingtool.co.uk" + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code });
                        //SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

                        MailMessage mail = new MailMessage();
                        mail.To.Add(model.Email);
                        mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                        mail.Subject = "Confirm your account";
                        mail.Body = "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>";
                        mail.IsBodyHtml = true;

                        try {
                            SmtpClient smtp = new SmtpClient();
                            smtp.Send(mail);

                        } catch (Exception ex) {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            throw;
                        }

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Main");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure() {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList() {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && UserManager != null) {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent) {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword() {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null) {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message) {
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771
        }

        public enum ManageMessageId {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Main");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) {
            }

            public ChallengeResult(string provider, string redirectUri, string userId) {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context) {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null) {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}