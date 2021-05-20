using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.Controllers
{
    public class PartnershipController : Controller
    {
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }

        IConfiguration _configuration;
        public PartnershipController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public IActionResult PartnerWithUs()
        {
            PartnersWithUsVM partnersWithUs = new PartnersWithUsVM(_configuration);
            return View(partnersWithUs);
        }

        public IActionResult GivingPlatform()
        {
            ProjectFinancePlatformsVM givingPlatforms = new ProjectFinancePlatformsVM(_configuration);
            return View(givingPlatforms);
            
        }
    }
}
