using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebApp_OpenIDConnect_DotNet.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Graph;
using WebApp_OpenIDConnect_DotNet.Helpers;
using Resources;

namespace WebApp.Controllers
{
    
    public class HomeController : Controller
    {
        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

        MailService mailService = new MailService();
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Name = ClaimsPrincipal.Current.FindFirst("name").Value;
            ViewBag.AuthorizationRequest = string.Empty;
            // The object ID claim will only be emitted for work or school accounts at this time.
            Claim oid = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            ViewBag.ObjectId = oid == null ? string.Empty : oid.Value;

            // The 'preferred_username' claim can be used for showing the user's primary way of identifying themselves
            ViewBag.Username = ClaimsPrincipal.Current.FindFirst("preferred_username").Value;

            // The subject or nameidentifier claim can be used to uniquely identify the user
            ViewBag.Subject = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return View();
        }

        [Authorize]
        public async Task<ActionResult> SendMail()
        {            
            // try to get token silently
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            TokenCache userTokenCache = new SessionTokenCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();            
            ConfidentialClientApplication cca = new ConfidentialClientApplication(clientId, redirectUri,new ClientCredential(appKey), userTokenCache, null);
            var accounts = await cca.GetAccountsAsync();
            if (accounts.Any())
            {
                string[] scopes = { "Mail.Send" };
                try
                {
                    AuthenticationResult result = await cca.AcquireTokenSilentAsync(scopes, accounts.First());
                }
                catch (MsalUiRequiredException)
                {
                    try
                    {// when failing, manufacture the URL and assign it
                        string authReqUrl = await WebApp.Utils.OAuth2RequestManager.GenerateAuthorizationRequestUrl(scopes, cca, this.HttpContext, Url);
                        ViewBag.AuthorizationRequest = authReqUrl;
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                }
            }
            else
            {

            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SendMail(string recipient, string subject, string body)
        {
            string messagetemplate = @"{{
  ""Message"": {{
    ""Subject"": ""{0}"",
    ""Body"": {{
                ""ContentType"": ""Text"",
      ""Content"": ""{1}""
    }},
    ""ToRecipients"": [
      {{
        ""EmailAddress"": {{
          ""Address"": ""{2}""
        }}
}}
    ],
    ""Attachments"": []
  }},
  ""SaveToSentItems"": ""false""
}}
";
            string message = String.Format(messagetemplate, subject, body, recipient);

            
            HttpClient client = new HttpClient();            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/microsoft.graph.sendMail");
            
            request.Content = new StringContent(message, Encoding.UTF8, "application/json");

            // try to get token silently
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            TokenCache userTokenCache = new SessionTokenCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
            ConfidentialClientApplication cca = new ConfidentialClientApplication(clientId, redirectUri, new ClientCredential(appKey), userTokenCache, null);
            var accounts = await cca.GetAccountsAsync();
            if (accounts.Any())
            {
                string[] scopes = { "Mail.Send" };
                try
                {
                    AuthenticationResult result = await cca.AcquireTokenSilentAsync(scopes, accounts.First());

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.AuthorizationRequest = null;
                        return View("MailSent");
                    }
                }
                catch (MsalUiRequiredException)
                {
                    try
                    {// when failing, manufacture the URL and assign it
                        string authReqUrl = await WebApp.Utils.OAuth2RequestManager.GenerateAuthorizationRequestUrl(scopes, cca, this.HttpContext, Url);
                        ViewBag.AuthorizationRequest = authReqUrl;
                    }
                    catch (Exception ee)
                    {
                        Response.Write(ee.Message);
                    }
                }
            }
            else { }
            return View();
        }

        public async Task<ActionResult> ReadMail()
        {
            ResultsViewModel results = new ResultsViewModel();
            try
            {

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Get the messages.
                results.Items = await mailService.GetMyMessages(graphClient);
                ViewBag.Message = results.Items.FirstOrDefault().Display;
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return View();
        }
        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/Home/ReadMail" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}