using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TravelIdeasPortalWeb.Controllers
{
    // Bindings to all routes not handled by other controllers/routes
    [Controller]
    public class UnboundRoutesController : Controller
    {
        /// <summary>
        /// Error handling page
        /// </summary>
        [HttpGet("/Error")]
        public IActionResult ErrorPage()
        {
            IExceptionHandlerFeature exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandler == null)
            {
                return View("ErrorView", null);
            }

            return View("ErrorView", exceptionHandler.Error);
        }

        /// <summary>
        /// Terms of Service
        /// </summary>
        [HttpGet("/terms")]
        [HttpGet("/terms-of-service")]
        [HttpGet("/termsofservice")]
        public IActionResult ShowTermsOfService()
        {
            return View("TermsOfServiceView");
        }

        /// <summary>
        /// Privacy Policy
        /// </summary>
        [HttpGet("/privacy")]
        [HttpGet("/privacy-policy")]
        [HttpGet("/privacypolicy")]
        public IActionResult ShowPrivacyPolicy()
        {
            return View("PrivacyPolicyView");
        }

        /// <summary>
        /// Generate and service robots.txt file
        /// </summary>
        // handle robots.txt requests
        [HttpGet("/robots.txt")]
        public IActionResult GenerateRobotsTxt()
        {
            return View("RobotsTxtView");
        }

        /// <summary>
        /// Generate and service sitemap.xml file
        /// </summary>
        [HttpGet("/sitemap.xml")]
        public IActionResult GenerateSiteMap()
        {
            return View("SitemapView");
        }

        // stop php hack attempts
        [Route("{*.php}")]
        public void HandleHackingAttemptsPHP()
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.WriteAsync($"Your IP address '{HttpContext.Connection.RemoteIpAddress.ToString()}' has been noted and is being reported to the FBI.").Wait();
        }

    }
}
