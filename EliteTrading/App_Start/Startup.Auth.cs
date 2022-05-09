using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using EliteTrading.Models;
using EliteTrading.Entities;
using System.Configuration;

namespace EliteTrading
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: ConfigurationManager.AppSettings["TwitterConsumerKey"],
            //   consumerSecret: ConfigurationManager.AppSettings["TwitterConsumerSecret"]);

            app.UseTwitterAuthentication(new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions {
                ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                BackchannelCertificateValidator = new Microsoft.Owin.Security.CertificateSubjectKeyIdentifierValidator(new[] {
        "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
        "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
        "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
        "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
        "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
        "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
        "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
    })
            });
            app.UseFacebookAuthentication(
               appId: ConfigurationManager.AppSettings["FacebookAppId"],
               appSecret: ConfigurationManager.AppSettings["FacebookAppSecret"]);

            //app.UseGoogleAuthentication();
        }
    }
}