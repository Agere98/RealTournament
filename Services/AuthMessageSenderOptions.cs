using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTournament.Services
{
    public class AuthMessageSenderOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
