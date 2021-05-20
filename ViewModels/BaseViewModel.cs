using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class BaseViewModel
    {
        static string apiUriMinistryArms = "MinistryArm";
        public IEnumerable<MinistryArmDTO> MinistryArms { get; set; }
        public IEnumerable<ResourceTypeDTO> ResourceTypes { get; set; }
        public IEnumerable<BranchDTO> Branches { get; set; }
        public string ScrollBarHeight { get; set; }
        public PageSetupDTO PageSetup { get; set; }
        public MenuDTO MenuDTO { get; set; }
        IConfiguration _configuration;
        public static string _baseUrl;
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }

        public BaseViewModel(IConfiguration configuration)
        {

            _configuration = configuration;
            _baseUrl = _configuration["EndpointURL"];
            MenuDTO menu = new MenuDTO();
            var response = StaticDataHandler.GetDynamicMenuItems(_baseUrl);//This gets menu list from the database for all pages to be populated on load
            this.PageSetup = new PageSetupDTO { PageTitle = "Title" };//initialising tto avoid null exception 
            menu = JsonConvert.DeserializeObject<MenuDTO>(response);
            this.MinistryArms = menu.MinistryArms;
            this.ResourceTypes = menu.ResourceTypes;
            this.Branches = menu.Branches;

        }



        
    }
}

 

