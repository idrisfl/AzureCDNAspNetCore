using AzureCDNAspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Cdn;
using Microsoft.Azure.Management.Cdn.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AzureCDNAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /* ================= ALL THESE VALUES WILL DISAPPEAR LONG BEFORE I COMMIT THIS CODE :-) :-) ==================== */
        private const string clientID = "5c9726de-e993-4582-ad0a-057f3a222d35";
        private const string clientSecret = "0kv~kK.jbi.GanoU~1pP4DdeKv3m5f680n";
        private const string authority = "https://login.microsoftonline.com/d5c8599f-4be6-479b-a8a8-f8d4cd6d2859/idrisflhotmail.onmicrosoft.com";

        //Application constants
        private const string subscriptionId = "d37c8b8a-f3ac-45c2-aa28-36aa9a1cf983";
        private const string profileName = "CDNConsoleProfile";
        private const string endpointName = "CDNProfileEndpoint";
        //Create a resource group and enter its name here
        private const string resourceGroupName = "test-stuff";
        private const string resourceLocation = "France Central";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            AuthenticationResult authResult = GetAccessToken();


            CdnManagementClient cdn = new CdnManagementClient(new TokenCredentials(authResult.AccessToken))
            { SubscriptionId = subscriptionId };

            // Create a new CDN Profile
            CreateCdnProfile(cdn);

            // Create a new CDN Endpoint
            CreateCdnEndpoint(cdn);

            Console.ReadKey();


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static AuthenticationResult GetAccessToken()
        {
            AuthenticationContext authContext = new AuthenticationContext(authority);
            ClientCredential credential = new ClientCredential(clientID, clientSecret);
            AuthenticationResult authResult =
                authContext.AcquireTokenAsync("https://management.core.windows.net/", credential).Result;

            return authResult;
        }

        private static void CreateCdnProfile(CdnManagementClient cdn)
        {
            Console.WriteLine("Creating profile {0}.", profileName);

            cdn.Profiles.Create(resourceGroupName, profileName, new Profile(resourceLocation, new Sku(SkuName.StandardMicrosoft)));

        }

        private static void CreateCdnEndpoint(CdnManagementClient cdn)
        {
            Console.WriteLine("Creating endpoint {0} on profile {1}.", endpointName, profileName);
            Endpoint endpointConfig =
                new Endpoint()
                {
                        //Origins = new List<DeepCreatedOrigin>() { new DeepCreatedOrigin("WiredbrainCoffee", "wbcoffee.azurewebsites.net") },

                        Origins = new List<DeepCreatedOrigin>() { new DeepCreatedOrigin("{Origin Server Name }", "{Origin Server Domain Name}") },
                    IsHttpAllowed = true,
                    IsHttpsAllowed = true,
                    Location = resourceLocation
                };

            cdn.Endpoints.Create(resourceGroupName, profileName, endpointName, endpointConfig);

        }
    }
}
