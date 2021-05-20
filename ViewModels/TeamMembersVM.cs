using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class TeamMembersVM : BaseViewModel
    {
        IConfiguration _configuration;
        public TeamMembersVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<TeamMembersDTO> TeamMembersDTO { get; set; }
        public IEnumerable<BranchDTO> BranchDTO { get; set; }
        public OutputHandler OutputHandler { get; set; }
         
    }
}
