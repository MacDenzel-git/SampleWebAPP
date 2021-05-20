using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class HomePageVM : BaseViewModel
    {

        IConfiguration _configuration;
        public HomePageVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
            
        public HomePageDTO HomePageDTO { get; set; }
        public string TimerDateTime { get; internal set; }
    }
}
