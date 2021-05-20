﻿using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs.Authentication
{
    public class UserLoginResource
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public OutputHandler OutputHandler { get; set; }
    }
}
