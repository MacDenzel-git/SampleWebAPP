using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class EventManagementVM : BaseViewModel
    {

        IConfiguration _configuration;
        public EventManagementVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<EventDTO> EventDTO { get; set; }
        public  EventDTO EventSingleObject { get; set; }
        public OutputHandler OutputHandler { get; set; }
        public IEnumerable<MinistryArmDTO> MinistryArms { get; set; }
        
    }
}
