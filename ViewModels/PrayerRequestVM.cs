﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class PrayerRequestVM: BaseViewModel
    {

        IConfiguration _configuration;
        public PrayerRequestVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

    }
}
