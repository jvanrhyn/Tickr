using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tickr.Client.Models;

namespace Tickr.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async  Task<IActionResult> Index()
        {
            var data = await GetAllTodo();
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private async Task<List<TodoReply>> GetAllTodo()
        {
            var serverAddress = "https://localhost:5001";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                serverAddress = "http://localhost:5000";
            }
            
            var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new Todo.TodoClient(channel);

            var accessToken = await GetAccessToken();
            var headers = new Metadata {{"Authorization", $"Bearer {accessToken}"}};


            var response = 
                client.GetAll(new TodoFilterRequest() {IncludeCompleted = true}, headers);

            var replies = new List<TodoReply>();
            
            await foreach (var resp in  response.ResponseStream.ReadAllAsync())
            {
                replies.Add(resp);
            }

            return replies;
        }
        
        
         async Task<string> GetAccessToken()
        {
            var appAuth0Settings = _configuration.GetSection("Auth0");
            var auth0Client = new AuthenticationApiClient(appAuth0Settings["Domain"]);
            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = appAuth0Settings["ClientId"],
                ClientSecret = appAuth0Settings["ClientSecret"],
                Audience = appAuth0Settings["Audience"]
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
        }
    }
}