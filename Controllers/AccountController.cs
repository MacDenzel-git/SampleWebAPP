using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs.Authentication;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace projectWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private const string Token = "Token";
        private const string LoggedInUser = "username";
        static string apiUriAccount = "Auth";

        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }


        public AccountController(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            UserLoginResource userLogin = new UserLoginResource
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = false
                }
            };
            return View(userLogin);
        }




        [HttpPost]
        public async Task<IActionResult> Login(UserLoginResource loginCredentials)
        {
            var requestUrl = $"{BaseUrl}{apiUriAccount}/SignIn";
            OutputHandler outputHandler = new OutputHandler();

            //Get Sermons
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl, loginCredentials);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    outputHandler = await response.Content.ReadAsAsync<OutputHandler>();
                    if (!outputHandler.IsErrorOccured)
                    { 
                        HttpContext.Session.Clear();
                        HttpContext.Session.SetString(LoggedInUser, outputHandler.Message);
                        HttpContext.Session.SetString(Token, outputHandler.Result.ToString());
                        var sessionDetails = await StaticDataHandler.GetSessionDetails();
                        if (sessionDetails.IsSet)
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            UserLoginResource userLogin = new UserLoginResource
                            {
                                OutputHandler = new OutputHandler { IsErrorOccured = true, Message = "Login was successful, System has experienced a technical fault, Contact TechArch" }
                            };
                            return View(userLogin);
                        }
                    }

                };
                outputHandler = await response.Content.ReadAsAsync<OutputHandler>();

            };
            UserLoginResource userLoginResource = new UserLoginResource
            {
                OutputHandler = new OutputHandler { IsErrorOccured = true, Message = outputHandler.Message }
            };
            return View(userLoginResource);


        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            UserSignUpResource userSignUpResource = new UserSignUpResource
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = false
                }
            };
            return View(userSignUpResource);
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpResource signUpCredentials)
        {
            var requestUrl = $"{BaseUrl}{apiUriAccount}/SignUp";
            OutputHandler outputHandler = new OutputHandler();

            //Get Sermons
            using (var client = new HttpClient())
            {
                signUpCredentials.Username = signUpCredentials.Email;
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl, signUpCredentials);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    outputHandler = await response.Content.ReadAsAsync<OutputHandler>();
                    if (!outputHandler.IsErrorOccured)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                }
                else
                {
                    outputHandler = await response.Content.ReadAsAsync<OutputHandler>();
                };

            };
            UserSignUpResource userSignUpResource = new UserSignUpResource
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = outputHandler.Message
                }
            };
            return View(userSignUpResource);


        }
    }
}
