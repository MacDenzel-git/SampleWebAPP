using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class ContactUsVM : BaseViewModel
    {

        IConfiguration _configuration;
        public ContactUsVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
            
        public BranchDTO Branch { get; set; }
    }
}
