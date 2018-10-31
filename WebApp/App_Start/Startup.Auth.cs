﻿using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web;
using WebApp.Utils;
using WebApp_OpenIDConnect_DotNet.Models;

namespace WebApp
{
    public partial class Startup
    {
        // The appId is used by the application to uniquely identify itself to Azure AD.
        // The appSecret is the application's password.
        // The aadInstance is the instance of Azure, for example public Azure or Azure China.
        // The redirectUri is where users are redirected after sign in and consent.
        private static string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private static string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static string nonAdminScopes = ConfigurationManager.AppSettings["ida:NonAdminScopes"];
        private static string adminScopes = ConfigurationManager.AppSettings["ida:AdminScopes"];
        private static string scopes = "openid email profile offline_access " + nonAdminScopes;

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOAuth2CodeRedeemer(
                new OAuth2CodeRedeemerOptions
                {
                    ClientId = appId,
                    ClientSecret = appSecret,
                    RedirectUri = redirectUri
                }
                );

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {

                    // The `Authority` represents the v2.0 endpoint - https://login.microsoftonline.com/common/v2.0
                    // The `Scope` describes the permissions that your app will need. See https://azure.microsoft.com/documentation/articles/active-directory-v2-scopes/                    
                    ClientId = appId,
                    Authority = String.Format(CultureInfo.InvariantCulture, aadInstance, "common", "/v2.0"),
                    RedirectUri = redirectUri,
                    Scope = scopes,
                    PostLogoutRedirectUri = redirectUri,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        // In a real application you would use IssuerValidator for additional checks, 
                        // like making sure the user's organization has signed up for your app.
                        //     IssuerValidator = (issuer, token, tvp) =>
                        //     {
                        //         if (MyCustomTenantValidation(issuer)) 
                        //             return issuer;
                        //         else
                        //             throw new SecurityTokenInvalidIssuerException("Invalid issuer");
                        //     },
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthorizationCodeReceived = async (context) =>
                        {
                            var code = context.Code;
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                            string graphScopes = nonAdminScopes;
                            string[] scopes = graphScopes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            ConfidentialClientApplication cca = new ConfidentialClientApplication(appId, redirectUri,
                               new ClientCredential(appSecret),
                               new SessionTokenCache(signedInUserID, context.OwinContext.Environment["System.Web.HttpContextBase"] as HttpContextBase).GetMsalCacheInstance(), null);
                            AuthenticationResult result = await cca.AcquireTokenByAuthorizationCodeAsync(code, scopes);

                            // Check whether the login is from the MSA tenant. 
                            // The sample uses this attribute to disable UI buttons for unsupported operations when the user is logged in with an MSA account(9188040d-6c67-4c5b-b112-36a304b66dad).
                            var currentTenantId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            if (currentTenantId == "91ff1688-1933-4d77-b0f6-f29035ccb4de")
                            {
                                HttpContext.Current.Session.Add("AccountType", "msa");
                            }
                            // Set IsAdmin session variable to false, since the user hasn't consented to admin scopes yet.
                            HttpContext.Current.Session.Add("IsAdmin", false);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Error?message=" + context.Exception.Message);
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}