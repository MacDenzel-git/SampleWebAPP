using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using projectWebApplication.Models;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace projectWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private const string Token = "Token";
        private readonly IConfiguration _configuration;

        static string apiUriHome = "Utils";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var requestUrl = $"{BaseUrl}{apiUriHome}/HomePageSetup";
            HomePageVM homePageVM = new HomePageVM(_configuration);

            //Get Sermons
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    homePageVM.HomePageDTO = await response.Content.ReadAsAsync<HomePageDTO>();

                };

            };

             return View(homePageVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            AboutUsVM aboutUsVM = new AboutUsVM(_configuration);
            return View(aboutUsVM);
        }

        public IActionResult StatementOfFaith()
        {
            StatementOfFaithVM aboutUsVM = new StatementOfFaithVM(_configuration);
            return View(aboutUsVM);
        }

        public IActionResult GlamorousLadies()
        {
            return View();
        }

        public IActionResult ZoneZ()
        {
            return View();
        }

        public IActionResult ZoneW()
        {
            return View();
        }

        public IActionResult Partnership()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            ContactUsVM contact = new ContactUsVM(_configuration);
            contact.Branch = new BranchDTO
            {
                BranchPastorName = "Denzel"
            };
             return View(contact);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //public async Task<IActionResult> GetMenusItems()
        //{
        //    BaseViewModel menu = new BaseViewModel();
        //    menu = await StaticDataHandler.GetDynamicMenuItems(BaseUrl);

        //    return Json(menu, new Newtonsoft.Json.JsonSerializerSettings());
        //}

        public IActionResult SalvationPrayer()
        {
            SalvationPrayerVM salvation = new SalvationPrayerVM(_configuration);
            return View(salvation);
        }

        public IActionResult PrayerRequest()
        {
            PrayerRequestVM prayerRequest = new PrayerRequestVM(_configuration);
            return View(prayerRequest);
        }

        public async Task<IActionResult> BranchDetailsByTeamMember(int teamMemberId)
        {
            BranchVM branch = new BranchVM(_configuration);
            var requestUrl = $"{BaseUrl}Branch/GetBranchDetailsByTeamMemberId?teamMemberId={teamMemberId}";

            //Get Sermons
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    branch.Branch = await response.Content.ReadAsAsync<BranchDTO>();

                };

            };

            //branch.PageSetup.PageTitle = $"{branch.Branch.BranchName} Branch Details";
            branch.PageSetup.PageTitle = $"{branch.Branch.BranchName} Branch Details";
            return View(branch);
        }
         
        public async Task<IActionResult> BranchDetails(int branchId)
        {
            BranchVM branch = new BranchVM(_configuration);
            var requestUrl = $"{BaseUrl}Branch/GetBranchDetailsByBranchId?branchId={branchId}";

            //Get Sermons
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    branch.Branch = await response.Content.ReadAsAsync<BranchDTO>();

                };

            };

            //branch.PageSetup.PageTitle = $"{branch.Branch.BranchName} Branch Details";
            branch.PageSetup.PageTitle = $"{branch.Branch.BranchName} Branch Details";
            return View(branch);
        }
        

       
        public async Task<IActionResult> AjaxBranchDetails(int? branchId)
        {
            BranchVM data = new BranchVM(_configuration);
            var requestUrl = $"{BaseUrl}Branch/GetBranchDetailsByBranchId?branchId={branchId}";

            //Get Sermons
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    data.Branch = await response.Content.ReadAsAsync<BranchDTO>();

                };

            };

            //branch.PageSetup.PageTitle = $"{branch.Branch.BranchName} Branch Details";
            data.PageSetup.PageTitle = $"{data.Branch.BranchName} Branch Details";
            
            return Json(data.Branch);
        }
    }
}
