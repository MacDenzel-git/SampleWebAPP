using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs.Authentication
{
    public class SessionDetailsDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public bool IsSet { get; set; }
    }
}
