using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class MinistryArmVM: BaseViewModel
    {

        IConfiguration _configuration;
        public MinistryArmVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
            
     
     
      
        public IEnumerable<MinistryArmDTO> MinistryArmsList { get; set; }
        public MinistryArmDTO MinistryArmDTO { get; set; }
        public OutputHandler OutputHandler { get; set; }
    }
}
