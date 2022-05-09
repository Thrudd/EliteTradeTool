using EliteTrading.Models;
using EliteTrading.Entities;
using EliteTrading.Models.ViewModels;
using EliteTrading.Services;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Net.Mail;

namespace EliteTrading.Controllers
{
    public class ContactController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        // GET: Contact
        public ActionResult Index() {
            return View();
        }


        [HttpPost]
        public ActionResult Index(ContactViewModel model) {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (String.IsNullOrEmpty(recaptchaHelper.Response)) {
                ModelState.AddModelError("", "Captcha answer cannot be empty.");
                return View(model);
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success) {
                ModelState.AddModelError("", "Incorrect captcha answer.");
            }

            if (ModelState.IsValid) {
                MailMessage mail = new MailMessage();
                mail.To.Add("thrudd@elitetradingtool.co.uk");
                mail.From = new MailAddress(model.Email);
                mail.Subject = "Website Inquiry";
                mail.Body = model.Message;
                mail.IsBodyHtml = true;

                try {
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(mail);
                    return View("MailSent");
                } catch (Exception ex) {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw;
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}